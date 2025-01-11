using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Demo.Common.Messages;
using DiceGame.Common.Messages;
using DiceGame.Common.Networking;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;
using DiceGame.Networking.ServerBase;

namespace Demo.Client;

public class Client
    {
        public Guid sessionToken;
        public async Task Run()
    {
        int port = 5678;
        string serverIp = "127.0.0.1";
        IPAddress ipAddress = IPAddress.Parse(serverIp);
        IPEndPoint remoteEndPoint = new(ipAddress, port);

        // Создание сокета и HHTPClient
        Socket serverSocket = new(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        HHTPClient hhtpClient = new(new SocketWrapper(serverSocket));

        try
        {
            // Подключение к серверу
            hhtpClient.Connect(remoteEndPoint);
            Console.WriteLine("Connected to the server!");

            // Запрос токена сессии
            SessionTokenMessage message = new SessionTokenMessage(Guid.NewGuid());
            bool success = await hhtpClient.SendMessage<SessionTokenMessage>(message, StatusCode.Ok, ProtocolMethod.POST, 0);

            if (success)
            {
                Console.WriteLine("Session token request sent.");

                // Получение токена
                Packet responsePacket = await hhtpClient.ReceivePacket();
                if (responsePacket.Header.StatusCode == StatusCode.Ok)
                {
                    if (responsePacket.TryExtractMessageFromPacket<SessionTokenMessage>(out var responseMessage))
                    {
                        Console.WriteLine($"Session token received: {responseMessage.SessionToken}");
                        sessionToken = (Guid)message.SessionToken!;
                        
                        CurrentTimeMessage timeRequest = new();
                        await hhtpClient.SendMessage<CurrentTimeMessage>(timeRequest, StatusCode.Ok, ProtocolMethod.GET, 1);
                        Packet timeResponsePacket = await hhtpClient.ReceivePacket();
                            if (timeResponsePacket.Header.StatusCode == StatusCode.Ok)
                                {
                                if (timeResponsePacket.TryExtractMessageFromPacket<CurrentTimeMessage>(out var timeResponseMessage))
                                {
                                    Console.WriteLine($"Server time: {timeResponseMessage.CurrentTime}");
                                }
                                else
                                {
                                    Console.WriteLine("Failed to extract time response message.");
                                }
                            }
                    }
                    else
                    {
                        Console.WriteLine("Failed to extract session token.");
                    }
                }
                else
                {
                    Console.WriteLine($"Server returned error: {responsePacket.Payload}");
                }
            }
            else
            {
                Console.WriteLine("Failed to send session token request.");
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"Socket exception: {e.SocketErrorCode}: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unknown exception: {e.Message}");
        }
        finally
        {
            // Закрытие соединения
            hhtpClient.CloseConnection();
            Console.WriteLine("Connection closed.");
        }
    }
}