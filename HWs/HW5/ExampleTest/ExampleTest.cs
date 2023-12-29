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

    [Test(Expected = typeof(InvalidOperationException))]
    public void TestExpectingException()
    {
        throw new InvalidOperationException("Expected exception thrown.");
    }

    [Test(Ignore = "This test is ignored")]
    public void IgnoredTest()
    {
        Console.WriteLine("Ignored Test executed.");
    }

    [Test]
    public void TestWithArguments(int number)
    {
        Console.WriteLine("Test With Arguments executed.");
    }

    [Test]
    public string TestWithReturnValue()
    {
        Console.WriteLine("Test With Return Value executd.");
        return "Hello, World!";
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

