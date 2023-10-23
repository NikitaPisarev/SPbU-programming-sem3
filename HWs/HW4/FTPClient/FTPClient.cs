namespace FTPClient;

using System.Net.Sockets;

public class FTPClient
{
    private readonly string _address;
    private readonly int _port;

    public FTPClient(string address, int port)
    {
        _address = address;
        _port = port;
    }

    public async Task ListAsync(string path)
    {
        string? response = await SendRequestAsync($"1 {path}");
        Console.WriteLine(response);
    }

    public async Task GetAsync(string path)
    {
        string? response = await SendRequestAsync($"2 {path}");
        Console.WriteLine(response);
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
