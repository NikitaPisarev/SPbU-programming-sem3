using static System.Console;
using System.Text;
using MatrixMultiplication;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

if (args[0] == "--help" || args[0] == "-h")
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

        Options:
        --a, -analyze    Performance Verification. The result is saved in the file "performance_results.png".
        --h, -help       Shows this help message.

        """);
}
else if (args[0] == "--a" || args[0] == "-analyze")
{
    var matrixSizes = new List<(int rowsA, int colsA, int rowsB, int colsB)>
        {
            (100, 200, 200, 100),
            (500, 300, 300, 500),
            (1000, 1000, 1000, 1000)
        };
    int numberOfRuns = 20;

    var analyzer = new MatrixPerformanceAnalyzer(matrixSizes, numberOfRuns);
    analyzer.AnalyzePerformance();
}
else
{
    if (args.Length != 2)
    {
        Write("Argument(s) error. Use \"dotnet run help\".");
        return;
    }

    try
    {
        var firstMatrix = Matrix.LoadFromFile(args[0]);
        var secondMatrix = Matrix.LoadFromFile(args[1]);
        var resultMatrix = Matrix.MultiplyParallel(firstMatrix, secondMatrix);

        resultMatrix.WriteInFile(Directory.GetCurrentDirectory() + "/result.txt");
        WriteLine("Done! The result of the multiplication is in the file \"result.txt\".");
    }
    catch (Exception e) when (e is ArgumentException || e is InvalidOperationException)
    {
        Write(e.Message);
        return;
    }
}