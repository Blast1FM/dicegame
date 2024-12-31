using System.Net;
using DiceGame.Common.Models;
using DiceGame.Networking;

namespace DiceGame.GameServer.GameRoom.Infrastructure;

public class Player
{
    public PlayerInfo PlayerInfo { get; set; }
    public HHTPClient HHTPClientConnection {get;set;}
    public IPEndPoint IPEndPoint {get;}
    public bool IsConnected {get;set;}
    public DateTime LastSeen {get;set;}
    public Player(PlayerInfo playerinfo, HHTPClient clientConnection)
    {
        PlayerInfo = playerinfo;
        HHTPClientConnection = clientConnection;
        IPEndPoint = (IPEndPoint)clientConnection.Socket.RemoteEndPoint!;
        IsConnected = true;
        LastSeen = DateTime.Now;
    }
}
