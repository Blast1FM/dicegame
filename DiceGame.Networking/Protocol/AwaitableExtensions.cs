using System.Net.Sockets;

namespace DiceGame.Networking;


public static class AwaitableExtensions
{
    /// <summary>
    /// Receive data using the specified awaitable class
    /// </summary>
    /// <param name="socket">The socket</param>
    /// <param name="awaitable">An instance of <see cref="SocketAwaitable"/></param>
    /// <returns><paramref name="awaitable"/></returns>
    public static SocketAwaitable ReceiveAsync
    (
        this Socket socket,
        SocketAwaitable awaitable
    )
    {
        awaitable.Reset();
        if (!socket.ReceiveAsync(awaitable.EventArgs))
            awaitable.IsCompleted = true;
        return awaitable;
    }
    /// <summary>
    /// Sends data using the specified awaitable class
    /// </summary>
    /// <param name="socket">The socket</param>
    /// <param name="awaitable">An instance of <see cref="SocketAwaitable"/></param>
    /// <returns><paramref name="awaitable"/></returns>
    public static SocketAwaitable SendAsync
    (
        this Socket socket,
        SocketAwaitable awaitable
    )
    {
        awaitable.Reset();
        if (!socket.SendAsync(awaitable.EventArgs))
            awaitable.IsCompleted = true;
        return awaitable;
    }
}
