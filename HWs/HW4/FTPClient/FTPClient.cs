namespace FTPClient;

using System.Net.Sockets;

/// <summary>
/// Represents a FTP client that can send requests to a FTP server.
/// </summary>
public class FTPClient
{
    private readonly string _address;
    private readonly int _port;

    /// <summary>
    /// Initializes a new instance of the <see cref="FTPClient"/> class.
    /// </summary>
    /// <param name="address">The IP address of the FTP server.</param>
    /// <param name="port">The port of the FTP server.</param>
    public FTPClient(string address, int port)
    {
        _address = address;
        _port = port;
    }

    /// <summary>
    /// Sends a request to list files in a directory on the server
    /// </summary>
    /// <param name="path">The path to file.</param>
    public async Task<string?> ListAsync(string path)
    {
        string? response = await SendRequestAsync($"1 {path}");
        return response;
    }

    /// <summary>
    /// Sends a request to download a file from the server.
    /// </summary>
    /// <param name="path">The path to file.</param>
    public async Task<string?> GetAsync(string path)
    {
        string? response = await SendRequestAsync($"2 {path}");
        return response;
    }

    private async Task<string?> SendRequestAsync(string request)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(_address, _port);
        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream);
        using var reader = new StreamReader(stream);
        await writer.WriteLineAsync(request);
        await writer.FlushAsync();
        return await reader.ReadLineAsync();
    }
}
