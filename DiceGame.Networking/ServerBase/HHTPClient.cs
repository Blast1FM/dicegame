using System.Net.Sockets;
using System.Security;
using System.Text;
using DiceGame.Networking.Protocol;

namespace DiceGame.Networking;

public class HHTPClient
{
    private readonly Socket _socket;
    public Socket Socket {get => _socket; }

    // TODO Rewrite method to account recieving less than 4 bytes on first recieve operation
    public async Task<Packet> RecievePacket()
    {
        // TODO make buffer length configurable if you can be bothered
        var recv = new byte[1024];
        var args = new SocketAsyncEventArgs();
        args.SetBuffer(recv, 0, recv.Length);
        var saw = new SocketAwaitable(args);

        await _socket.ReceiveAsync(saw);
        var bytesRead = args.BytesTransferred;

        var header = HeaderSerialiser.DeserialiseHeader(recv);
        int payloadLength = header.PayloadLength;
        
        // This is to account for the first 4 bytes being the header
        int bufferOffset = 4;

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
    public void CloseConnection()
    {
        try
        {
            _socket.Close();
        }
        catch (SocketException e)
        {
            System.Console.WriteLine($"Socket exception: {e.SocketErrorCode}:{e.Message}");
        }
        finally
        {
            _socket.Dispose();
        }
    }
    public HHTPClient(Socket socket)
    {
        _socket = socket;
    }
}