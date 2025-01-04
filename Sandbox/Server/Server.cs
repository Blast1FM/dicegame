using System.Text.Json;
using DiceGame.Common.Messages;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;

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
        HHTPClient client = new(e.ClientSocket);
        if(!client.Socket.Connected) 
        {
            System.Console.WriteLine($"Client disconnected prematurely");
            return;
        }
        System.Console.WriteLine($"Client connected from {e.ClientSocket.RemoteEndPoint}");
        BaseMessage message = new();
        Packet hello = new(StatusCode.Ok, ProtocolMethod.GET, 0, JsonSerializer.Serialize(message));
        System.Console.WriteLine($"Packet created, message text: {message.CreatedAt}");
        
        await client.SendPacket(hello);
        System.Console.WriteLine("Packet sent");
        client.CloseConnection();
    }
}
