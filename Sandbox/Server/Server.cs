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

        while(true)
        {
            Thread.Sleep(100);
        }
        
    }

    public async void HandleClientConnected(object? sender, ClientConnectedEventArgs e)
    {
        System.Console.WriteLine($"Client connected from {e.clientSocket.RemoteEndPoint}");
        BaseMessage message = new();
        Packet hello = new(StatusCode.Ok, ProtocolMethod.GET, 0, JsonSerializer.Serialize(message));
        System.Console.WriteLine($"Packet created, message text: {message.CreatedAt}");
        HHTPClient client = new(e.clientSocket);
        await client.SendPacket(hello);
        System.Console.WriteLine("Packet sent");
    }
}
