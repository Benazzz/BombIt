# Bomb It 

A multiplayer desktop clone of the classic "Bomb It" game, built from scratch using **C#**, **WPF**, and **ASP.NET Core SignalR**. The game uses a strict lockstep Client-Server architecture where the server is the absolute authority for physics, collisions, and game state.

## Architecture & How It Works

The game runs at a strict **30 Hz tick-rate**. 
1. **Client** sends raw input commands (Up, Down, Left, Right, Place Bomb) to the Server.
2. **Server** processes inputs, calculates physics (collisions, explosions, power-ups), and generates a new state (`GameStateDto`).
3. **Server** broadcasts the updated state to all connected clients 30 times a second.
4. **Client** passively renders the state it receives (No client-side prediction).

## Project Structure & Main Classes

The solution is divided into three projects:

### 1. BombIt.Server (ASP.NET Core)
Handles all game logic and client synchronization.
* `GameStateManager` (Singleton Pattern): The core engine. Tracks players, bombs, grid physics, power-ups, and manages the lobby/match lifecycle.
* `GameLoopService`: A background worker that runs the 30 Hz loop, calling `GameStateManager.Tick()` and broadcasting the result to clients.
* `GameHub`: The SignalR Hub that receives connections, disconnections, and inputs from clients.

### 2. BombIt.Client (WPF)
The desktop application responsible for rendering and input handling.
* `MainWindow.xaml.cs`: Captures keyboard inputs (W/A/S/D/Enter) and manages UI panels (Lobby, Game, Death Screen).
* `SignalRClient`: Manages the WebSocket connection to the server.
* `GameRenderer`: Translates the server's `GameStateDto` (and map arrays) into WPF visual elements on a Canvas.

### 3. BombIt.Shared
Contains shared definitions to ensure the client and server speak the exact same language.
* `DTOs/`: Data Transfer Objects like `GameStateDto`, `PlayerStateDto`, `MapDto`.
* `Enums/`: Shared states like `GamePhase`, `Direction`, `PowerUpType`.

## How to Run

You will need the **.NET 9.0 SDK**.

1. **Start the Server**
   Open a terminal in the `BombIt/BombIt.Server` folder and run:
   ```bash
   dotnet run
   ```
   *The server will start listening on `http://localhost:5071`.*

2. **Start the Client(s)**
   Open one or more terminals in the `BombIt/BombIt.Client` folder and run:
   ```bash
   dotnet run
   ```
3. **Play**
   * The first player to connect becomes the **Host**.
   * The Host can configure the match time and click **Start Game**.
   * Use **W, A, S, D** or **Arrow Keys** to move.
   * Press **Enter** to place a bomb.
