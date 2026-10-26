namespace BombIt.Server.Game.Maps.Builders;

using BombIt.Server.Game.Models;

// Builder Pattern - Builder Interface
public interface IMapBuilder
{
    IMapBuilder Reset(int width, int height);
    IMapBuilder AddBorderWalls();
    IMapBuilder AddPillarGrid();
    IMapBuilder FromLayout(string[] layout);
    IMapBuilder AddRandomDestructibles(double density, Random random);
    IMapBuilder ClearSpawnZones();
    Map Build();
}
