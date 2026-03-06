# Game Design Document: Neon Trinity Slots

## 1. Game Concept
*   **Game Name**: Neon Trinity Slots
*   **Theme & Mood**: Cyberpunk / Retro-Futuristic. A dark, sleek interface illuminated by glowing neon symbols. The mood is energetic, fast-paced, and arcade-like.
*   **Pitch**: A high-energy 3x3 slot game that revitalizes classic fruit machine mechanics with a modern neon aesthetic. Players chase the elusive "Triple Seven" jackpot while enjoying smooth, rapid-fire gameplay perfect for quick sessions.
*   **Main Feature (Original Idea)**: **"Voltage Multiplier"** — Randomly, the screen flickers with electricity before a spin, guaranteeing that any win resulting from that spin is multiplied by 2x, 3x, or 5x.
*   **Symbol Direction**:
    *   **High Value**: Seven (Neon Red), Bar (Gold), Bell (Cyan).
    *   **Low Value**: Plum, Orange, Lemon, Cherry (Stylized wireframe/neon look).
*   **Math Direction**:
    *   **RTP Range**: 96.0% - 96.5%
    *   **Volatility**: Medium
    *   **Feature Frequency**: Voltage Multiplier triggers approx. 1 in 50 spins.

## 2. Art Direction
*   **Resolution**: 1920x1080 (Landscape)
*   **Layout Sketch**:
    *   **Center**: 3x3 Grid framed by metallic pipes and neon tubes.
    *   **Bottom**: Control bar (Spin Button, Auto-Play, Bet Adjustment).
    *   **Top**: Game Logo and a digital "Jackpot" ticker.
    *   **Background**: Abstract dark city grid or circuit board pattern.
*   **Art Style Keywords**: Neon, Glow, Metallic, Arcade, Sleek.

## 3. Front-End
*   **Basic Round Flow**:
    1.  **Tap**: Player presses Spin.
    2.  **Spin**: Reels accelerate with a blur effect.
    3.  **Stop**: Reels stop sequentially (Left → Right) with a heavy mechanical "thud".
    4.  **Result**: Win lines pulse/glow; win amount pops up.
    5.  **Feature**: If "Voltage Multiplier" is active, electricity arcs across the screen during the win count-up.
*   **Server Communication**:
    *   Client sends: `betAmount`
    *   Server returns: `symbolGrid` (3x3 array), `totalWin`, `winLines` (array of winning coordinates), `multiplierActive` (bool).
*   **Highlight Moment**: **"Anticipation Spin"** — When two high-value symbols (e.g., Sevens) land on the first two reels, the third reel spins longer with a rising sound effect and visual glow, building tension before the final stop.

## 4. Back-End
*   **Server Controls**:
    *   **RNG**: Generates the random outcome for the grid.
    *   **Win Evaluation**: Calculates wins based on the server-side paytable.
    *   **Wallet**: Deducts bet and adds winnings to the user's balance.
*   **State Tracking**:
    *   Current Balance
    *   Current Bet Size
    *   Session History (Last 10 spins for verification)
*   **Minimal Data Fields**:
    *   `userId` (String)
    *   `balance` (Decimal)
    *   `bet` (Decimal)
    *   `currency` (String)
*   **RTP Handling**: All logic and probability weights are stored and executed server-side to prevent client-side tampering.

## 5. Sound
*   **Sound List**:
    *   **Spin**: Mechanical click followed by a rising electronic hum.
    *   **Stop**: Distinct metallic thud (slightly higher pitch for the 3rd reel).
    *   **Win**: Short synth arpeggio (length/intensity scales with win size).
    *   **Feature Trigger**: Electric zap or thunder crackle.
    *   **Background Loop**: Low-tempo Synthwave/Retrowave beat (bass-heavy, unobtrusive).
*   **Mood Keywords**: Electronic, Punchy, Retro.
