using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using DiceGame.Networking.Protocol;
using DiceGame.Networking.ServerBase;

namespace DiceGame.Networking;

public class HHTPClient
{
    private readonly ISocketWrapper _socket;
    public Socket Socket {get => _socket.GetSocket; }

    public async Task<Packet> ReceivePacket()
    {
        var receiveBuffer = new byte[1024];
        byte[] headerBytes = new byte[HeaderSerialiser.HeaderSize];
        int headerBytesRead = 0;

        // Read the header
        while (headerBytesRead < HeaderSerialiser.HeaderSize)
        {
            int bytesRead = await _socket.ReceiveAsync(receiveBuffer.AsMemory(headerBytesRead, HeaderSerialiser.HeaderSize - headerBytesRead),CancellationToken.None);
            if (bytesRead == 0)
            {
                throw new SocketException((int)SocketError.ConnectionReset);
            }
            Buffer.BlockCopy(receiveBuffer, 0, headerBytes, headerBytesRead, bytesRead);
            headerBytesRead += bytesRead;
        }

        PacketHeader header = HeaderSerialiser.DeserialiseHeader(headerBytes);

        // Read the payload
        int payloadLength = header.PayloadLength;
        byte[] payloadBytes = new byte[payloadLength];
        int payloadBytesRead = 0;

        while (payloadBytesRead < payloadLength)
        {
            int bytesToRead = Math.Min(receiveBuffer.Length, payloadLength - payloadBytesRead);
            int bytesRead = await _socket.ReceiveAsync(receiveBuffer.AsMemory(0, bytesToRead), CancellationToken.None);
            if (bytesRead == 0)
            {
                throw new SocketException((int)SocketError.ConnectionReset);
            }
            Buffer.BlockCopy(receiveBuffer, 0, payloadBytes, payloadBytesRead, bytesRead);
            payloadBytesRead += bytesRead;
        }

        string payload = Encoding.BigEndianUnicode.GetString(payloadBytes);
        return new Packet(header, payload);
    }

    public async Task<bool> SendPacket(Packet packet)
    {
        try
        {
            byte[] headerBytes = HeaderSerialiser.SerialiseHeader(packet.Header);
            byte[] payloadBytes = Encoding.BigEndianUnicode.GetBytes(packet.Payload);
            
            byte[] data = headerBytes.Concat(payloadBytes).ToArray();

            // Split into chunks? Unless it's done automagically

            // Send the little shit off
            // TODO check if this makes sense lol
            return await _socket.SendAsync(data)==data.Length;
            
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"TimeoutException during SendPacket: {ex.Message}");
            throw;
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException during SendPacket: {ex.SocketErrorCode}, {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during SendPacket: {ex.Message}");
            throw;
        }
    }
    public async Task SendErrorPacket(Packet request, string errorMessage)
    {
        Packet errorPacket = new
        (
            StatusCode.Error,
            request.Header.ProtocolMethod,
            request.Header.ResourceIdentifier,
            errorMessage
        );

        await SendPacket(errorPacket);
    }

    public void Connect(EndPoint endPoint)
    {
        try
        {
            _socket.Connect((IPEndPoint)endPoint);
        }
        catch (SocketException e)
        {
            System.Console.WriteLine($"Socket exception: {e.SocketErrorCode}:{e.Message}");
            throw;
        }
        catch(Exception e)
        {
            System.Console.WriteLine($"Unknown exception: {e.Message}");
            throw;
        }
        
    }
    public void CloseConnection()
    {
        try
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        catch (SocketException e)
        {
            System.Console.WriteLine($"Socket exception: {e.SocketErrorCode}:{e.Message}");
            throw;
        }
        finally
        {
            _socket.Dispose();
        }
    }
    public HHTPClient(ISocketWrapper socket)
    {
        _socket = socket;
    }
}