namespace MD5;

using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

public static class ChecksumCalculator
{
    public static string CalculateDirectoryChecksumSingleThread(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new ArgumentException($"The directory(file) does not exist: {directoryPath}");
        }

        using var md5 = MD5.Create();
        return CalculateChecksumSingle(directoryPath, md5);
    }

    public static string CalculateDirectoryChecksumMultiThread(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new ArgumentException($"The directory(file) does not exist: {directoryPath}");
        }

        using var md5 = MD5.Create();
        return CalculateChecksumMulti(directoryPath, md5);
    }

    private static string CalculateChecksumMulti(string path, MD5 md5)
    {
        var combinedHashes = new StringBuilder();
        combinedHashes.Append(GetDirectoryNameHash(path, md5));

        string[] files = Directory.GetFiles(path);

        var fileHashes = new ConcurrentDictionary<string, string>();
        Parallel.ForEach(files, file =>
        {
            using var md5File = MD5.Create();
            fileHashes[file] = GetFileHash(file, md5File);
        });

        foreach (var file in files)
        {
            combinedHashes.Append(fileHashes[file]);
        }

        string[] dirs = Directory.GetDirectories(path);
        foreach (var dir in dirs)
        {
            combinedHashes.Append(CalculateChecksumMulti(dir, md5));
        }

        return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(combinedHashes.ToString())));
    }

    private static string CalculateChecksumSingle(string path, MD5 md5)
    {
        StringBuilder hash = new();
        hash.Append(GetDirectoryNameHash(path, md5));

        foreach (var file in Directory.GetFiles(path))
        {
            hash.Append(GetFileHash(file, md5));
        }

        foreach (var dir in Directory.GetDirectories(path))
        {
            hash.Append(CalculateChecksumSingle(dir, md5));
        }

        return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(hash.ToString())));
    }

    private static string GetFileHash(string filePath, MD5 md5)
    {
        using var stream = File.OpenRead(filePath);
        return BitConverter.ToString(md5.ComputeHash(stream));
    }

    private static string GetDirectoryNameHash(string dirPath, MD5 md5)
    {
        return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(new DirectoryInfo(dirPath).Name)));
    }
}
