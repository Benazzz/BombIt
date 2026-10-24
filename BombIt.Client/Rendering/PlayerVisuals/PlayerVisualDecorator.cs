using System.Windows.Controls;
using BombIt.Shared.DTOs;

namespace BombIt.Client.Rendering.PlayerVisuals;

public abstract class PlayerVisualDecorator : IPlayerVisual
{
    protected readonly IPlayerVisual _wrappee;

    protected PlayerVisualDecorator(IPlayerVisual wrappee)
    {
        ArgumentNullException.ThrowIfNull(wrappee);
        _wrappee = wrappee;
    }

    public virtual void Draw(Canvas canvas, PlayerStateDto player, int tileSize, bool isHost)
    {
        _wrappee.Draw(canvas, player, tileSize, isHost);
    }
}
