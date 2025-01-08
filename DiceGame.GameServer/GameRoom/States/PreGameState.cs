using System;
using DiceGame.GameServer.GameRoom.Infrastructure;

namespace DiceGame.GameServer.GameRoom.States;

public class PreGameState : GameState
{
    private RequestRouter _requestRouter;
    private GameRoomController _controller;
    public PreGameState(GameRoomController controller)
    {
        _controller = controller;
    }
    public override async Task Enter()
    {
        
    }

    public override async Task Exit()
    {
        throw new NotImplementedException();
    }

    public override async Task Update()
    {
        throw new NotImplementedException();
    }
}
