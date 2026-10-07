// Observer Pattern

namespace BombIt.Server.Game.Events
{
    public interface IGameEventSubject
    {
        void Attach(IGameEventObserver observer);
        void Detach(IGameEventObserver observer);
        void Notify(GameEvent gameEvent);
    }
}
