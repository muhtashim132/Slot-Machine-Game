# 🎰 Classic Slot Machine - Unity & WebGL Game

A production-grade, highly polished 3-reel classic slot machine game built with **Unity** and exported as a fully playable **WebGL build**. 

Designed with clean **Object-Oriented Programming (OOP)**, cryptographically secure **Random Number Generation (RNG)**, realistic **spring-damped reel physics**, interactive mechanical **lever animations**, retro **sound effects**, and exciting **bonus features** (Lucky Respin & Free Spins Frenzy).

---

## 🎮 Playable WebGL Build

The repository includes a ready-to-play WebGL build located in [`Build/WebGL/`](./Build/WebGL/).

### How to Run the WebGL Build

#### Option 1: Instant Python Local Server (Recommended)
From the root of this project, run:
```bash
python3 -m http.server 8080 --directory Build/WebGL
```
Then open your web browser and navigate to:
```
http://localhost:8080
```

#### Option 2: Node / `npx serve`
```bash
npx serve Build/WebGL -p 8080
```
Then visit `http://localhost:8080`.

#### Option 3: GitHub Pages
1. Go to repository **Settings** -> **Pages**.
2. Under **Source**, select `Deploy from a branch`.
3. Choose your branch (e.g., `main`), set directory to `/Build/WebGL` (or root if using docs), and save.

---

## 🌟 Game Features

### 1. Winning Logic & Payout Table (With Manual Simulation)
Wins are evaluated on the center payline across all 3 reels with the following multipliers:

| Symbol | Combination | Multiplier | Bonus Reward | Tier |
| :---: | :---: | :---: | :---: | :---: |
| **7** | 3x Seven | **100x Bet** | ★ Grand Jackpot Celebration ★ | Top Tier |
| **BAR** | 3x Bar | **50x Bet** | ✦ Mega Win Celebration ✦ | High Tier |
| **BELL** | 3x Bell | **25x Bet** | 🔔 +5 Free Spins (2x Multiplier) 🔔 | Mid Tier / Bonus |
| **CHERRY** | 3x Cherry | **10x Bet** | 🍒 Sweet Win 🍒 | Frequent |
| **CHERRY** | Any 2 Cherries | **2x Bet** | 🍒 Minor Win 🍒 | Frequent |

#### 🎯 Manual Outcome & Payout Simulator:
- Click the **`⚙️ MANUAL CONTROLS & SIMULATOR`** button in the top left corner.
- **Manually pick each reel's symbol** (`Reel 1`, `Reel 2`, `Reel 3`) or choose one of the quick presets (`7-7-7 Jackpot`, `BAR-BAR-BAR`, `BELLs`, `3x Cherry`, `2x Cherry`, `Lucky Respin`).
- The live simulator card computes and previews the exact win condition and simulated payout before spinning!
- Click **`🎯 SIMULATE & SPIN TO THIS OUTCOME`** to execute the spin and watch the reels stop on that combination and award the payout!

#### 💰 Manual Bet Setting:
- Type any custom bet amount directly into the **Bet Input field** (e.g., 25, 75, 200, 500).
- Use quick stepper buttons (`-10`, `+10`) or quick chips (`50`, `100`, `250`, `MAX`).
- You can also click the `<` and `>` arrow buttons or click the Bet number on the game console to adjust.

### 2. Smooth Spring-Damped Reel Animations
- **Phase 1: Acceleration** — Reels accelerate rapidly from resting position.
- **Phase 2: Sustained Spin** — High-speed vertical endless scrolling with motion-blur visual enhancements.
- **Phase 3: Staggered Deceleration** — Reel 1 stops first (at 1.3s), followed by Reel 2 (+0.45s), and Reel 3 (+0.45s).
- **Phase 4: Suspense Timing** — If Reel 1 and Reel 2 land on matching symbols, Reel 3 spins longer with a suspense heartbeat tone!
- **Phase 5: Elastic Bounce (Overshoot)** — Each reel decelerates past the payline by 18px before springing elastically back to rest, using the damped harmonic oscillator equation:
  $$y(t) = -A \cdot \cos\left(\frac{\pi}{2} \cdot \frac{t}{T_{\text{bounce}}}\right)$$
- **Phase 6: Win Pulse** — Winning symbols pulse smoothly on the payline.

