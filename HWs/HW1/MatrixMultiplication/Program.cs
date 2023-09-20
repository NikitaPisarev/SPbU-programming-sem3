using static System.Console;
using MatrixMultiplication;

if (args[0] == "help")
{
    WriteLine("""
        This program is designed to multiply matrices

        Usage:
        Enter the paths to the matrices:
        -------------------------------------------
        dotnet run <File path1> <File path2>
        -------------------------------------------
        The matrix in <File path1> will be multiplied by the matrix in <File path2>
        and the result will be written to the project folder.
        """);
}
else
{
    if (args.Length != 2)
    {
        Write("Argument(s) error. Use \"dotnet help\".");
        return;
    }

    if (!File.Exists(args[0]))
    {
        throw new FileNotFoundException("File not found.");
    }

    if (!File.Exists(args[1]))
    {
        throw new FileNotFoundException("File not found.");
    }

    var firstMatrix = Matrix.LoadFromFile(args[0]);
    var secondMatrix = Matrix.LoadFromFile(args[1]);
    var resultMatrix = Matrix.MultiplyParallel(firstMatrix, secondMatrix);

    resultMatrix.WriteInFile(Directory.GetCurrentDirectory() + "/result.txt");
    WriteLine("Done!");
}