using DiceGame.Common.Models;
using DiceGame.Networking;

namespace DiceGame.GameServer.GameRoom;

public class Player
{
    public PlayerInfo? PlayerInfo { get; set; }
    public HHTPClient HHTPClientConnection {get;set;}
    public decimal RequestedPayout {get; set;}

    public Player(PlayerInfo playerinfo, HHTPClient clientConnection, decimal requestedPayout)
    {
        PlayerInfo = playerinfo;
        HHTPClientConnection = clientConnection;
        RequestedPayout = requestedPayout;
    }
}
