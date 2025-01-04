using System.Net.Sockets;

namespace DiceGame.Networking;

public class ClientConnectedEventArgs : EventArgs
{
    public Socket ClientSocket{get; private set;}

    public ClientConnectedEventArgs(Socket socket)
    {
        ClientSocket = socket;
    }
}