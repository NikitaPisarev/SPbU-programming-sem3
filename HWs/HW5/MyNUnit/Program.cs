if (args.Length == 0)
{
    Console.WriteLine("Specify the file path.");
    return;
}

var path = args[0];
if (!Directory.Exists(path))
{
    Console.WriteLine($"File not found: {path}");
    return;
}

MyNUnit.TestRunner.RunTestsFromPath(path);