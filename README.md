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

### 1. Interactive Onboarding Walkthrough (First-Time Device Onboarding)
When launching the game for the first time on any device, an **interactive 5-step spotlight onboarding walkthrough** automatically introduces the player to the game:
- 💡 **Spotlight Focus Frame**: Dynamically tracks and illuminates the active feature on the arcade cabinet while dimming the background.
- 🎯 **Step-by-Step Guidance**:
  1. **The 3-Reel Payline**: Explains the center payline win conditions and default strict rules.
  2. **Bet Settings & Rules Configurator**: Demonstrates wager adjustments and the new **`⚙️ BET SETTINGS`** button on the console bar.
  3. **Interactive Lever & Spin**: Shows how to pull the mechanical lever or press Spacebar/Spin.
  4. **Paytable & Free Spins**: Explains symbol payouts, the 3-Bells Free Spins feature, and the 2X win multiplier.
  5. **Bankroll & Tour Replay**: Welcomes the player with 1,000 complimentary coins and notes that the tour can be replayed anytime.
- ❓ **Replay Anytime**: Click the **`❓ TOUR`** button on the bottom console bar or the top **`❓ HOW TO PLAY & WALKTHROUGH`** badge to restart the tour at any time.

---

### 2. Manual Bet Settings & Winning Rule Engine
The previous `MAX BET` button has been replaced with the prominent **`⚙️ BET SETTINGS`** button on the main console bar:

- ⚙️ **Access Bet Settings**: Click the **`⚙️ BET SETTINGS`** button directly on the bottom console, or click the **TOTAL BET** display.
- 💰 **Manual Bet Input**: Directly enter any bet amount in the numeric input field (e.g. `25`, `77`, `250`), use `-10`/`+10` stepper buttons, or click chip presets (`25`, `50`, `100`, `250`, `500`).
- ⭐ **Strict Assignment Rule (Default)**: Conforms strictly to the assignment requirement: **"Player wins when all slots have the same symbol"**. Secondary rules such as 2-cherries or respin are **disabled by default**, so only 3 matching symbols win!
- ⚖️ **Toggle Any Combination**: Enable or disable any winning condition with a single click. Want only 7-7-7 to pay out? Simply uncheck the other symbols.
- 🔢 **Custom Multipliers**: Edit the payout multiplier directly in the table for each combination (e.g., customize 7-7-7 from 100x to 500x, or BAR from 50x to 150x).
- ➕ **Add Custom Rules**: Define new winning combinations dynamically by specifying:
  - Condition: *All 3 Slots Match*, *At Least 2 Match*, or *All 3 Match Any Symbol*.
  - Target Symbol: Seven, BAR, Bell, or Cherry.
  - Multiplier: Any custom integer multiplier.
  - Bonus Feature: None, +5 Free Spins (2x multiplier), or Lucky Respin.
- 🎯 **Quick Rule Presets**:
  - **⭐ Strict Assignment**: Only 3 matching symbols win (7-7-7, BAR-BAR-BAR, BELL-BELL-BELL, CHERRY-CHERRY-CHERRY). 2-cherries and respin disabled.
  - **🎰 Classic Arcade + Bonus**: 3-of-a-kind + 2 Cherries (2x) + Lucky Respin enabled.
  - **🔥 High Roller Mania**: 250x, 100x, 50x, 20x multipliers for high stakes play.
- 💾 **Live Application & Storage**: Settings are saved to browser storage and update the in-game paytable modal immediately.

---

### 3. Edge Case Handling & Polish
- 🛡️ **Mid-Spin Bet Lock**: Bet amounts and rules cannot be modified while reels are in motion.
- 🔒 **Free Spins Wager Freeze**: Bet modification is locked during Free Spins mode to preserve the triggering wager.
- 🪙 **Bankruptcy Recovery**: If coins drop to 0, players can press **R** or click their balance display to instantly reload 1,000 complimentary coins.
- ⌨️ **Full Keyboard Accessibility**:
  - `Space` / `Enter`: Spin the reels or progress onboarding.
  - `B`: Toggle Bet Settings modal.
  - `P`: Toggle Paytable modal.
  - `M`: Toggle Sound on/off.
  - `A`: Toggle Auto Spin.
  - `R`: Reload free coins when bankrupt.
  - `Escape`: Close any open modal or dismiss walkthrough.
- 📱 **Non-Overlapping Pixel-Perfect Layout**: Redesigned console bar separates Coin Balance, Last Win, Total Bet steppers, Bet Settings, Paytable, Sound, and Tour buttons with clean dividers.
- 🔊 **Safe WebAudio Unlock**: Auto-unlocks audio context on initial keypress or touch without throwing unhandled browser autoplay rejections.

---

### 2. Default Winning Combinations & Payout Table

| Symbol | Combination | Multiplier | Bonus Reward | Status (Assignment Default) |
| :---: | :---: | :---: | :---: | :---: |
| **7** | 3x Seven | **100x Bet** | ★ Grand Jackpot Celebration ★ | ✅ Enabled |
| **BAR** | 3x Bar | **50x Bet** | ✦ Mega Win Celebration ✦ | ✅ Enabled |
| **BELL** | 3x Bell | **25x Bet** | 🔔 +5 Free Spins (2x Multiplier) 🔔 | ✅ Enabled |
| **CHERRY** | 3x Cherry | **10x Bet** | 🍒 Sweet Win 🍒 | ✅ Enabled |
| **CHERRY** | Any 2 Cherries | **2x Bet** | 🍒 Minor Win 🍒 | ⏸️ Disabled (Manual Toggle) |
| **ANY** | Matching Pair | **Respin** | ⚡ Lucky Respin (Hold & Spin) ⚡ | ⏸️ Disabled (Manual Toggle) |

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
