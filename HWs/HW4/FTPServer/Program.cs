namespace FTPServer;

using System.Net.Sockets;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 1 || !int.TryParse(args[0], out int port) || port < 0 || port > 65535)
        {
            Console.WriteLine("Incorrect port value.");
            return;
        }

        var server = new FTPServer(port);
        try
        {
            await server.StartAsync();
        }
        catch (Exception e) when (e is SocketException || e is IOException)
        {
            Console.WriteLine("Client connection error.");
        }
    }
}