using System.Net;
using System.Net.Sockets;
using Demo.Common.Messages;
using DiceGame.Common.Messages;
using DiceGame.Common.Networking;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;
using DiceGame.Networking.ServerBase;

namespace Demo.Client;

public class Client
{
    public async Task Run()
    {
        int port = 5678;
        string ServerIp = "127.0.0.1";
        IPAddress ipAddress = IPAddress.Parse(ServerIp);
        IPEndPoint remoteEndPoint = new(ipAddress, port);
        Socket serverSocket = new(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        HHTPClient server = new(new SocketWrapper(serverSocket));
        try
        {
            CurrentTimeMessage currentTimeRequest = new(DateTime.Now);
            server.Connect(remoteEndPoint);
            System.Console.WriteLine("Connected!");

            bool success = await server.SendMessage<CurrentTimeMessage>(currentTimeRequest, StatusCode.Ok, ProtocolMethod.GET, 0);

            if(success)
            {
                System.Console.WriteLine($"Request sent");
                var currentTimeReponseResult = await server.ReceiveMessage<CurrentTimeMessage>();
                if(currentTimeReponseResult.Result == DataExchangeResult.Ok)
                {
                    System.Console.WriteLine($"Message received");
                    System.Console.WriteLine($"Current time: {currentTimeReponseResult.Message!.CurrentTime}");
                }
            }

            RandomNumberMessage randomNumberRequest = new(0);
            success = await server.SendMessage<RandomNumberMessage>(randomNumberRequest, StatusCode.Ok, ProtocolMethod.GET, 1);

            if(success)
            {
                System.Console.WriteLine($"Request sent");
                var currentTimeReponseResult = await server.ReceiveMessage<RandomNumberMessage>();
                if(currentTimeReponseResult.Result == DataExchangeResult.Ok)
                {
                    System.Console.WriteLine($"Message received");
                    System.Console.WriteLine($"Random number: {currentTimeReponseResult.Message!.Number}");
                }
            }

            ASCIIArtMessage artRequest = new("a","b");
            success = await server.SendMessage<ASCIIArtMessage>(artRequest, StatusCode.Ok, ProtocolMethod.GET, 2);

            if(success)
            {
                System.Console.WriteLine($"Request sent");
                var currentTimeReponseResult = await server.ReceiveMessage<ASCIIArtMessage>();
                if(currentTimeReponseResult.Result == DataExchangeResult.Ok)
                {
                    System.Console.WriteLine($"Message received");
                    System.Console.WriteLine($"Ascii art:\n{currentTimeReponseResult.Message!.Art}");
                }
            }
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
            await Task.Delay(Timeout.Infinite);
            server.CloseConnection();
        }
    }
}
