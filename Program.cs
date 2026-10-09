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

        if (scanner.HadError)
        {
            Console.Error.WriteLine(
                $"Scanning reached the end of the file (line {tokens[^1].Line}): " +
                $"{tokens.Count - 1} valid token(s), {scanner.ErrorCount} error(s).");

            Environment.ExitCode = 65;
            return;
        }

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Environment.ExitCode = 65;
    }

    return;
}

// Keep Lab 0 behavior for now
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

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}