public class Token
{
    public TokenType Type { get; }
    public string Lexeme { get; }
    public object? Literal { get; }
    public int Line { get; }

    public Token(TokenType type, string lexeme, object? literal, int line)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
    }

    public override string ToString()
    {   // formats the literal value for display in the ToString() method
        return $"Token(type={Type}, lexeme=\"{Lexeme}\", literal={FormatLiteral()}, line={Line})";
    }

    private string FormatLiteral()
    {
        if (Literal == null)
            return "null";

        if (Literal is bool boolean)
            return boolean ? "true" : "false";

        if (Literal is double number)
            return number.ToString();

        if (Literal is string text)
        {

            // Escape special characters in the string literal 
            return text
                .Replace("\\", "\\\\")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t")
                .Replace("\"", "\\\"");
        }

        return Literal.ToString() ?? "null";
    }
}