namespace LazyTests;

using Lazy;

public class Tests
{
    private readonly ManualResetEvent _threadsHandler = new(false);

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

    [Test]
    public void MultiGet_DefaultFunction_CorrectResultAndCall()
    {
        int counter = 0;
        var handler = new ManualResetEvent(false);

        var lazyInstance = new MultiLazy<int?>(() =>
        {
            ++counter;
            return 0;
        });

        int threadCount = Environment.ProcessorCount;
        var results = new int?[threadCount];
        Thread[] threads = new Thread[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            int localI = i;
            threads[i] = new Thread(() =>
            {
                handler.WaitOne();
                results[localI] = lazyInstance.Get();
            });
            threads[i].Start();
        }

        handler.Set();

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        foreach (int? result in results)
        {
            Assert.That(result, Is.EqualTo(0));
        }

        Assert.That(counter, Is.EqualTo(1));
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