using System.Net.Sockets;
using Moq;
using NUnit.Framework;
using DiceGame.Networking;
using DiceGame.Networking.Protocol;
using DiceGame.Networking.ServerBase;

public class RecievePacketTests
{
    [Test]
    public async Task RecievePacket_LessThanFourBytesInitially_ReadsHeaderCorrectly()
    {
        // Arrange
        var mockSocket = new Mock<ISocketWrapper>();

        PacketHeader expectedHeader = new(ProtocolVersion.V1, StatusCode.Ok, ProtocolMethod.GET, 0, 0);
        var expectedHeaderBytes = HeaderSerialiser.SerialiseHeader(expectedHeader);
        var receiveBuffer = new List<byte[]>
        {
            expectedHeaderBytes.Take(2).ToArray(),
            expectedHeaderBytes.Take(2).ToArray(),
        };

        int receiveCalls = 0;


        mockSocket.Setup( socket => socket.ReceiveAsync(It.IsAny<byte[]>()).Result)
        .Callback<byte[]>(buffer => {
            var chunk = receiveBuffer[receiveCalls];
            Array.Copy(chunk, buffer, chunk.Length);
        })
        .Returns(receiveBuffer[receiveCalls++].Length);
        
        var client = new HHTPClient(mockSocket.Object);

        // Act
        var packet = await client.ReceivePacket();

        // Assert
        Assert.That(packet.Header, Is.EqualTo(expectedHeader));

    }
}
