namespace Chat;

using System.Net.Sockets;

/// <summary>
/// Represents a client that can chat with the server.
/// </summary
public class Client
{
    private readonly string _ip;
    private readonly int _port;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="address">The IP address of the server.</param>
    /// <param name="port">The port of the server.</param>
    public Client(string ip, int port)
    {
        _ip = ip;
        _port = port;
    }

    /// <summary>
    /// Establishing a connection to the server.
    /// </summary>
    public async Task StartAsync()
    {
        using var client = new TcpClient(_ip, _port);
        Console.WriteLine("Connected to server.");

        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        await Task.WhenAny(
            ReadAsync(reader),
            WriteAsync(writer));

        client.Close();
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
            await writer.WriteLineAsync("Client: " + line);
        }
        await writer.WriteLineAsync(line);
    }
}
