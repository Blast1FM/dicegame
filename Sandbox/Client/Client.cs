using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using DiceGame.Common.Messages;
using DiceGame.Networking;
using DiceGame.Networking.ServerBase;

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
        HHTPClient client = new(new SocketWrapper(serverSocket));
        try
        {
            client.Connect(remoteEndPoint);
            System.Console.WriteLine("Connected!");
            var hello = await client.ReceivePacket();
            System.Console.WriteLine("Recieved packet");
            System.Console.WriteLine(JsonSerializer.Deserialize<BaseMessage>(hello.Payload)!.CreatedAt);
        }
        catch (SocketException e)
        {
            System.Console.WriteLine($"Socket exception: {e.SocketErrorCode}:{e.Message}");
            throw;
        }
        catch (Exception e)
        {
            System.Console.WriteLine($"Unknown exception: {e.Message}");
            throw;
        }
        finally
        {
            client.CloseConnection();
        }
    }
}
