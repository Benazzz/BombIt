namespace BombIt.Shared.DTOs;

public class PlayerStateDto
{
    public string PlayerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public bool IsAlive { get; set; } = true;
}