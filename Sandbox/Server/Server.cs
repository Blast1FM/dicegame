using System.Net.Sockets;
using System.Text.Json;
using DiceGame.Common.Messages;
using DiceGame.Common.Networking;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;
using DiceGame.Networking.ServerBase;

namespace Server;

public class Server
{
    public async Task Run()
    {
        HHTPListener listener = new HHTPListener();

        listener.StartListening();

        listener.ClientConnected += HandleClientConnected;

        System.Console.WriteLine("Started listening");

        await Task.Delay(Timeout.Infinite);
        
    }

    public async void HandleClientConnected(object? sender, ClientConnectedEventArgs e)
    {
        HHTPClient client = new(new SocketWrapper(e.ClientSocket));
        if(!client.Socket.Connected) 
        {
            System.Console.WriteLine($"Client disconnected prematurely");
            return;
        }
        System.Console.WriteLine($"Client connected from {e.ClientSocket.RemoteEndPoint}");

        ErrorMessage message = new(new string('a',20000));
        System.Console.WriteLine($"Packet created, message text: {message.ErrorMessageText}");
        try
        {
            var result = await client.SendMessage<ErrorMessage>(message, StatusCode.Ok, ProtocolMethod.POST, 0);
            System.Console.WriteLine($"{result}");
        }
        catch (SocketException ex)
        {
            if(ex.SocketErrorCode == SocketError.ConnectionReset)
            {
                System.Console.WriteLine($"Connection reset by client");
                return;
            }
            else
            {
                System.Console.WriteLine($"Socket exception: {ex.SocketErrorCode}:{ex.Message}");
                throw;
            }
        }
        catch(Exception ex)
        {
            System.Console.WriteLine($"Unhandled exception: {ex.Message}");
            throw;
        }
        System.Console.WriteLine("Packet sent");
        client.CloseConnection();
    }
}
