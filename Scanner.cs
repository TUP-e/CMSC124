using System.Text;

public class Scanner
{
    private readonly string source;
    private readonly List<Token> tokens = new();

    private int start = 0;
    private int current = 0;
    private int line = 1;

    public bool HadError { get; private set; }

    private static readonly Dictionary<string, TokenType> Keywords =
        new()
        {
            { "class", TokenType.CLASS },
            { "extends", TokenType.EXTENDS },
            { "new", TokenType.NEW },
            { "this", TokenType.THIS },
            { "public", TokenType.PUBLIC },
            { "private", TokenType.PRIVATE },
            { "abstract", TokenType.ABSTRACT },
            { "print", TokenType.PRINT },

            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "while", TokenType.WHILE },
            { "for", TokenType.FOR },
            { "return", TokenType.RETURN },
            { "break", TokenType.BREAK },
            { "continue", TokenType.CONTINUE },

            { "true", TokenType.TRUE },
            { "false", TokenType.FALSE },
            { "nil", TokenType.NIL }
        };

    public Scanner(string source)
    {
        this.source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line));

        return tokens;
    }

    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            // Single-character tokens
            case '(':
                AddToken(TokenType.LEFT_PAREN);
                break;

            case ')':
                AddToken(TokenType.RIGHT_PAREN);
                break;

            case '{':
                AddToken(TokenType.LEFT_BRACE);
                break;

            case '}':
                AddToken(TokenType.RIGHT_BRACE);
                break;

            case '.':
                AddToken(TokenType.DOT);
                break;

            case ',':
                AddToken(TokenType.COMMA);
                break;

            // Operators
            case '+':
                AddToken(TokenType.PLUS);
                break;

            case '-':
                AddToken(TokenType.MINUS);
                break;

            case '*':
                AddToken(TokenType.STAR);
                break;

            case '%':
                AddToken(TokenType.PERCENT);
                break;

            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;

            case '!':
                AddToken(Match('=') ? TokenType.NOT_EQUAL : TokenType.NOT);
                break;

            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;

            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;

            case '/':
                if (Match('/'))
                {
                    // Comment: discard everything until newline.
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                }
                else
                {
                    AddToken(TokenType.SLASH);
                }
                break;

            // Whitespace
            case ' ':
            case '\t':
            case '\r':
                break;

            case '\n':
                line++;
                break;

            // String
            case '"':
                ScanString();
                break;

            default:
                if (IsDigit(c))
                {
                    ScanNumber();
                }
                else if (IsAlpha(c))
                {
                    ScanIdentifier();
                }
                else
                {
                    Error("Unexpected character.");
                }

                break;
        }
    }

    private void ScanString()
    {
        StringBuilder value = new();

        while (!IsAtEnd())
        {
            char c = Peek();

            if (c == '"')
                break;

            // Collective does not support multiline strings.
            if (c == '\n')
            {
                Error("Unterminated string.");
                return;
            }

            if (c == '\\')
            {
                Advance();

                if (IsAtEnd())
                {
                    Error("Unterminated string.");
                    return;
                }

                char escaped = Advance();

                switch (escaped)
                {
                    case 'n':
                        value.Append('\n');
                        break;

                    case 't':
                        value.Append('\t');
                        break;

                    case '"':
                        value.Append('"');
                        break;

                    case '\\':
                        value.Append('\\');
                        break;

                    default:
                        Error($"Invalid escape sequence '\\{escaped}'.");
                        break;
                }

                continue;
            }

            value.Append(Advance());
        }

        if (IsAtEnd())
        {
            Error("Unterminated string.");
            return;
        }

        // Consume closing quote.
        Advance();

        AddToken(TokenType.STRING, value.ToString());
    }

    private void ScanNumber()
    {
        while (IsDigit(Peek()))
            Advance();

        // Decimal part only if '.' is followed by a digit.
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek()))
                Advance();
        }

        string text = source.Substring(start, current - start);

        if (double.TryParse(text, out double value))
        {
            AddToken(TokenType.NUMBER, value);
        }
        else
        {
            Error("Invalid number.");
        }
    }

    private void ScanIdentifier()
    {
        while (IsAlphaNumeric(Peek()))
            Advance();

        string text = source.Substring(start, current - start);

        if (Keywords.TryGetValue(text, out TokenType type))
        {
            switch (type)
            {
                case TokenType.TRUE:
                    AddToken(TokenType.TRUE, true);
                    break;

                case TokenType.FALSE:
                    AddToken(TokenType.FALSE, false);
                    break;

                case TokenType.NIL:
                    AddToken(TokenType.NIL, null);
                    break;

                default:
                    AddToken(type);
                    break;
            }
        }
        else
        {
            AddToken(TokenType.IDENTIFIER);
        }
    }

    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    private char Advance()
    {
        return source[current++];
    }

    private char Peek()
    {
        if (IsAtEnd())
            return '\0';

        return source[current];
    }

    private char PeekNext()
    {
        if (current + 1 >= source.Length)
            return '\0';

        return source[current + 1];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
            return false;

        if (source[current] != expected)
            return false;

        current++;
        return true;
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object? literal)
    {
        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, line));
    }

    private void Error(string message)
    {
        HadError = true;
        Console.Error.WriteLine($"[line {line}] Error: {message}");
    }

    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private static bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               c == '_';
    }

    private static bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }
}