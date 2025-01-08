using System;
using DiceGame.Common.Models;

namespace DiceGame.Common.Messages.GameRoomMessages;

public class ConnectedPlayerListMessage : BaseMessage
{
    List<PlayerInfo> ConnectedPlayers {get;set;}
    public ConnectedPlayerListMessage(List<PlayerInfo> connectedPlayers)
    {
        ConnectedPlayers = connectedPlayers;
    }
}
