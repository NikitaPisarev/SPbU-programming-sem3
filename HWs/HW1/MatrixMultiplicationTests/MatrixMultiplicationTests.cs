namespace MatrixMultiplicationTests;

using MatrixMultiplication;

public class MatrixMultiplicationTests
{
    private static IEnumerable<TestCaseData> Files
        => new TestCaseData[]
    {
        new TestCaseData("../../../Files/Empty.txt"),
        new TestCaseData("../../../Files/IncorrectMatrix.txt"),
        new TestCaseData("../../../Files/MatrixWithIncorrectValues.txt"),
    };

    [TestCaseSource(nameof(Files))]
    public void LoadFromFile_IncorrectFile_ArgumentExceptionReturned(string filePath)
        => Assert.Throws<ArgumentException>(() => Matrix.LoadFromFile(filePath));
}