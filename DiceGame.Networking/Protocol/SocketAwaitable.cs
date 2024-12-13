using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace DiceGame.Networking;

/// <summary>
/// An awaiter for asynchronous socket operations
/// </summary>
// adapted from Stephen Toub's code at
// https://blogs.msdn.microsoft.com/pfxteam/2011/12/15/awaiting-socket-operations/
public sealed class SocketAwaitable : INotifyCompletion
{
    // placeholder for when we don't have an actual continuation. does nothing
    readonly static Action _guard = () => { };
    // the continuation to use
    Action _continuation;

    /// <summary>
    /// Creates a new instance of the class for the specified <paramref name="eventArgs"/>
    /// </summary>
    /// <param name="eventArgs">The socket event args to use</param>
    public SocketAwaitable(SocketAsyncEventArgs eventArgs)
    {
        if (null == eventArgs) throw new ArgumentNullException("eventArgs");
        EventArgs = eventArgs;
        eventArgs.Completed += delegate
        {
            var prev = _continuation ?? Interlocked.CompareExchange(
                ref _continuation, _guard, null);
            if (prev != null) prev();
        };
    }
    /// <summary>
    /// Indicates the event args used by the awaiter
    /// </summary>
    public SocketAsyncEventArgs EventArgs { get; internal set; }
    /// <summary>
    /// Indicates whether or not the operation is completed
    /// </summary>
    public bool IsCompleted { get; internal set; }

    internal void Reset()
    {
        _continuation = null;
    }
    /// <summary>
    /// This method supports the async/await framework
    /// </summary>
    /// <returns>Itself</returns>
    public SocketAwaitable GetAwaiter() { return this; }
        
    // for INotifyCompletion
    void INotifyCompletion.OnCompleted(Action continuation)
    {
        if (_continuation == _guard ||
            Interlocked.CompareExchange(
                ref _continuation, continuation, null) == _guard)
        {
            Task.Run(continuation);
        }
    }
    /// <summary>
    /// Checks the result of the socket operation, throwing if unsuccessful
    /// </summary>
    /// <remarks>This is used by the async/await framework</remarks>
    public void GetResult()
    {
        if (EventArgs.SocketError != SocketError.Success)
            throw new SocketException((int)EventArgs.SocketError);
    }
}