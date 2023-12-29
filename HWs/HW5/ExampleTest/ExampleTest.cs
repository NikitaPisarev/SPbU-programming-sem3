namespace ExampleTest;

using MyNUnit.Attributes;
using static System.Console;

public class Tests
{
    [BeforeClass]
    public static void SetUpClass()
    {
        WriteLine("BeforeClass called!");
    }

    [Before]
    public void SetUp()
    {
        WriteLine("Before called!");
    }

    [Test]
    public void TestMethod1()
    {
        WriteLine("Test Method 1 executed.");
    }

    [Test]
    public void TestMethod2()
    {
        WriteLine("Test Method 2 executed.");
        throw new InvalidOperationException("Test Method 2 failed!");
    }

    [After]
    public void TearDown()
    {
        WriteLine("After called!");
    }

    [AfterClass]
    public static void TearDownClass()
    {
        WriteLine("AfterClass called!");
    }
}

