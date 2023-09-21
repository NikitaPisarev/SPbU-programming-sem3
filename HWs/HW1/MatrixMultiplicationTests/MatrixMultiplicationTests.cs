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

    private static IEnumerable<TestCaseData> CorrectMatrices
        => new TestCaseData[]
    {
        new TestCaseData(
            new Matrix(new int[,]
            {
                {5, -2, 0},
                {11, 3, 3},
                {0, 56, 2}
            }),
            new Matrix(new int[,]
            {
                {4, 9, 2},
                {1, 1, 1},
                {-5, -2, -1}
            }),
            new Matrix(new int[,]
            {
                {18, 43, 8},
                {32, 96, 22},
                {46, 52, 54}
            })
        ),
        new TestCaseData(
            new Matrix(new int[,]
            {
                {5, -2, 0, 2},
                {11, 3, 3, 10},
                {0, 56, 2, -7}
            }),
            new Matrix(new int[,]
            {
                {4, 9},
                {1, 1},
                {-5, -2},
                {2, 2}
            }),
            new Matrix(new int[,]
            {
                {22, 47},
                {52, 116},
                {32, 38}
            })
        ),
    };

    private static IEnumerable<TestCaseData> IncorrectMatrices
        => new TestCaseData[]
    {
        new TestCaseData(
            new Matrix(new int[,]
            {
                {5, -2, 0, 2},
                {11, 3, 3, 10},
                {0, 56, 2, -7}
            }),
            new Matrix(new int[,]
            {
                {4, 9, 2},
                {1, 1, 1},
                {-5, -2, -1}
            })
        ),
    };

    [TestCaseSource(nameof(Files))]
    public void LoadFromFile_IncorrectFile_ArgumentExceptionReturned(string filePath)
        => Assert.Throws<ArgumentException>(() => Matrix.LoadFromFile(filePath));

    [TestCaseSource(nameof(CorrectMatrices))]
    public void Multiply_CorrectMatrices_CorrectResultMatrix(Matrix firstMatrix, Matrix secondMatrix, Matrix expected)
        => Assert.That(Matrix.Multiply(firstMatrix, secondMatrix).IsEqual(expected), Is.True);

    [TestCaseSource(nameof(CorrectMatrices))]
    public void MultiplyParallel_CorrectMatrices_CorrectResultMatrix(Matrix firstMatrix, Matrix secondMatrix, Matrix expected)
        => Assert.That(Matrix.MultiplyParallel(firstMatrix, secondMatrix).IsEqual(expected), Is.True);

    [TestCaseSource(nameof(IncorrectMatrices))]
    public void Multiply_CorrectMatrices_InvalidOperationExceptionReturned(Matrix firstMatrix, Matrix secondMatrix)
        => Assert.Throws<InvalidOperationException>(() => Matrix.Multiply(firstMatrix, secondMatrix));

    [TestCaseSource(nameof(IncorrectMatrices))]
    public void MultiplyParallel_CorrectMatrices_InvalidOperationExceptionReturned(Matrix firstMatrix, Matrix secondMatrix)
        => Assert.Throws<InvalidOperationException>(() => Matrix.MultiplyParallel(firstMatrix, secondMatrix));
}