namespace MD5Tests;

using static MD5.ChecksumCalculator;

public class Tests
{
    private static IEnumerable<TestCaseData> Directories
        => new TestCaseData[]
    {
        new("../../../Directories"),
        new("../../../Directories/dir1"),
        new("../../../Directories/dir2"),
    };

    [TestCaseSource(nameof(Directories))]
    public void CalculateChecksum_SingleThreadAndMultiThreadMethods_Equal(string path)
    {
        var singleThreadHash = MD5.ChecksumCalculator.CalculateDirectoryChecksumSingleThread(path);
        var multiThreadHash = MD5.ChecksumCalculator.CalculateDirectoryChecksumMultiThread(path);

        Assert.That(singleThreadHash, Is.EqualTo(multiThreadHash));
    }
}