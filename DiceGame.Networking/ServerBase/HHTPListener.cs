using System.Net;
using System.Net.Sockets;

namespace DiceGame.Networking;

public class HHTPListener
{
    private CancellationTokenSource _cts = new();
    public CancellationToken ListenerCancellationToken {get=> _cts.Token;}
    private Task? _listeningTask;
    public int ListeningPort{get;set;} = 5678;
    public event EventHandler<ClientConnectedEventArgs>? ClientConnected;
    protected virtual void OnClientConnected(ClientConnectedEventArgs e)
    {
        ClientConnected?.Invoke(this, e);
    }

    public void StartListening()
    {
        _listeningTask = Listen(_cts.Token);
    }
    public async Task Listen(CancellationToken cancellationToken)
    {
        var endPoint = new IPEndPoint(IPAddress.Any, ListeningPort);
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endPoint);
        try
        {
            socket.Listen(10);
        }
        catch (SocketException e)
        {
            System.Console.WriteLine($"Socket exception: {e.Message}");
            socket.Dispose();
        }
        while(!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var acceptedClientSocket = await socket.AcceptAsync(cancellationToken);
                var eventArgs = new ClientConnectedEventArgs(acceptedClientSocket);
                OnClientConnected(eventArgs);
            }
            catch (OperationCanceledException e)
            {
                System.Console.WriteLine($"Operation cancelled: {e.Message}");
                socket.Dispose();
                break;
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Unhandled exception: {e.Message}");
            }
        }
    }

    public async Task StopListeningAsync()
    {
        if (_cts != null)
        {
            _cts.Cancel();
        }

        if (_listeningTask != null)
        {
            await _listeningTask;
        }
    }
}
