namespace LazyTests;

using Lazy;

public class Tests
{
    private static IEnumerable<TestCaseData> LazyImplementations
        => new TestCaseData[]
    {
        new TestCaseData(new SingleLazy<int?>(TestFunctions.RandInt)),
        new TestCaseData(new MultiLazy<int?>(TestFunctions.RandInt)),
    };

    private static IEnumerable<TestCaseData> LazyImplementationsWithNullSupplier()
        => new TestCaseData[]
    {
        new TestCaseData(new SingleLazy<int?>(TestFunctions.NullFunction)),
        new TestCaseData(new MultiLazy<int?>(TestFunctions.NullFunction)),
    };

    private static IEnumerable<TestCaseData> LazyImplementationsWithExceptionSupplier()
        => new TestCaseData[]
    {
        new TestCaseData(new SingleLazy<int>(() => throw new ArgumentException())),
        new TestCaseData(new MultiLazy<int>(() => throw new ArgumentException())),
    };

    [TestCaseSource(nameof(LazyImplementations))]
    [TestCaseSource(nameof(LazyImplementationsWithNullSupplier))]
    public void Get_DefaultFunctionAndNull_CorrectResult(ILazy<int?> lazyInstance)
    {
        var firstCall = lazyInstance.Get();
        var secondCall = lazyInstance.Get();

        Assert.That(firstCall, Is.EqualTo(secondCall));
    }

    [TestCaseSource(nameof(LazyImplementationsWithExceptionSupplier))]
    public void Get_ExceptionFunction_ThrowsException(ILazy<int> lazyInstance)
    {
        Assert.Throws<ArgumentException>(() => lazyInstance.Get());
        Assert.Throws<ArgumentException>(() => lazyInstance.Get());
    }
}

public class TestFunctions
{
    public static int? RandInt()
    {
        var random = new Random();
        return random.Next();
    }

    public static int? NullFunction() => null;
}