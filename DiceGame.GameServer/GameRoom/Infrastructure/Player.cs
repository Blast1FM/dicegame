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
        // this horrible abomination is here because i wanted to deep clone it in one line
        IPEndPoint = (IPEndPoint)IPEndPoint!.Create(((IPEndPoint)clientConnection.Socket.RemoteEndPoint!).Serialize());
        IsConnected = true;
        LastSeen = DateTime.Now;
    }
}
