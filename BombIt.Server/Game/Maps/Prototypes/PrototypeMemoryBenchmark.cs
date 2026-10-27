// using System.Runtime.CompilerServices;
// using System.Runtime.InteropServices;
// using BombIt.Server.Game.Models;
// using BombIt.Shared.Enums;

// namespace BombIt.Server.Game.Maps.Prototypes;

// public static class PrototypeMemoryBenchmark
// {
//     public static void PrintComparison()
//     {
//         Console.WriteLine("\n=======================================================");
//         Console.WriteLine("    PROTOTYPE PATTERN: DEEP vs SHALLOW COPY TEST       ");
//         Console.WriteLine("=======================================================");

//         var original = Map.LoadDefault();
//         var deep = original.DeepClone();
//         var shallow = original.ShallowClone();

//         // 1. Map objektų identifikatoriai
//         Console.WriteLine($"[1] Map objektų nuorodos (Object HashCodes):");
//         Console.WriteLine($"  - Original (Prototipas):  0x{RuntimeHelpers.GetHashCode(original):X8}");
//         Console.WriteLine($"  - Deep Copy Map:          0x{RuntimeHelpers.GetHashCode(deep):X8}  (atskiras objektas)");
//         Console.WriteLine($"  - Shallow Copy Map:       0x{RuntimeHelpers.GetHashCode(shallow):X8}  (atskiras objektas)");
//         Console.WriteLine();

//         // 2. Tiles[][] išorinių masyvų nuorodos
//         Console.WriteLine($"[2] Tiles[][] matricos rodyklės (Array HashCodes):");
//         Console.WriteLine($"  - Original Tiles:         0x{RuntimeHelpers.GetHashCode(original.Tiles):X8}");
//         Console.WriteLine($"  - Deep Tiles:             0x{RuntimeHelpers.GetHashCode(deep.Tiles):X8}  (Naujas masyvas)");
//         Console.WriteLine($"  - Shallow Tiles:          0x{RuntimeHelpers.GetHashCode(shallow.Tiles):X8} (tas pats masyvas)");
//         Console.WriteLine();

//         // 3. 0-tos eilutės Tiles[0] tikrieji RAM atminties adresai
//         GCHandle handleOrig = GCHandle.Alloc(original.Tiles[0], GCHandleType.Pinned);
//         GCHandle handleDeep = GCHandle.Alloc(deep.Tiles[0], GCHandleType.Pinned);
//         GCHandle handleShallow = GCHandle.Alloc(shallow.Tiles[0], GCHandleType.Pinned);

//         try
//         {
//             IntPtr addrOrig = handleOrig.AddrOfPinnedObject();
//             IntPtr addrDeep = handleDeep.AddrOfPinnedObject();
//             IntPtr addrShallow = handleShallow.AddrOfPinnedObject();

//             Console.WriteLine($"[3] 0-tos eilutės Tiles[0] tikrieji RAM adresai (RAM Pointers):");
//             Console.WriteLine($"  - Original RAM Address:   0x{addrOrig.ToString("X")}");
//             Console.WriteLine($"  - Deep RAM Address:       0x{addrDeep.ToString("X")}  (kitas adresas atmintyje)");
//             Console.WriteLine($"  - Shallow RAM Address:    0x{addrShallow.ToString("X")} ");
//         }
//         finally
//         {
//             handleOrig.Free();
//             handleDeep.Free();
//             handleShallow.Free();
//         } 

//         Console.WriteLine();
//         Console.WriteLine("[4] Atliekame pakeitimus ir testuojame (Modifikuojame kopijų [1][1] langelį):");
        
//         // Pakeičiame shallow kopijoje langelį
//         shallow.Tiles[1][1] = TileType.Empty;
//         bool originalDamaged = original.Tiles[1][1] == TileType.Empty;
//         Console.WriteLine($"  - Modifikavus Shallow kopiją -> Originalas pakito? {originalDamaged} (Prototipas sugadintas)");

//         // Atstatome ir patikriname deep
//         original.Tiles[1][1] = TileType.Indestructible;
//         deep.Tiles[1][1] = TileType.Empty;
//         bool originalSafe = original.Tiles[1][1] == TileType.Indestructible;
//         Console.WriteLine($"  - Modifikavus Deep kopiją    -> Originalas išliko saugus? {originalSafe} (Protipas nepakito)");

//         Console.WriteLine("=======================================================\n");
//     }
// }