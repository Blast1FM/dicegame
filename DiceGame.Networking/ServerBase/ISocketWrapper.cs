using System;
using System.Net;
using System.Net.Sockets;

namespace DiceGame.Networking.ServerBase;

public interface ISocketWrapper : IDisposable
{
    public Socket GetSocket {get;}
    public Task<int> ReceiveAsync(byte[]? receiveBuffer);
    public Task<int> ReceiveAsync(Memory<byte> receiveBuffer, CancellationToken cancellationToken);
    public Task<int> SendAsync(byte[] data);
    public void Shutdown(SocketShutdown how);
    public void Connect(EndPoint endpoint);
    public void Close();
}
