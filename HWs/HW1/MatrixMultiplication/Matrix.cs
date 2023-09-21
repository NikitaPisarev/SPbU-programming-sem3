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
    public static Matrix MultiplyParallel(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.ColumnsCount != secondMatrix.RowsCount)
        {
            throw new InvalidOperationException("Number of columns in first matrix must be equal to number of rows in second matrix.");
        }

        var resultMatrix = new Matrix(firstMatrix.RowsCount, secondMatrix.ColumnsCount);
        int numberOfCores = Environment.ProcessorCount;
        var threads = new Thread[numberOfCores];

        int chunkSize = firstMatrix.RowsCount / numberOfCores;
        for (int i = 0; i < numberOfCores; i++)
        {
            int startRow = i * chunkSize;
            int endRow = (i == numberOfCores - 1) ? firstMatrix.RowsCount : startRow + chunkSize;

            threads[i] = new Thread(() => MultiplyRowRange(startRow, endRow, firstMatrix, secondMatrix, resultMatrix));
            threads[i].Start();
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        return resultMatrix;
    }

    private static void MultiplyRowRange(int startRow, int endRow, Matrix firstMatrix, Matrix secondMatrix, Matrix result)
    {
        for (int i = startRow; i < endRow; i++)
        {
            for (int j = 0; j < secondMatrix.ColumnsCount; j++)
            {
                for (int k = 0; k < firstMatrix.ColumnsCount; k++)
                {
                    result[i, j] += firstMatrix[i, k] * secondMatrix[k, j];
                }
            }
        }
    }

    public bool IsEqual(Matrix? otherMatrix)
    {
        if (otherMatrix == null)
            return false;

        if (RowsCount != otherMatrix.RowsCount || ColumnsCount != otherMatrix.ColumnsCount)
            return false;

        for (int i = 0; i < RowsCount; i++)
        {
            for (int j = 0; j < ColumnsCount; j++)
            {
                if (_data[i, j] != otherMatrix[i, j])
                    return false;
            }
        }

        return true;
    }

    public static Matrix LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found.");
        }

        using (StreamReader reader = new StreamReader(filePath))
        {
            var matrixLines = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            int numberRows = matrixLines.Count;

            if (numberRows == 0)
            {
                throw new ArgumentException("File is empty.");
            }

            int numberColumns = matrixLines[0].Split(' ').Length;
            var resultMatrix = new Matrix(numberRows, numberColumns);

            for (int i = 0; i < numberRows; i++)
            {
                string[] values = matrixLines[i].Split(' ');
                if (values.Length != numberColumns)
                {
                    throw new ArgumentException("Inconsistent number of columns in the matrix.");
                }

                for (int j = 0; j < numberColumns; j++)
                {
                    int value = 0;
                    if (!int.TryParse(values[j], out value))
                    {
                        throw new ArgumentException("Incorrect values in the matrix.");
                    }
                    resultMatrix[i, j] = value;
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