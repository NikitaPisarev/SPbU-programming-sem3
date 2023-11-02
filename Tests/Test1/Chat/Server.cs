namespace Chat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Represents a server that can chat with the client.
/// </summary
public class Server
{
    private readonly int _port;

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="port">The port to listen on.</param>
    public Server(int port)
    {
        _port = port;
    }

    /// <summary>
    /// Listen on the port, wait for connection to the client to be established.
    /// </summary>
    public async Task StartAsync()
    {
        var listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();

        Console.WriteLine($"Server started on port {_port}.");
        var client = await listener.AcceptTcpClientAsync();
        Console.WriteLine("Client connected.");

        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        await Task.WhenAny(
            ReadAsync(reader),
            WriteAsync(writer));

        client.Close();
        listener.Stop();
    }

    private static async Task ReadAsync(StreamReader reader)
    {
        string? line;
        while ((line = await reader.ReadLineAsync()) != "exit")
        {
            Console.WriteLine(line);
        }
    }

    private static async Task WriteAsync(StreamWriter writer)
    {
        string? line;
        while ((line = Console.ReadLine()) != "exit")
        {
            await writer.WriteLineAsync("Server: " + line);
        }
        await writer.WriteLineAsync("exit");
    }
}
