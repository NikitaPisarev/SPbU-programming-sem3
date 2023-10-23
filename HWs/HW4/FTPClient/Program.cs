namespace FTPClient;

using System.Net.Sockets;
using System.Net.WebSockets;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 1 || !int.TryParse(args[0], out int port) || port < 0 || port > 65535)
        {
            Console.WriteLine("Incorrect port value.");
            return;
        }

        var client = new FTPClient("localhost", port);

        try
        {
            Console.WriteLine(await client.ListAsync("../FTPTests/Files"));
            Console.WriteLine(await client.GetAsync("../FTPTests/Files/file1.txt"));
        }
        catch (Exception e) when (e is SocketException || e is IOException)
        {
            Console.WriteLine("Connection to server error.");
        }
    }
}
