namespace BombIt.Server.Game.MapFactories;

// Vienintele vieta, kur registruojami zemelapiai.
// Naujas zemelapis = nauja IMapFactory realizacija + viena eilute cia.
// Zemelapio pavadinimas (IMapFactory.Name) automatiskai atsiranda pasirinkime.
public static class MapFactoryRegistry
{
    private static readonly IReadOnlyList<IMapFactory> Factories = new IMapFactory[]
    {
        new ClassicMapFactory(),
        new DesertMapFactory(),
    };

    public static IReadOnlyList<IMapFactory> All => Factories;

    public static IMapFactory Default => Factories[0];

    // Grazina null, jei pavadinimo nera (pvz. "Random") - tada zemelapis renkamas atsitiktinai
    public static IMapFactory? FindByName(string? name) =>
        Factories.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}