using System;
using System.Net;
using System.Net.Sockets;
using DiceGame.Networking;

namespace Client;

public class Client
{
    public async Task Run()
    {
        int port = 5678;
        string ServerIp = "127.0.0.1";
        IPAddress ipAddress = IPAddress.Parse(ServerIp);
        IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, port);

        Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        sender.Connect(remoteEndPoint);

        System.Console.WriteLine("Connected!");
        HHTPClient client = new(sender);

        var hello = await client.RecievePacket();
        System.Console.WriteLine("Recieved packet");
        System.Console.WriteLine(hello.Payload);
    }
}
