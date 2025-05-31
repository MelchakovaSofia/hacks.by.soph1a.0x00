using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ClientInfo
{
    public TcpClient Client;
    public string Name;
}

class Program
{
    static List<ClientInfo> clients = new();
    static object locker = new();

    static void Main()
    {
        TcpListener server = new(IPAddress.Any, 5555);
        server.Start();
        Console.WriteLine("🚀 Сервер запущено на порту 5555");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Thread t = new(() => HandleClient(client));
            t.Start();
        }
    }

    static void HandleClient(TcpClient client)
    {
        string name = "";
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        stream.Write(Encoding.UTF8.GetBytes("Введіть ім’я: "));
        int len = stream.Read(buffer, 0, buffer.Length);
        name = Encoding.UTF8.GetString(buffer, 0, len).Trim();

        var info = new ClientInfo { Client = client, Name = name };
        lock (locker) clients.Add(info);

        Broadcast($"🟢 {name} приєднався до чату", info);

        try
        {
            while (true)
            {
                len = stream.Read(buffer, 0, buffer.Length);
                if (len == 0) break;
                string msg = Encoding.UTF8.GetString(buffer, 0, len).Trim();

                if (msg.StartsWith("/ім’я "))
                {
                    string newName = msg.Substring(6).Trim();
                    string oldName = info.Name;
                    info.Name = newName;
                    Broadcast($"🔁 {oldName} змінив ім’я на {newName}", info);
                }
                else
                {
                    Broadcast($"💬 {info.Name}: {msg}", info);
                }
            }
        }
        catch { }
        finally
        {
            lock (locker) clients.Remove(info);
            Broadcast($"🔴 {info.Name} вийшов з чату", info);
            client.Close();
        }
    }

    static void Broadcast(string message, ClientInfo sender)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\n");
        lock (locker)
        {
            foreach (var c in clients)
            {
                if (c != sender)
                {
                    try { c.Client.GetStream().Write(data, 0, data.Length); }
                    catch { }
                }
            }
        }
        Console.WriteLine(message);
    }
}
