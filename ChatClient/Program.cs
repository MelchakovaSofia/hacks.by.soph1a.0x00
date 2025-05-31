using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

class ChatClient
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8; // Чтобы консоль печатала UTF-8

        Console.Write("Введіть ім’я: ");
        string name = Console.ReadLine();

        using TcpClient client = new TcpClient("127.0.0.1", 5555);
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        writer.WriteLine(name);

        while (true)
        {
            string message = Console.ReadLine();
            if (message.ToLower() == "exit")
                break;

            writer.WriteLine(message);
            string response = reader.ReadLine();
            if (response != null)
                Console.WriteLine(response);
        }
    }
}
