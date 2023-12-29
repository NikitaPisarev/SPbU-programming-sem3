namespace MyNUnit;

public class TestResult
{
    public string TestName { get; set; }
    public bool IsPassed { get; set; }
    public TimeSpan Duration { get; set; }
    public string ExceptionMessage { get; set; }
    public bool IsIgnored { get; set; }
    public string Reason { get; set; }

    public TestResult(string testName)
    {
        TestName = testName;
        IsPassed = true;
        ExceptionMessage = "";
        IsIgnored = false;
        Reason = "";
    }
}