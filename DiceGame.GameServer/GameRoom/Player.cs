using DiceGame.Common.Models;
using DiceGame.Networking;

namespace DiceGame.GameServer.GameRoom;

public class Player
{
    public PlayerInfo? PlayerInfo { get; set; }
    public HHTPClient HHTPClientConnection {get;set;}

}
