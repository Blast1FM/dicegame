namespace DiceGame.GameServer.GameRoom.States;

public abstract class GameState
{
    public event EventHandler<ChangeStateEventArgs>? StateChanged;
    public virtual void OnStateChanged(ChangeStateEventArgs e)
    {
        StateChanged?.Invoke(this,e);
    }
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
}