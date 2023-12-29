namespace FTPServer;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Represents a FTP server that can handle requests from FTP clients.
/// </summary>
public class FTPServer
{
    private readonly TcpListener _listener;
    private readonly int _port;
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FTPServer"/> class.
    /// </summary>
    /// <param name="port">The port to listen on.</param>
    public FTPServer(int port)
    {
        if (port < 0 || port > 65535)
        {
            throw new ArgumentException("Incorrect port value.");
        }

        _port = port;
        _listener = new TcpListener(IPAddress.Any, _port);
    }

    /// <summary>
    /// Starts the FTP server.
    /// </summary>
    public async Task StartAsync()
    {
        _listener.Start();

        while (!_cts.Token.IsCancellationRequested)
        {
            var client = await _listener.AcceptTcpClientAsync();
            Task.Run(async () =>
            {
                await using var stream = client.GetStream();
                using var reader = new StreamReader(stream);
                using var writer = new StreamWriter(stream);

                string? request;
                while ((request = await reader.ReadLineAsync()) != null)
                {
                    if (request.StartsWith("1 "))
                    {
                        await HandleListAsync(request[2..], writer);
                    }

                    if (request.StartsWith("2 "))
                    {
                        await HandleGetAsync(request[2..], writer);
                    }
                }
                client.Close();
            });
        }
    }

    /// <summary>
    /// Stop the FTP server.
    /// </summary> <summary>
    public void Stop()
    {
        _cts.Cancel();
        _listener.Stop();
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
        Array.Sort(entries);

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