using System.Globalization;

public abstract record Expr;
public sealed record LiteralExpr(object? Value) : Expr;
public sealed record UnaryExpr(Token Operator, Expr Right) : Expr;
public sealed record BinaryExpr(Expr Left, Token Operator, Expr Right) : Expr;
public sealed record GroupingExpr(Expr Expression) : Expr;

public sealed class Parser
{
    private readonly List<Token> tokens;
    private readonly string[] sourceLines;
    private int current;

    public bool HadError { get; private set; }

    public Parser(List<Token> tokens, string source)
    {
        this.tokens = tokens;
        sourceLines = source.Replace("\r\n", "\n").Split('\n');
    }

    public IReadOnlyList<Expr> Parse()
    {
        var expressions = new List<Expr>();
        int start = 0;

        for (int i = 0; i < tokens.Count - 1; i++)
        {
            if (IsLineBoundary(i, start))
            {
                ParseChunk(start, i, expressions);
                start = i;
            }
        }

        ParseChunk(start, tokens.Count - 1, expressions);
        return HadError ? Array.Empty<Expr>() : expressions;
    }

    private bool IsLineBoundary(int index, int chunkStart)
    {
        if (index == 0)
            return false;

        Token previous = tokens[index - 1];
        Token next = tokens[index];
        if (previous.Line == next.Line || previous.Type == TokenType.LEFT_PAREN)
            return false;

        int depth = 0;
        for (int i = chunkStart; i < index; i++)
        {
            if (tokens[i].Type == TokenType.LEFT_PAREN) depth++;
            else if (tokens[i].Type == TokenType.RIGHT_PAREN) depth--;
        }

        return depth == 0 && !CanContinue(tokens[index - 1].Type);
    }

    private static bool CanContinue(TokenType type) => type is
        TokenType.PLUS or TokenType.MINUS or TokenType.STAR or TokenType.SLASH or TokenType.PERCENT or
        TokenType.EQUAL_EQUAL or TokenType.NOT_EQUAL or TokenType.LESS or TokenType.LESS_EQUAL or
        TokenType.GREATER or TokenType.GREATER_EQUAL or TokenType.NOT or TokenType.LEFT_PAREN or TokenType.COMMA;

    private void ParseChunk(int start, int end, List<Expr> expressions)
    {
        if (start >= end)
            return;

        var chunk = tokens.GetRange(start, end - start);
        if (end == tokens.Count - 1)
            chunk.Add(tokens[end]);
        else
        {
            Token last = tokens[end - 1];
            chunk.Add(new Token(TokenType.EOF, "", null, last.Line, last.Column + last.Lexeme.Length));
        }

        var parser = new Parser(chunk, source);
        try
        {
            expressions.Add(parser.Expression());
            if (!parser.Check(TokenType.EOF))
                throw parser.Error(parser.Peek(), "Unexpected token after expression.");
        }
        catch (ParseError error)
        {
            parser.Report(error);
            HadError = true;
        }
    }

    private Expr Expression() => Equality();

    private Expr Equality()
    {
        Expr expression = Comparison();
        while (Match(TokenType.EQUAL_EQUAL, TokenType.NOT_EQUAL))
        {
            Token op = Previous();
            expression = new BinaryExpr(expression, op, Comparison());
        }
        return expression;
    }

    private Expr Comparison()
    {
        Expr expression = Term();
        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token op = Previous();
            expression = new BinaryExpr(expression, op, Term());
        }
        return expression;
    }

    private Expr Term()
    {
        Expr expression = Factor();
        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            Token op = Previous();
            expression = new BinaryExpr(expression, op, Factor());
        }
        return expression;
    }

    private Expr Factor()
    {
        Expr expression = Unary();
        while (Match(TokenType.SLASH, TokenType.STAR, TokenType.PERCENT))
        {
            Token op = Previous();
            expression = new BinaryExpr(expression, op, Unary());
        }
        return expression;
    }

    private Expr Unary()
    {
        if (Match(TokenType.NOT, TokenType.MINUS))
        {
            Token op = Previous();
            return new UnaryExpr(op, Unary());
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE)) return new LiteralExpr(false);
        if (Match(TokenType.TRUE)) return new LiteralExpr(true);
        if (Match(TokenType.NIL)) return new LiteralExpr(null);
        if (Match(TokenType.NUMBER, TokenType.STRING)) return new LiteralExpr(Previous().Literal);

        if (Match(TokenType.LEFT_PAREN))
        {
            Expr expression = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new GroupingExpr(expression);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private bool Match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw Error(Peek(), message);
    }

    private bool Check(TokenType type) => Peek().Type == type;
    private Token Advance() { if (!Check(TokenType.EOF)) current++; return Previous(); }
    private bool AtEnd() => Check(TokenType.EOF);
    private Token Peek() => tokens[current];
    private Token Previous() => tokens[current - 1];

    private ParseError Error(Token token, string message) => new(token, message);

    private void Report(ParseError error)
    {
        HadError = true;
        Token token = error.Token;
        string where = token.Type == TokenType.EOF ? "at end" : $"at '{token.Lexeme}'";
        Console.Error.WriteLine($"[line {token.Line}, col {token.Column}] Error {where}: {error.Message}");

        int lineIndex = token.Line - 1;
        if (lineIndex >= 0 && lineIndex < sourceLines.Length)
        {
            string text = sourceLines[lineIndex].Replace('\t', ' ');
            string gutter = token.Line.ToString(CultureInfo.InvariantCulture);
            Console.Error.WriteLine($"  {gutter} | {text}");
            Console.Error.WriteLine($"  {new string(' ', gutter.Length)} | {new string(' ', Math.Max(0, token.Column - 1))}^");
        }
    }

    private sealed class ParseError(Token token, string message) : Exception(message)
    {
        public Token Token { get; } = token;
    }
}

public static class AstPrinter
{
    public static string Print(Expr expression) => expression switch
    {
        LiteralExpr literal => PrintLiteral(literal.Value),
        UnaryExpr unary => Parenthesize(unary.Operator.Lexeme, unary.Right),
        BinaryExpr binary => Parenthesize(binary.Operator.Lexeme, binary.Left, binary.Right),
        GroupingExpr grouping => Parenthesize("group", grouping.Expression),
        _ => throw new ArgumentOutOfRangeException(nameof(expression))
    };

    private static string Parenthesize(string name, params Expr[] expressions) =>
        $"({name}{(expressions.Length == 0 ? "" : " " + string.Join(" ", expressions.Select(Print)))})";

    private static string PrintLiteral(object? value) => value switch
    {
        null => "nil",
        bool boolean => boolean ? "true" : "false",
        double number => number.ToString("0.0###############", CultureInfo.InvariantCulture),
        string text => $"\"{Escape(text)}\"",
        _ => value.ToString() ?? "nil"
    };

    private static string Escape(string text) => text
        .Replace("\\", "\\\\")
        .Replace("\n", "\\n")
        .Replace("\t", "\\t")
        .Replace("\"", "\\\"");
}
