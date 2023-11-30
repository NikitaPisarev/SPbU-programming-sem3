using System.Diagnostics;
using static MD5.ChecksumCalculator;

static void PrintHelp()
{
    Console.WriteLine("""
        This programme is designed to calculate the hash of a directory.

        Usage:
        Enter the path of the directory:
        -------------------------------------------
        dotnet run <path>
        -------------------------------------------
        """);
}

if (args.Length != 1)
{
    PrintHelp();
    return;
}

if (!Directory.Exists(args[0]))
{
    Console.WriteLine($"The directory(file) does not exist: {args[0]}");
    return;
}

var stopwatchSingleThread = Stopwatch.StartNew();
string checksumSingleThread = MD5.ChecksumCalculator.CalculateDirectoryChecksumSingleThread(args[0]);
stopwatchSingleThread.Stop();
Console.WriteLine($"Single-thread check-sum: {checksumSingleThread}, Time: {stopwatchSingleThread.ElapsedMilliseconds} ms");

var stopwatchMultiThread = Stopwatch.StartNew();
string checksumMultiThread = MD5.ChecksumCalculator.CalculateDirectoryChecksumMultiThread(args[0]);
stopwatchMultiThread.Stop();
Console.WriteLine($"Multi-thread chek-sum:  {checksumMultiThread}, Time: {stopwatchMultiThread.ElapsedMilliseconds} ms");