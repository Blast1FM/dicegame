namespace DiceGame.Common.Messages;

public class CreateLobbyRequestMessage : BaseMessage
{
    public string LobbyName {get;set;}
}