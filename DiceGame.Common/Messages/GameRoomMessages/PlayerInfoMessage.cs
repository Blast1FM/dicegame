using DiceGame.Common.Models;

namespace DiceGame.Common.Messages;

public class PlayerInfoMessage : BaseMessage
{
    public PlayerInfo PlayerInfo {get; set;}
    public PlayerInfoMessage(PlayerInfo playerInfo)
    {
        PlayerInfo = playerInfo;
    }
}