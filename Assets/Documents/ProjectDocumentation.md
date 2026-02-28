# Nautilus Engine - Project Documentation

## Overview
**Nautilus Engine** is a 3x3 slot machine game built in Unity. It features a unique **Sea Animals Steampunk** aesthetic, a dynamic economy system, and a unique "Voltage Multiplier" feature that rewards consecutive wins.

## Architecture
The project follows **SOLID principles** to ensure modularity and maintainability.

### Core Scripts

#### 1. `SlotMachineManager.cs`
**Responsibility**: Manages the game flow, UI, and user input.
- **Key Functions**:
  - `OnSpinClick()`: Orchestrates the spin sequence (deduct bet, spin reels, handle result).
  - `GenerateResults()`: Simulates server-side RNG to determine the grid outcome.
  - `HandleSpinResult()`: Calculates wins and applies the consecutive win multiplier.
  - `ProcessWin()`: Updates balance, triggers animations, and plays sounds.
- **Server Integration Points**:
  - `Start()`: Fetch initial balance/state from API.
  - `DeductBet()`: Send bet request to server.
  - `GenerateResults()`: Replace local RNG with server response containing the result grid.
  - `ProcessWin()`: Validate win amount with server data.

#### 2. `ReelController.cs`
**Responsibility**: Handles the visual representation and animation of a single reel.
- **Key Functions**:
  - `InitializeReel()`: Sets up the "infinite strip" by instantiating symbols.
  - `Spin()`: Moves the strip vertically and handles the looping logic.
  - `SnapToResult()`: Aligns the strip to show the specific result symbols.
  - `HighlightSymbol()`: Animates winning symbols (scale/color punch).

#### 3. `WinEvaluator.cs` (implements `IWinEvaluator`)
**Responsibility**: Pure logic class for calculating wins.
- **Key Functions**:
  - `Evaluate()`: Analyzes the 3x3 grid for matches on 3 rows and 2 diagonals. Returns a `WinResult` struct containing the total win amount, multiplier, and winning lines.

#### 4. `SoundManager.cs`
**Responsibility**: Singleton managing all audio playback.
- **Features**:
  - Audio Mixer integration for volume control.
  - Pitch randomization for SFX variety.
  - Dedicated methods for game events (`PlaySpinStart`, `PlayWin`, etc.).

#### 5. `SymbolDataSO.cs`
**Responsibility**: ScriptableObject holding data for all slot symbols (Sprites, Payout Values).

## Game Mechanics

### Win Logic
- **Grid**: 3x3.
- **Paylines**: 3 Horizontal, 2 Diagonal.
- **Payout Formula**: `BasePayout * (CurrentBet / MinBet) * Multiplier`.
  - This ensures payouts scale linearly with the bet size.

### Voltage Multiplier (Consecutive Wins)
- **Mechanism**: Winning streaks increase the payout multiplier.
  - 1st Win: 1x
  - 2nd Win: 2x
  - 3rd Win: 3x
  - 4th+ Win: 5x (Capped)
- **Reset**: Any loss resets the streak to 0.
- **Persistence**: The streak is saved between sessions.

### Visuals
- **Reels**: Implemented as scrolling strips (not sprite swaps) for realism.
- **Animations**: DOTween is used for UI punches, text fades, and reel bouncing.

## Future Server Integration
To connect this client to a backend:
1.  **Authentication**: Implement login in a separate manager.
2.  **API Calls**: Replace `PlayerPrefs` and local logic in `SlotMachineManager` with `UnityWebRequest` or a networking library.
    - `POST /spin`: Send `betAmount`. Receive `grid`, `totalWin`, `balance`.
3.  **Validation**: Ensure the client-side `WinEvaluator` matches the server's logic for verification, but trust the server's result.
