using System;
using System.Net;
using System.Net.Sockets;

namespace DiceGame.Networking.ServerBase;

public class SocketWrapper : ISocketWrapper
{
    public SocketWrapper(Socket socket)
    {
        _socket = socket;
    }
    private Socket _socket;
    public Socket GetSocket => _socket;

    public void Close()
    {
        _socket.Close();
    }

    public void Connect(EndPoint endpoint)
    {
        _socket.Connect(endpoint);
    }

    public void Dispose()
    {
        _socket.Dispose();
    }

    public async Task<int> ReceiveAsync(byte[]? receiveBuffer)
    {
        return await _socket.ReceiveAsync(receiveBuffer);
    }

    public async Task<int> SendAsync(byte[] data)
    {
        return await _socket.SendAsync(data);
    }

    public void Shutdown(SocketShutdown how)
    {
        _socket.Shutdown(how);
    }
}
