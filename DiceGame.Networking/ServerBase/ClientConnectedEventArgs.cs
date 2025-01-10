using System.Net.Sockets;

namespace DiceGame.Networking;

/// <summary>
/// Event args used to pass around a socket created by HHTPListener
/// </summary>
public class ClientConnectedEventArgs : EventArgs
{
    public Socket ClientSocket{get; private set;}

    public ClientConnectedEventArgs(Socket socket)
    {
        ClientSocket = socket;
    }
}