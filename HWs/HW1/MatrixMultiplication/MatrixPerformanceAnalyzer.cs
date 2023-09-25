namespace MatrixMultiplication;

using System.Diagnostics;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

/// <summary>
/// Represents the performance results of a matrix operation.
/// </summary>
public class PerformanceResult
{
    /// <summary>
    /// Gets or sets the size of the matrices used in the operation, represented as a string.
    /// </summary>
    public string Size { get; set; }

    /// <summary>
    /// Gets or sets the name of the method used for the matrix operation.
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// Gets or sets the average time taken for the matrix operation.
    /// </summary>
    public double AverageTime { get; set; }

    /// <summary>
    /// Gets or sets the standard deviation of the times taken for the matrix operation.
    /// </summary>
    public double StandardDeviation { get; set; }
}

/// <summary>
/// Provides functionality to analyze the performance of matrix multiplication methods.
/// </summary>
public class MatrixPerformanceAnalyzer
{
    private readonly List<(int rowsA, int colsA, int rowsB, int colsB)> _matrixSizes;
    private readonly int _numberOfRuns;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixPerformanceAnalyzer"/> class.
    /// </summary>
    /// <param name="matrixSizes">A list of tuples representing the sizes of the matrices to be analyzed.</param>
    /// <param name="numberOfRuns">Number of multiplications.</param>
    public MatrixPerformanceAnalyzer(List<(int rowsA, int colsA, int rowsB, int colsB)> matrixSizes, int numberOfRuns)
    {
        _matrixSizes = matrixSizes;
        _numberOfRuns = numberOfRuns;
    }

    /// <summary>
    /// Analyzes the performance of matrix multiplication methods and writes the results to a PNG file.
    /// </summary>
    public void AnalyzePerformance()
    {
        var results = new List<PerformanceResult>();

        foreach (var size in _matrixSizes)
        {
            results.AddRange(AnalyzeMethodPerformance(size, Matrix.Multiply, "Sequential"));
            results.AddRange(AnalyzeMethodPerformance(size, Matrix.MultiplyParallel, "Parallel"));
        }

        WriteResultsToPdf(results, "performance_results.pdf");
    }

    private List<PerformanceResult> AnalyzeMethodPerformance(
        (int rowsA, int colsA, int rowsB, int colsB) sizes,
        Func<Matrix, Matrix, Matrix> multiplyMethod,
        string methodName)
    {
        var times = new List<double>();

        for (int i = 0; i < _numberOfRuns; i++)
        {
            var matrixA = Matrix.CreateRandomMatrix(sizes.rowsA, sizes.colsA);
            var matrixB = Matrix.CreateRandomMatrix(sizes.rowsB, sizes.colsB);

            var stopwatch = Stopwatch.StartNew();
            multiplyMethod(matrixA, matrixB);
            stopwatch.Stop();

            times.Add(stopwatch.Elapsed.TotalMilliseconds);
        }

        return new List<PerformanceResult>
        {
            new PerformanceResult
            {
                Size = $"{sizes.rowsA}x{sizes.colsA} * {sizes.rowsB}x{sizes.colsB}",
                Method = methodName,
                AverageTime = times.Average(),
                StandardDeviation = CalculateStandardDeviation(times),
            }
        };
    }

    private static void WriteResultsToPdf(List<PerformanceResult> results, string filePath)
    {
        PdfDocument document = new PdfDocument();
        document.Info.Title = "Matrix Performance Analysis Results";

        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Verdana", 10, XFontStyle.Regular);

        int yPosition = 50;
        int rowHeight = 20;
        int col1Width = 130;
        int col2Width = 100;
        int col3Width = 130;
        int col4Width = 150;

        DrawCell(gfx, font, "Size", 40, yPosition, col1Width, rowHeight);
        DrawCell(gfx, font, "Method", 40 + col1Width, yPosition, col2Width, rowHeight);
        DrawCell(gfx, font, "Average Time (ms)", 40 + col1Width + col2Width, yPosition, col3Width, rowHeight);
        DrawCell(gfx, font, "Standard Deviation (ms)", 40 + col1Width + col2Width + col3Width, yPosition, col4Width, rowHeight);

        yPosition += rowHeight;

        foreach (var result in results)
        {
            DrawCell(gfx, font, result.Size, 40, yPosition, col1Width, rowHeight);
            DrawCell(gfx, font, result.Method, 40 + col1Width, yPosition, col2Width, rowHeight);
            DrawCell(gfx, font, result.AverageTime.ToString("F2"), 40 + col1Width + col2Width, yPosition, col3Width, rowHeight);
            DrawCell(gfx, font, result.StandardDeviation.ToString("F2"), 40 + col1Width + col2Width + col3Width, yPosition, col4Width, rowHeight);

            yPosition += rowHeight;
        }

        document.Save(filePath);
    }

    private static void DrawCell(XGraphics gfx, XFont font, string text, int x, int y, int width, int height)
    {
        gfx.DrawRectangle(XPens.Black, x, y, width, height);
        gfx.DrawString(text, font, XBrushes.Black, new XRect(x, y, width, height), XStringFormats.Center);
    }

    private static double CalculateStandardDeviation(List<double> values)
    {
        int count = values.Count;
        double sum = 0;
        double sumOfSquares = 0;

        for (int i = 0; i < count; i++)
        {
            double value = values[i];
            sum += value;
            sumOfSquares += value * value;
        }

        double variance = (sumOfSquares - (sum * sum / count)) / count;
        return Math.Sqrt(variance);
    }
}