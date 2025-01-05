using System.Net.Sockets;
using Moq;
using NUnit.Framework;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;

public class RecievePacketTests
{
    [Test]
    public async Task RecievePacket_LessThanFourBytesInitially_ReadsHeaderCorrectly()
    {
        // Arrange
        var mockSocket = new Mock<Socket>();
        var recvBuffer = new byte[1024];
        var args = new SocketAsyncEventArgs();
        args.SetBuffer(recvBuffer, 0, recvBuffer.Length);

        mockSocket.Setup(s => s.ReceiveAsync(args))
            .Returns(() =>
            {
                if (args.BytesTransferred == 0)
                {
                    Array.Copy(new byte[] { 1, 2 }, recvBuffer, 2);
                    args.SocketError = SocketError.Success;
                    typeof(SocketAsyncEventArgs).GetProperty(nameof(SocketAsyncEventArgs.BytesTransferred))
                    .SetValue(args, 2); // Simulate the first receive
                    args.SetBuffer(recvBuffer, 0, 2); // Set buffer for the first chunk
                    return false; // Indicate immediate completion
                }
                else
                {
                    Array.Copy(new byte[] { 3, 4 }, recvBuffer, 2);
                    args.SocketError = SocketError.Success;
                    typeof(SocketAsyncEventArgs).GetProperty(nameof(SocketAsyncEventArgs.BytesTransferred))
                    .SetValue(args, 4); // Use reflection to modify the property
                    args.SetBuffer(recvBuffer, 2, 2); // Set buffer for the second chunk
                    return false; // Indicate immediate completion
                }
            });


        var headerSerialiser = new HeaderSerialiser();
        var client = new HHTPClient(mockSocket.Object);

        // Act
        var packet = await client.RecievePacket();

        // Assert
        Assert.NotNull(packet);
        Assert.AreEqual(ProtocolVersion.V1, packet.Header.ProtocolVersion);
        Assert.AreEqual(StatusCode.Ok, packet.Header.StatusCode);
        Assert.AreEqual(6, packet.Header.PayloadLength);
        Assert.AreEqual(string.Empty, packet.Payload); // Assuming no payload for simplicity
    }
}
