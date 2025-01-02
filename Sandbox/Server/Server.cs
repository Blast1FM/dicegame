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
        Packet hello = new Packet(StatusCode.Ok, ProtocolMethod.GET, 0, "HELLO");
        System.Console.WriteLine("Packet created");
        HHTPClient client = new(e.clientSocket);
        await client.SendPacket(hello);
        System.Console.WriteLine("Packet sent");
    }
}
