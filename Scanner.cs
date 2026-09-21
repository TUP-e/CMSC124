using System.Text;

public class Scanner
{
    private readonly string source;
    private readonly List<Token> tokens = new();

    private int start = 0;
    private int current = 0;
    private int line = 1;

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

            case ' ':
            case '\t':
            case '\r':
                break;

            case '\n':
                line++;
                break;

            default:
                if (IsDigit(c))
                {
                    ScanNumber();
                }
                break;
        }
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

    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private void AddToken(TokenType type)
    {
        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, null, line));
    }

    private void AddToken(TokenType type, object? literal)
    {
        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, line));
    }
}
