// Observer Pattern

namespace BombIt.Server.Game.Events
{
    public interface IGameEventObserver
    {
        void Update(GameEvent gameEvent);
    }
}
