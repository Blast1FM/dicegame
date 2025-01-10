using System.Net.Sockets;
using System.Text.Json;
using Demo.Common.Messages;
using DiceGame.Common.Messages;
using DiceGame.Common.Networking;
using DiceGame.GameServer.GameRoom.Infrastructure;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;
using DiceGame.Networking.ServerBase;

namespace Demo.Server;

public class Server
{
    public RequestRouter _router;
    public HHTPListener _listener;
    public int _maxRetryCount = 5;
    public TimeSpan _retryInterval = TimeSpan.FromSeconds(5);

    public Server()
    {
        _listener = new();
        _router = new();
    }
    public async Task Run()
    {
        _listener.StartListening();

        _listener.ClientConnected += HandleClientConnected;

        List<Action<Packet,HHTPClient>> getHandlers = [HandleGetCurrentTimeRequest];
        _router.SetGetHandlers(getHandlers);

        System.Console.WriteLine("Started listening");

        await Task.Delay(Timeout.Infinite);

    }

    public async void HandleClientConnected(object? sender, ClientConnectedEventArgs e)
    {
        HHTPClient client = new(new SocketWrapper(e.ClientSocket));
        System.Console.WriteLine($"Client connected from {e.ClientSocket.RemoteEndPoint}");

        while(true)
        {
            try
            {
                // Check if there's data available
                if (client.Socket.Available > 0)
                {
                    await HandleClientRequests(client);
                }
                else
                {
                    // No data available, wait before polling again
                    await Task.Delay(100); // Adjust polling interval as needed
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.WouldBlock || ex.SocketErrorCode == SocketError.IOPending)
            {
                // No data available, continue polling
                await Task.Delay(100);
            }
            catch (OperationCanceledException)
            {
                // Polling was canceled
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                break;
            }
        }
        
    }

    public async Task HandleClientRequests(HHTPClient client)
    {
        int retryCount = 0;
        bool success = false;

        while (retryCount < _maxRetryCount && !success)
        {
            if(retryCount >= _maxRetryCount)
            {
                System.Console.WriteLine($"Maximum retries reached, closing connection");
                client.CloseConnection();
            }

            if(retryCount > 0) await Task.Delay(_retryInterval);

            try
            {
                Packet? inboundPacket = await client.ReceivePacket(TimeSpan.FromSeconds(5));

                if(inboundPacket is null)
                {
                    retryCount++;
                    System.Console.WriteLine($"Timed out waiting for client packet, retry {retryCount}/{_maxRetryCount}");
                    continue;
                }

                System.Console.WriteLine($"Received packet from {client.Socket.RemoteEndPoint}");
                _router.RouteRequest(inboundPacket, client);
                success = true;
            }
            catch (Exception ex)
            {
                retryCount++;
                System.Console.WriteLine($"Retry {retryCount}/{_maxRetryCount}: {ex.Message}");
            }
        }
    }

    public async void HandleGetCurrentTimeRequest(Packet packet, HHTPClient clientConnection)
    {
        try
        {
            bool success = false;
            int retryCount = 0;
            CurrentTimeMessage response = new(DateTime.Now);
            while(!success || retryCount < _maxRetryCount)
            {
                success = await clientConnection.SendMessage<CurrentTimeMessage>(response, packet);
            }
        }
        catch (JsonException e)
        {
            ErrorMessage errorMessage = new($"Server json error: {e.Message}");
            bool success = await clientConnection.SendMessage<ErrorMessage>(errorMessage, packet);
        }
        catch (SocketException e)
        {
            System.Console.WriteLine($"Socket error: {e.ErrorCode}:{e.Message}");
            if(e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.NotConnected)
            {
                return;
            }
        }
        catch (Exception e)
        {
            ErrorMessage errorMessage = new($"Unknown server error: {e.Message}");
            bool success = await clientConnection.SendMessage<ErrorMessage>(errorMessage, packet);
        }
    }
}