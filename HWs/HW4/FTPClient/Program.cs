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

        while (true)
        {
            Console.Write("Enter the command (list [path] / get [path] / exit): ");
            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                continue;
            }

            var parts = input.Split(' ', 2);
            var command = parts[0].ToLower();

            try
            {
                switch (command)
                {
                    case "list" when parts.Length == 2:
                        Console.WriteLine(await client.ListAsync(parts[1]));
                        break;
                    case "get" when parts.Length == 2:
                        Console.WriteLine(await client.GetAsync(parts[1]));
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Incorrect command.");
                        break;
                }
            }
            catch (Exception e) when (e is SocketException || e is IOException)
            {
                Console.WriteLine("Connection to server error.");
            }
        }
    }
}