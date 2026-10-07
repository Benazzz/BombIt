// Observer Pattern
namespace BombIt.Server.Game.Events;

public class GameEventPublisher : IGameEventSubject
{
    private readonly List<IGameEventObserver> _observers = new();

    public void Attach(IGameEventObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Detach(IGameEventObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(GameEvent gameEvent)
    {
        // ToList() – kopija, kad stebėtojas galėtų atsijungti pranešimo metu
        foreach (var observer in _observers.ToList())
            observer.Update(gameEvent);
    }
}