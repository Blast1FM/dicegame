using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class PlayerInitMessage : BaseMessage
{
    public PlayerInfo PlayerInfo {get; private set;}
    public decimal RequestedPayout {get;private set;}
    public PlayerInitMessage(PlayerInfo playerInfo, decimal requestedPayout)
    {
        PlayerInfo = playerInfo;
        RequestedPayout = requestedPayout;
    }
}