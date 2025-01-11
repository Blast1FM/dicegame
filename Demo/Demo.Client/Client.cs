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
        server.Connect(remoteEndPoint);
        System.Console.WriteLine("Connected!");
        while(true)
        {
            System.Console.WriteLine($"Enter command:\n1.Get current server time\n2.Get a random number\n3.Get an ascii art message\n0.Exit");
            bool success = int.TryParse(Console.ReadLine(), out int cmd);
            if(!success)
            {
                System.Console.WriteLine($"unable to parse");
                continue;
            }
            switch(cmd)
            {
                case 0:
                    System.Console.WriteLine($"Cya");
                    server.CloseConnection();
                    return;
                case 1:
                    try
                    {
                        CurrentTimeMessage currentTimeRequest = new(DateTime.Now);
                        bool sendSuccess = await server.SendMessage<CurrentTimeMessage>(currentTimeRequest, StatusCode.Ok, ProtocolMethod.GET, 0);

                        if(sendSuccess)
                        {
                            System.Console.WriteLine($"Request sent");
                            var currentTimeReponseResult = await server.ReceiveMessage<CurrentTimeMessage>();
                            if(currentTimeReponseResult.Result == DataExchangeResult.Ok)
                            {
                                System.Console.WriteLine($"Message received");
                                System.Console.WriteLine($"Current time: {currentTimeReponseResult.Message!.CurrentTime}");
                            }
                        }
                    }catch (SocketException e)
                    {
                        System.Console.WriteLine($"Socket exception: {e.SocketErrorCode}:{e.Message}");
                        throw;
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine($"Unknown exception: {e.Message}");
                        throw;
                    }

                    break;
                case 2:
                try
                {
                    RandomNumberMessage randomNumberRequest = new(0);
                    success = await server.SendMessage<RandomNumberMessage>(randomNumberRequest, StatusCode.Ok, ProtocolMethod.GET, 1);

                    if(success)
                    {
                        System.Console.WriteLine($"Request sent");
                        var randomNumberMessage = await server.ReceiveMessage<RandomNumberMessage>();
                        if(randomNumberMessage.Result == DataExchangeResult.Ok)
                        {
                            System.Console.WriteLine($"Message received");
                            System.Console.WriteLine($"Random number: {randomNumberMessage.Message.Number}");
                        }
                    }
                }catch (SocketException e)
                {
                    System.Console.WriteLine($"Socket exception: {e.SocketErrorCode}:{e.Message}");
                    throw;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"Unknown exception: {e.Message}");
                    throw;
                }
                    break;
                case 3:
                try
                {
                    ASCIIArtMessage artRequest = new("a","b");
                    success = await server.SendMessage<ASCIIArtMessage>(artRequest, StatusCode.Ok, ProtocolMethod.GET, 2);

                    if(success)
                    {
                        System.Console.WriteLine($"Request sent");
                        var artResponse = await server.ReceiveMessage<ASCIIArtMessage>();
                        if(artResponse.Result == DataExchangeResult.Ok)
                        {
                            System.Console.WriteLine($"Message received");
                            System.Console.WriteLine($"Ascii art:\n{artResponse.Message!.Art}");
                        }
                    }
                }catch (SocketException e)
                {
                    System.Console.WriteLine($"Socket exception: {e.SocketErrorCode}:{e.Message}");
                    throw;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"Unknown exception: {e.Message}");
                    throw;
                }
                    break;
                default:
                    System.Console.WriteLine($"invalid command");
                    break;
            }

        }
    }
}
