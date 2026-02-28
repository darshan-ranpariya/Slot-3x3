# Slot 3x3 (Nautilus Engine)

A 3x3 slot machine game built in Unity with a sea-animal steampunk theme. It features a dynamic economy system, a 5-line paytable (3 horizontal, 2 diagonal), and a “Voltage Multiplier” that rewards consecutive wins.

## Quick Start

1. Open the project in Unity `6000.3.10f1`.
2. Open the main scene: `Assets/Scenes/SlotGamePlay.unity`.
3. Press Play.

## Gameplay Summary

- Grid: 3x3
- Paylines: 3 horizontal, 2 diagonal
- Payout formula: `BasePayout * (CurrentBet / MinBet) * Multiplier`
- Voltage Multiplier (win streaks): 1x, 2x, 3x, 5x (cap). Any loss resets the streak.

## Core Scripts

- `Assets/Scripts/SlotMachineManager.cs`: Game flow, input, balance updates, spin orchestration.
- `Assets/Scripts/ReelController.cs`: Reel animation, strip looping, snapping to results, highlights.
- `Assets/Scripts/WinEvaluator.cs`: Pure win-logic for the 3x3 grid and paylines.
- `Assets/Scripts/SoundManager.cs`: Centralized audio playback and mixer integration.
- `Assets/Scripts/SymbolDataSO.cs`: Symbol definitions (sprites + payout values).

## Tech Notes

- Animation: DOTween for UI punches, fades, and reel bounce.
- Data: Symbol definitions are ScriptableObjects.
- Project template: URP 2D (see `ProjectSettings/ProjectSettings.asset`).

## Server Integration (Planned)

Client-side flow is designed to be server-authoritative. Intended integration points:

1. Authentication via a separate manager.
2. `POST /spin` with `betAmount` returns `grid`, `totalWin`, `balance`.
3. Client `WinEvaluator` mirrors server logic for verification only.

## Documentation

See `Assets/Documents/ProjectDocumentation.md` for a deeper architecture overview.

## License

See `LICENSE`.
