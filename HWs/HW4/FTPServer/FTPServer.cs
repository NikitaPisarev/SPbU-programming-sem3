namespace FTPServer;

using System.Net;
using System.Net.Sockets;

public class FTPServer
{
    private readonly TcpListener _listener;
    private readonly int _port;

    public FTPServer(int port)
    {
        _port = port;
        _listener = new TcpListener(IPAddress.Any, _port);
    }

    public async Task StartAsync()
    {
        _listener.Start();

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            await using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);

            string? request = await reader.ReadLineAsync();
            if (request is null)
            {
                continue;
            }

            if (request.StartsWith("1 "))
            {
                await HandleListAsync(request[2..], writer);
            }

            if (request.StartsWith("2 "))
            {
                await HandleGetAsync(request[2..], writer);
            }
        }
    }

    private static async Task HandleListAsync(string path, StreamWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            await writer.FlushAsync();
            return;
        }

        var entries = Directory.GetFileSystemEntries(path);
        await writer.WriteAsync($"{entries.Length}");
        foreach (var entry in entries)
        {
            var isDirectory = Directory.Exists(entry);
            await writer.WriteAsync($" {Path.GetFileName(entry)} {isDirectory}");
        }
        await writer.WriteLineAsync();
        await writer.FlushAsync();
    }

    private static async Task HandleGetAsync(string path, StreamWriter writer)
    {
        if (!File.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            await writer.FlushAsync();
            return;
        }

        var content = await File.ReadAllBytesAsync(path);
        string contentHex = BitConverter.ToString(content).Replace("-", "");
        await writer.WriteLineAsync($"{content.Length} {contentHex}");
        await writer.FlushAsync();
    }
}