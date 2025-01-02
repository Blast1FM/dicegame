using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using DiceGame.Common.Messages;
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

        Socket serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        HHTPClient client = new(serverSocket);

        client.Connect(remoteEndPoint);
        System.Console.WriteLine("Connected!");

        var hello = await client.RecievePacket();
        System.Console.WriteLine("Recieved packet");
        System.Console.WriteLine(JsonSerializer.Deserialize<BaseMessage>(hello.Payload)!.CreatedAt);
    }
}
