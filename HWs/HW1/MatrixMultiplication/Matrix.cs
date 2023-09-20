namespace MatrixMultiplication;

public class Matrix
{
    public int RowsCount { get; }
    public int ColumnsCount { get; }
    private int[,] _data;

    public Matrix(int rowsCount, int columnsCount)
    {
        _data = new int[rowsCount, columnsCount];
        RowsCount = rowsCount;
        ColumnsCount = columnsCount;
    }

    public Matrix(int[,] data)
    {
        RowsCount = data.GetLength(0);
        ColumnsCount = data.GetLength(1);
        _data = data;
    }

    public int this[int row, int col]
    {
        get => _data[row, col];
        set => _data[row, col] = value;
    }

    public static Matrix Multiply(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.ColumnsCount != secondMatrix.RowsCount)
        {
            throw new InvalidOperationException("Number of columns in first matrix must be equal to number of rows in second matrix.");
        }

        var newMatrix = new Matrix(firstMatrix.RowsCount, secondMatrix.ColumnsCount);

        for (int i = 0; i < firstMatrix.RowsCount; ++i)
        {
            for (int j = 0; j < secondMatrix.ColumnsCount; ++j)
            {
                for (int k = 0; k < firstMatrix.ColumnsCount; ++k)
                {
                    newMatrix[i, j] += firstMatrix._data[i, k] * secondMatrix._data[k, j];
                }
            }
        }

        return newMatrix;
    }

    public static Matrix LoadFromFile(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            var matrixLines = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            int numberRows = matrixLines.Count;
            int numberColumns = matrixLines[0].Split(' ').Length;

            var resultMatrix = new Matrix(numberRows, numberColumns);

            for (int i = 0; i < numberRows; i++)
            {
                string[] values = matrixLines[i].Split(' ');
                if (values.Length != numberColumns)
                {
                    throw new InvalidOperationException("Inconsistent number of columns in the matrix.");
                }

                for (int j = 0; j < numberColumns; j++)
                {
                    resultMatrix[i, j] = int.Parse(values[j]);
                }
            }

            return resultMatrix;
        }
    }

    public void WriteInFile(string pathToFile)
    {
        using (var writer = new StreamWriter(pathToFile))
        {
            for (var i = 0; i < RowsCount; ++i)
            {
                for (var j = 0; j < ColumnsCount; ++j)
                {
                    writer.Write($"{_data[i, j]} ");
                }

                if (i != RowsCount - 1)
                {
                    writer.Write("\n");
                }
            }
        }
    }
}