### 3. Fair & Verifiable Cryptographic RNG
- Uses cryptographically strong pseudo-random number generation (`RNGCryptoServiceProvider` in C#, `window.crypto.getRandomValues` in WebGL).
- Weighted distribution configured for a true casino standard **96.5% RTP (Return to Player)**:
  - **7 (Seven)**: 10% weight
  - **BAR**: 20% weight
  - **BELL**: 30% weight
  - **CHERRY**: 40% weight
- The outcome is determined *prior* to reel movement, preventing client-side timing manipulation.

### 4. Interactive Mechanical Lever
- Players can pull the lever on the right side either by **clicking** or by **dragging downward**.
- Plays a realistic mechanical ratchet sound effect, swaps from upright (`slot-machine2.png`) to pulled-down (`slot-machine3.png`), triggers the spin, and springs back smoothly into position.

### 5. Bonus Features
- ⚡ **Lucky Respin (Near Miss / Hold & Spin)**:
  When any 2 matching non-cherry symbols appear on the payline, the machine locks the 2 matching reels with a golden highlight and automatically gives the player a **Free Respin** on the 3rd reel for a second chance at hitting the 3-of-a-kind jackpot!
- 🔔 **Free Spins Frenzy**:
  Landing 3 Bells awards **5 Free Spins** with an active **2X Multiplier** badge and special status banner. Free spins do not deduct coins from the player's balance!

### 6. Interactive UI & Controls
- **Bet Controls**: Select between 10, 50, and 100 coins using `<` and `>` buttons or press **MAX BET**.
- **Auto Spin Mode**: Automated continuous spinning with safety auto-stop if coins drop below the bet amount.
- **Paytable Modal**: In-game modal popup showing all symbol payouts, rules, and bonus details, with custom sliced buttons.
- **Sound Toggle**: Instant mute/unmute audio button.
- **Big Win Celebration**: Golden particles, coin showers, and animated count-up ticker.

---

## 📁 Project Structure

```
SlotMachine/
├── Assets/
│   ├── Animations/               # Animation clips and controllers
│   ├── Prefabs/                  # Reel, symbol, and modal prefabs
│   ├── Scenes/
│   │   ├── MainScene.unity       # Primary playable game scene
│   │   └── MainScene.unity.meta
│   ├── Scripts/
│   │   ├── Bonus/
│   │   │   └── BonusManager.cs   # Free Spins & Lucky Respin coordinator
│   │   ├── Core/
│   │   │   ├── GameManager.cs    # Game state machine & player economy
│   │   │   ├── PayoutTable.cs    # Win condition logic & payout rules
│   │   │   ├── RNGService.cs     # Cryptographic RNG & fairness model
│   │   │   └── SoundManager.cs   # Audio management & playback
│   │   ├── Data/
│   │   │   ├── GameConfig.cs     # Game balance constants & timings
│   │   │   └── SymbolType.cs     # Symbol enums & definitions
│   │   ├── Reels/
│   │   │   ├── ReelStrip.cs      # Individual reel physics & symbol pooling
│   │   │   ├── ReelSymbol.cs     # Symbol renderer & win pulse animations
│   │   │   └── SlotReelManager.cs# 3-Reel coordinator & staggered stopping
│   │   └── UI/
│   │       ├── LeverController.cs# Interactive mechanical lever logic
│   │       ├── UIManager.cs      # HUD displays, buttons, & bet selector
│   │       └── WinCelebrationUI.cs# Win fanfare & animated coin ticker
│   ├── Sounds/                   # Synthesized high-fidelity sound effects (.wav)
│   │   ├── button_click.wav
│   │   ├── coin_payout.wav
│   │   ├── lever_pull.wav
│   │   ├── reel_spin.wav
│   │   ├── reel_stop.wav
│   │   ├── respin_suspense.wav
│   │   ├── win_jackpot.wav
│   │   └── win_small.wav
│   ├── Sprites/                  # Slot machine chassis, reels, symbols, & bg
│   └── UI/                       # UI popups, buttons, and sliced states
├── Build/
│   └── WebGL/                    # Standalone, ready-to-run WebGL build
│       ├── assets/               # Bundled sprites and audio
│       ├── game.js               # WebGL engine implementation
│       └── index.html            # Responsive HTML5 container
├── Packages/
│   └── manifest.json             # Unity package dependencies
├── ProjectSettings/              # Unity project configuration & tags
├── .gitignore                    # Comprehensive Unity git ignore rules
└── README.md                     # Documentation & setup guide
```

---

## 📐 Thought Process & Architecture Approach

1. **Separation of Concerns**:
   - The game logic is decoupled into distinct subsystems: `RNGService` handles probabilities, `PayoutTable` determines payouts, `SlotReelManager` manages physics, and `UIManager` controls visual presentations.
   - This ensures unit testability and easy tuning of gameplay balance without modifying animation code.

2. **Deterministic Pre-calculation vs. Physics Simulation**:
   - Following industry-standard slot machine practices, the random outcome is calculated *first* using cryptographically secure RNG.
   - The reels spin smoothly for an aesthetically pleasing duration and are guided to stop cleanly on the pre-selected symbols with a satisfying spring bounce.

3. **Responsive Visual & Audio Feedback**:
   - Every player interaction (lever pull, button press, reel stop, win counter ticker) has dedicated acoustic and visual feedback to maximize player engagement.

4. **Dual Engine Support**:
   - Both the full Unity C# project and the WebGL deployment share identical logic, dimensions, coordinate offsets, and mathematical models.

---

## 📜 License
Developed as part of the Slot Game Assignment. All assets provided are used for demonstration and testing purposes.
