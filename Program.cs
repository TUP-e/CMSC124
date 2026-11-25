if (args.Length == 0)
{
    RunRepl();
    return;
}

if (args[0] == "--tokenize")
{
    if (args.Length != 2)
    {
        Console.Error.WriteLine("Usage: ./run --tokenize <file>");
        Environment.ExitCode = 65;
        return;
    }

    try
    {
        string source = File.ReadAllText(args[1]);

        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }

        if (scanner.HadError)
        {
            Console.Error.WriteLine(
                $"Scanning reached the end of the file (line {tokens[^1].Line}): " +
                $"{tokens.Count - 1} valid token(s), {scanner.ErrorCount} error(s).");

            Environment.ExitCode = 65;
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Environment.ExitCode = 65;
    }

    return;
}

if (args[0] == "--parse")
{
    if (args.Length != 2)
    {
        Console.Error.WriteLine("Usage: ./run --parse <file>");
        Environment.ExitCode = 65;
        return;
    }

    try
    {
        string source = File.ReadAllText(args[1]);
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        if (scanner.HadError)
        {
            Environment.ExitCode = 65;
            return;
        }

        var parser = new Parser(tokens, source);
        var expressions = parser.Parse();
        if (parser.HadError)
        {
            Environment.ExitCode = 65;
            return;
        }

        foreach (Expr expression in expressions)
            Console.WriteLine(AstPrinter.Print(expression));
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Environment.ExitCode = 65;
    }

    return;
}

// Program execution arrives in Lab 4.
Console.WriteLine("CMSC124 DUO Tope & Josef");

static void RunRepl()
{
    while (true)
    {
        Console.Write("> ");

        string? line = Console.ReadLine();

        if (line == null)
            break;

        var scanner = new Scanner(line);
        var tokens = scanner.ScanTokens();

        if (scanner.HadError)
            continue;

        var parser = new Parser(tokens, line);
        var expressions = parser.Parse();
        if (!parser.HadError)
            foreach (Expr expression in expressions)
                Console.WriteLine(AstPrinter.Print(expression));
    }
}
