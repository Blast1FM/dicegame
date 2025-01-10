using System.Net;
using System.Net.Sockets;

namespace DiceGame.Networking;

/// <summary>
/// Used to listen for and accept connections.
/// </summary>
public class HHTPListener
{
    private CancellationTokenSource _cts = new();
    private Task? _listeningTask;
    public int ListeningPort{get;set;} = 5678;
    /// <summary>
    /// Event invoked once a client is connected. Subscribe to this to retrieve the accepted socket.
    /// </summary>
    public event EventHandler<ClientConnectedEventArgs>? ClientConnected;
    protected virtual void OnClientConnected(ClientConnectedEventArgs e)
    {
        ClientConnected?.Invoke(this, e);
    }
    /// <summary>
    /// Starts the asynchronous listening task
    /// </summary>
    public void StartListening()
    {
        _listeningTask = Listen(_cts.Token);
    }
    /// <summary>
    /// Creates a listening socket and starts listening for connections and accepting them.
    /// Upon connection, triggers the ClientConnected event and passes the accepted socket
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gracefully stops listening via the cancellation token
    /// </summary>
    /// <returns></returns>
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
