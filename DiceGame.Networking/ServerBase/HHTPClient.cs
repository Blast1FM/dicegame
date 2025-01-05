using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using DiceGame.Networking.Protocol;

namespace DiceGame.Networking;

public class HHTPClient
{
    private readonly Socket _socket;
    public Socket Socket {get => _socket; }
    private HeaderSerialiser _headerSerialiser;

    // TODO Rewrite method to account recieving less than 4 bytes on first recieve operation
    public async Task<Packet> RecievePacket()
    {
        // TODO make buffer length configurable if you can be bothered
        var recv = new byte[1024];
        var args = new SocketAsyncEventArgs();
        args.SetBuffer(recv, 0, recv.Length);
        var saw = new SocketAwaitable(args);

        PacketHeader header;
        int bufferOffset;

        await _socket.ReceiveAsync(saw);
        var bytesRead = args.BytesTransferred;
        byte[] headerBytes = new byte[4];

        if(bytesRead < HeaderSerialiser.HeaderSize)
        {
            int totalBytesRead = bytesRead;
            Buffer.BlockCopy(recv, 0, headerBytes, 0, bytesRead);

            while(totalBytesRead < HeaderSerialiser.HeaderSize)
            {
                await _socket.ReceiveAsync(saw);
                bytesRead = args.BytesTransferred;
                totalBytesRead += bytesRead;

                Buffer.BlockCopy(recv, 0, headerBytes, totalBytesRead-1, Math.Min(bytesRead, HeaderSerialiser.HeaderSize-totalBytesRead));
            }

            header = HeaderSerialiser.DeserialiseHeader(headerBytes);
            bufferOffset = args.BytesTransferred;
        }
        else
        {
            header = HeaderSerialiser.DeserialiseHeader(recv);
            // This is to account for the first 4 bytes being the header
            bufferOffset = 4;
        }

        int payloadLength = header.PayloadLength;

        byte[] payloadBytes = new byte[payloadLength];
        int payloadOffset = 0;

        // TODO WRITE A TEST FOR THIS SHIT
        while (bytesRead > 0 && payloadOffset < payloadLength)
        {
            int recvLengthOnCurrentIteration = recv.Length - bufferOffset;

            // Recieved more bytes than what we have left in the payload - should discard the remaining garbage
            if(recvLengthOnCurrentIteration > (payloadLength - payloadOffset))
            {
                Buffer.BlockCopy(recv, bufferOffset, payloadBytes, payloadOffset, payloadLength - payloadOffset);
                payloadOffset = payloadLength;
            }
            else 
            {
                Buffer.BlockCopy(recv, bufferOffset, payloadBytes, payloadOffset, recvLengthOnCurrentIteration);
                payloadOffset += recvLengthOnCurrentIteration;
            }
            
            // If recieve async doesn't overwrite from the start i may have a problem here
            await _socket.ReceiveAsync(saw);
            bytesRead = args.BytesTransferred;

            bufferOffset = 0;
        }

        string payload = Encoding.BigEndianUnicode.GetString(payloadBytes);

        return new Packet(header, payload);
    }
    public async Task SendPacket(Packet packet)
    {
        try
        {
            byte[] headerBytes = HeaderSerialiser.SerialiseHeader(packet.Header);
            byte[] payloadBytes = Encoding.BigEndianUnicode.GetBytes(packet.Payload);
            
            byte[] data = headerBytes.Concat(payloadBytes).ToArray();

            // Split into chunks? Unless it's done automagically

            // Send the little shit off

            var socketArgs = new SocketAsyncEventArgs();
            socketArgs.SetBuffer(data, 0, data.Length);
            var saw = new SocketAwaitable(socketArgs);

            await _socket.SendAsync(saw);
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
    public HHTPClient(Socket socket)
    {
        _socket = socket;
        _headerSerialiser = new();
    }
}