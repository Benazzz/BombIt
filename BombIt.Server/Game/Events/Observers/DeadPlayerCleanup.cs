// Observer Pattern
namespace BombIt.Server.Game.Events.Observers;

public class DeadPlayerCleanup : IGameEventObserver
{
    public void Update(GameEvent gameEvent)
    {
        if (gameEvent is PlayerDiedEvent died)
        {
            died.Player.ResetPowerUps();
        }
    }
}