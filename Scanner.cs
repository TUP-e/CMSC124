using System.Globalization;
using System.Text;

public class Scanner
{
    private readonly string source;
    private readonly List<Token> tokens = new();

    private int start = 0;
    private int current = 0;
    private int line = 1;

    // Tracks line start and token start positions for accurate error columns.
    private int lineStart = 0;
    private int startColumn = 1;

    // HadError used to be a plain bool, now we count the errors too
    public int ErrorCount { get; private set; }
    public bool HadError => ErrorCount > 0;

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
            startColumn = current - lineStart + 1;
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
                // Handles decimal numbers starting with a dot or treats it as a member-access DOT.
                if (IsDigit(Peek()))
                    ScanNumber();
                else
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
                lineStart = current;
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
                    Error($"Unexpected character '{c}'.");
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
                int backslashColumn = current - lineStart + 1;
                Advance();

                // Treats trailing backslashes at line ends as unterminated strings.
                if (IsAtEnd() || Peek() == '\n')
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
                        Error($"Invalid escape sequence '\\{escaped}'.", backslashColumn);
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
        // if we came from a leading dot (".5") the dot is already consumed
        bool startsWithDot = source[start] == '.';

        ConsumeDigits();

        // Parses the decimal fraction part if the dot is followed by a digit.
        if (!startsWithDot && Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            ConsumeDigits();
        }

        // Reports numbers incorrectly attached to letters or underscores as invalid tokens.
        if (IsAlphaNumeric(Peek()))
        {
            while (IsAlphaNumeric(Peek()))
                Advance();

            string bad = source.Substring(start, current - start);
            Error($"Invalid number or identifier '{bad}'. Identifiers cannot start with a digit.");
            return;
        }

        // the lexeme keeps the underscores ("1_000"), the value does not (1000)
        string text = source.Substring(start, current - start).Replace("_", "");

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
            && !double.IsInfinity(value))
        {
            AddToken(TokenType.NUMBER, value);
        }
        else
        {
            Error("Invalid number.");
        }
    }

    // Consumes valid numeric digits supporting internal underscores like Python.
    private void ConsumeDigits()
    {
        while (true)
        {
            if (IsDigit(Peek()))
                Advance();
            else if (Peek() == '_' && IsDigit(PeekNext()))
                Advance();
            else
                break;
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

    // Reports error details and token trace to stderr while allowing scanning to continue.
    private void Error(string message, int? column = null)
    {
        ErrorCount++;
        int col = column ?? startColumn;

        Console.Error.WriteLine($"[line {line}, col {col}] Error: {message}");

        int end = source.IndexOf('\n', lineStart);
        if (end < 0)
            end = source.Length;
        string text = source.Substring(lineStart, end - lineStart).TrimEnd('\r').Replace('\t', ' ');
        string gutter = line.ToString();
        Console.Error.WriteLine($"  {gutter} | {text}");
        Console.Error.WriteLine($"  {new string(' ', gutter.Length)} | {new string(' ', Math.Max(col - 1, 0))}^");

        PrintTrace();
    }

    private void PrintTrace()
    {
        // tokens only has what we managed to scan up to this point
        Console.Error.WriteLine(
            $"  Trace: {line - 1} earlier line(s) read, {tokens.Count} token(s) scanned before this error:");

        // only show the last few lines so a big file doesn't flood the screen
        const int maxLinesShown = 5;
        var earlier = tokens.Where(t => t.Line < line).GroupBy(t => t.Line).ToList();
        int hidden = Math.Max(0, earlier.Count - maxLinesShown);

        if (hidden > 0)
            Console.Error.WriteLine($"    ... ({hidden} earlier line(s) with tokens not shown)");

        foreach (var group in earlier.Skip(hidden))
            Console.Error.WriteLine($"    line {group.Key}: {Summarize(group)}");

        var thisLine = tokens.Where(t => t.Line == line).ToList();
        string soFar = thisLine.Count > 0 ? Summarize(thisLine) + " " : "";
        Console.Error.WriteLine($"    line {line}: {soFar}<-- error here");
    }

    private static string Summarize(IEnumerable<Token> lineTokens)
    {
        return string.Join(" ", lineTokens.Select(t => $"{t.Type}({t.Lexeme})"));
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