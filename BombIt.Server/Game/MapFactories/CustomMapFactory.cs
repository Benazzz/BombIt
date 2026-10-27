using BombIt.Server.Game.MapFactories.Products;
using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.MapFactories;

// Abstract Factory Pattern - Concrete Factory
// Žemėlapio išdėstymą ir atsiradimo taškus deleguoja kitai gamyklai,
// o temą pateikia žaidėjo sukurtą. NĖRA registruojama MapFactoryRegistry,
// nes sukuriama kiekvienam mačui atskirai (kad "Random" jos netyčia nepasirinktų).
public sealed class CustomMapFactory : IMapFactory
{
    private readonly IMapFactory _layoutSource;
    private readonly MapTheme _theme;

    public CustomMapFactory(IMapFactory layoutSource, MapTheme theme)
    {
        ArgumentNullException.ThrowIfNull(layoutSource);
        ArgumentNullException.ThrowIfNull(theme);
        _layoutSource = layoutSource;
        _theme = theme;
    }

    public string Name => "Custom";

    public Map CreateMap() => _layoutSource.CreateMap();

    public MapTheme CreateTheme() => _theme;

    public ISpawnPointProvider CreateSpawnPoints(Map map) => _layoutSource.CreateSpawnPoints(map);
}
