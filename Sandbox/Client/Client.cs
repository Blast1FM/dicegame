using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using DiceGame.Common.Messages;
using DiceGame.Common.Networking;
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
            var packet = await client.ReceivePacket();
            var message = packet.TryExtractMessageFromPacket<ErrorMessage>(out ErrorMessage eMessage);
            System.Console.WriteLine($"Message received");
            System.Console.WriteLine($"{eMessage.ErrorMessageText}");
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
