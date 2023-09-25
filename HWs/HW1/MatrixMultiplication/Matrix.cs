namespace MatrixMultiplication;

/// <summary>
/// Represents a matrix and provides methods for multiplying matrices.
/// </summary>
public class Matrix
{
    /// <summary>
    /// Gets the number of rows in the matrix.
    /// </summary>
    public int RowsCount { get; }

    /// <summary>
    /// Gets the number of columns in the matrix.
    /// </summary>
    public int ColumnsCount { get; }

    private int[,] _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class with the specified number of rows and columns.
    /// </summary>
    /// <param name="rowsCount">The number of rows in the matrix.</param>
    /// <param name="columnsCount">The number of columns in the matrix.</param>
    public Matrix(int rowsCount, int columnsCount)
    {
        _data = new int[rowsCount, columnsCount];
        RowsCount = rowsCount;
        ColumnsCount = columnsCount;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class with the specified two-dimensional array.
    /// </summary>
    /// <param name="data">The two-dimensional array to initialize the matrix with.</param>
    public Matrix(int[,] data)
    {
        RowsCount = data.GetLength(0);
        ColumnsCount = data.GetLength(1);
        _data = data;
    }

    /// <summary>
    /// Matrix indexer.
    /// </summary>
    public int this[int row, int col]
    {
        get => _data[row, col];
        set => _data[row, col] = value;
    }

    /// <summary>
    /// Multiplies the specified matrices.
    /// </summary>
    /// <param name="firstMatrix">The first matrix to multiply.</param>
    /// <param name="secondMatrix">The second matrix to multiply.</param>
    /// <returns>The result of multiplying the first matrix by the second matrix.</returns>
    /// <exception cref="InvalidOperationException"></exception>
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
                newMatrix[i, j] = Enumerable.Range(0, firstMatrix.ColumnsCount).Sum(k => firstMatrix._data[i, k] * secondMatrix._data[k, j]);
            }
        }

        return newMatrix;
    }

    /// <summary>
    /// Multiplies the specified matrices using parallel processing.
    /// </summary>
    /// <param name="firstMatrix">The first matrix to multiply.</param>
    /// <param name="secondMatrix">The second matrix to multiply.</param>
    /// <returns>The result of multiplying the first matrix by the second matrix.</returns>
    /// <exception cref="InvalidOperationException"></exception>
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
                result[i, j] = Enumerable.Range(0, firstMatrix.ColumnsCount).Sum(k => firstMatrix._data[i, k] * secondMatrix._data[k, j]);
            }
        }
    }

    /// <summary>
    /// Compares this matrix to the specified matrix for equality.
    /// </summary>
    /// <param name="otherMatrix">The matrix to compare with this matrix.</param>
    /// <returns>true if the matrices are equal; otherwise, false.</returns>
    public bool IsEqual(Matrix otherMatrix)
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

    /// <summary>
    /// Loads a matrix from the specified file.
    /// </summary>
    /// <param name="filePath">The path to the file containing the matrix data.</param>
    /// <returns>The matrix loaded from the file.</returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
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

    /// <summary>
    /// Writes this matrix to the specified file.
    /// </summary>
    /// <param name="pathToFile">The path to the file where this matrix will be written.</param>
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

    /// <summary>
    /// Creates a Matrix object with the specified number of rows and columns and fills it with random values.
    /// </summary>
    /// <param name="rowsCount">The number of rows in the matrix. Must be greater than 0.</param>
    /// <param name="columnsCount">The number of columns in the matrix. Must be greater than 0.</param>
    /// <param name="minValue">Lower limit of values. Default is -50.</param>
    /// <param name="maxValue">Upper limit of values. Default is 50.</param>
    /// <returns>A Matrix filled with random values between minValue and maxValue.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Matrix CreateRandomMatrix(int rowsCount, int columnsCount, int minValue = -50, int maxValue = 50)
    {
        if (rowsCount <= 0 || columnsCount <= 0)
        {
            throw new ArgumentException("Number of rows and columns must be greater than 0.");
        }

        var random = new Random();
        var data = new Matrix(rowsCount, columnsCount);
        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                data[i, j] = random.Next(minValue, maxValue);
            }
        }

        return data;
    }
}