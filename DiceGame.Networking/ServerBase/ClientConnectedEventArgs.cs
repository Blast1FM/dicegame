using System.Net.Sockets;

namespace DiceGame.Networking;

public class ClientConnectedEventArgs : EventArgs
{
    public Socket clientSocket{get; private set;}

    public ClientConnectedEventArgs(Socket socket)
    {
        clientSocket = socket;
    }
}