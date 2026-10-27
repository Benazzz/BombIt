using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace BombIt.Server.Game.Services;

public static class SingletonThreadSafetyBenchmark
{
    public static void RunBenchmark()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    SINGLETON PATTERN: THREAD-SAFETY VERIFICATION      ");
        Console.WriteLine("=======================================================");

        const int threadCount = 50;
        var instances = new ConcurrentBag<GameStateManagerService>();
        var threadIds = new ConcurrentBag<int>();

        Console.WriteLine($"[1] Paleidžiama {threadCount} lygiagrečių gijų kreiptis į Singleton...");

        // Paleidžiame 50 gijų lygiagrečiai vienu metu
        Parallel.For(0, threadCount, i =>
        {
            threadIds.Add(Environment.CurrentManagedThreadId);
            
            // Kiekviena gija bando pasiimti Singleton egzempliorių
            var instance = GameStateManagerService.Instance;
            instances.Add(instance);
        });

        // Pasiimame unikalius gijų ID ir unikalius objektų hash kodus
        var uniqueThreads = threadIds.Distinct().Count();
        var uniqueInstances = instances.Select(x => RuntimeHelpers.GetHashCode(x)).Distinct().ToList();

        Console.WriteLine($"\n[2] Testo rezultatai:");
        Console.WriteLine($"  - Kreipimųsi skaičius:          {instances.Count}");
        Console.WriteLine($"  - Skirtingų vykdytų gijų skaičius: {uniqueThreads} gijų");
        Console.WriteLine($"  - Sukurtų Singleton egzempliorių: {uniqueInstances.Count} (Privalo būti 1!)");
        Console.WriteLine($"  - Singleton atminties HashCode:  0x{uniqueInstances.First():X8}");

        if (uniqueInstances.Count == 1)
        {
            Console.WriteLine("\n[IŠVADA]: Testas sėkmingas! Singleton yra 100% thread-safe.");
            Console.WriteLine("Visos gijos gavo tą patį atminties egzempliorių.");
        }
        else
        {
            Console.WriteLine("\n[IŠVADA]: Klaida! Singleton nėra gijoms saugus (Race Condition)!");
        }

        Console.WriteLine("=======================================================\n");
    }
}