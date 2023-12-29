namespace MyNUnit;

public class TestResult
{
    public string TestName { get; set; }
    public bool IsPassed { get; set; }
    public TimeSpan Duration { get; set; }
    public string ExceptionMessage { get; set; }

    public TestResult(string testName)
    {
        TestName = testName;
        IsPassed = true;
        ExceptionMessage = "";
    }
}