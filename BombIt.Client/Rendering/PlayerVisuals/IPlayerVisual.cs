using System.Windows.Controls;
using BombIt.Shared.DTOs;

namespace BombIt.Client.Rendering.PlayerVisuals;

public interface IPlayerVisual
{
    void Draw(Canvas canvas, PlayerStateDto player, int tileSize, bool isHost);
}
