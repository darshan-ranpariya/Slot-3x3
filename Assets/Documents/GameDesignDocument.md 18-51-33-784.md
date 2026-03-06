# Game Design Document: Nautilus Engine

## 1. Game Concept
*   **Game Name**: Nautilus Engine
*   **Theme & Mood**: Sea Animals Steampunk. A mechanical underwater world where brass gears meet bioluminescent creatures. The mood is mysterious, industrial, and adventurous.
*   **Pitch**: Dive into the depths with "Nautilus Engine", a 3x3 slot game where mechanical sea creatures power the reels. Spin the gears to unlock the treasures of the deep in this steampunk-infused aquatic adventure.
*   **Main Feature (Original Idea)**: **"Voltage Multiplier"** — Consecutive wins charge the engine, increasing the payout multiplier (2x, 3x, up to 5x) for sustained winning streaks.
*   **Symbol Direction**:
    *   **High Value**: Mechanical Kraken, Brass Turtle, Steam-Powered Shark.
    *   **Low Value**: Gear-shaped Starfish, Pipe-work Seahorse, Glowing Jellyfish.
*   **Math Direction**:
    *   **RTP Range**: 96.0% - 96.5%
    *   **Volatility**: Medium
    *   **Feature Frequency**: Multiplier activates on 2nd consecutive win.

## 2. Art Direction
*   **Resolution**: 1920x1080 (Landscape)
*   **Layout Sketch**:
    *   **Center**: 3x3 Grid framed by portholes, pipes, and pressure gauges.
    *   **Bottom**: Control panel resembling a submarine dashboard (Levers for spin, dials for bet).
    *   **Top**: Game Logo and a "Pressure" gauge tracking the multiplier.
    *   **Background**: Deep blue ocean with floating gears and bubbles.
*   **Art Style Keywords**: Steampunk, Aquatic, Mechanical, Brass, Bioluminescent.

## 3. Front-End
*   **Basic Round Flow**:
    1.  **Tap**: Player pulls the lever (Spin).
    2.  **Spin**: Reels rotate like heavy machinery.
    3.  **Stop**: Reels lock into place with a metallic clank.
    4.  **Result**: Winning lines light up with steam/electricity; win amount displayed on a nixie tube display.
    5.  **Feature**: Consecutive wins cause the "Voltage/Pressure" gauge to rise, indicating the active multiplier.
*   **Server Communication**:
    *   Client sends: `betAmount`
    *   Server returns: `symbolGrid` (3x3 array), `totalWin`, `winLines` (array of winning coordinates), `multiplierActive` (bool).
*   **Highlight Moment**: **"Engine Overload"** — When the 5x multiplier is reached, steam vents release pressure and the screen shakes slightly.

## 4. Back-End
*   **Server Controls**:
    *   **RNG**: Generates the random outcome for the grid.
    *   **Win Evaluation**: Calculates wins based on the server-side paytable.
    *   **Wallet**: Deducts bet and adds winnings to the user's balance.
*   **State Tracking**:
    *   Current Balance
    *   Current Bet Size
    *   Consecutive Win Count (for multiplier logic)
*   **Minimal Data Fields**:
    *   `userId` (String)
    *   `balance` (Decimal)
    *   `bet` (Decimal)
    *   `currency` (String)
*   **RTP Handling**: All logic and probability weights are stored and executed server-side.

## 5. Sound
*   **Sound List**:
    *   **Spin**: Hydraulic hiss and gear grinding.
    *   **Stop**: Heavy metallic latching sound.
    *   **Win**: Bubbling sound mixed with a steam whistle.
    *   **Feature Trigger**: Electrical hum or pressure valve release.
    *   **Background Loop**: Underwater ambience (sonar pings, muffled water) with a rhythmic clockwork beat.
*   **Mood Keywords**: Industrial, Submerged, Rhythmic.
