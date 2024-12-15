using System;
using System.Net;
using System.Net.Sockets;

namespace DiceGame.Networking;

public class HHTPListener
{
    public event EventHandler<ClientConnectedEventArgs>? ClientConnected;
    protected virtual void OnClientConnected(ClientConnectedEventArgs e)
    {
        ClientConnected?.Invoke(this, e);
    }

    public int ListeningPort{get;set;} = 5678;
    public async void Run()
    {
        var endPoint = new IPEndPoint(IPAddress.Any, ListeningPort);
        using var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endPoint);
        try
        {
            socket.Listen(10);
        }
        catch (SocketException)
        {
            socket.Dispose();
        }
        while(true)
        {
            using var acceptedClientSocket = await socket.AcceptAsync();
            var eventArgs = new ClientConnectedEventArgs(acceptedClientSocket);
            OnClientConnected(eventArgs);
        }
    }
}
