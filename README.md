# 🎰 Classic Slot Machine - Unity & WebGL Game

A production-grade, highly polished 3-reel classic slot machine game built with **Unity** and exported as a fully playable **WebGL build**. 

Designed with clean **Object-Oriented Programming (OOP)**, cryptographically secure **Random Number Generation (RNG)**, realistic **spring-damped reel physics**, interactive mechanical **lever animations**, retro **sound effects**, and exciting **bonus features** (Lucky Respin & Free Spins Frenzy).

---

## 📊 Presentation Deck & Engineering Walkthrough (PDF & Interactive Slides)

A complete **12-Slide Widescreen (16:9 Full-HD) Presentation Deck** covering **Part I: About the Game** and **Part II: How We Built It** is included directly in the repository:

- 📄 **[`Slot_Machine_Presentation.pdf`](./Slot_Machine_Presentation.pdf)** — Ready-to-present 12-slide Full-HD (`1920×1080`) PDF presentation with embedded game sprites, kinematic physics curves, probability charts, and system architecture diagrams.
- 🖥️ **[`Presentation.html`](./Presentation.html)** — Interactive browser slide deck with one-click **Print / Save as PDF** support.

### Slide Deck Outline
| Slide | Section | Topic | Key Highlights |
| :---: | :--- | :--- | :--- |
| **01** | **Part I: About The Game** | **Title & Executive Overview** | 96.5% RTP CSPRNG, 60 FPS spring-damped physics, Unity C# + WebGL parity, 4 core symbols |
| **02** | **Part I: About The Game** | **Core Loop & Spotlight Onboarding** | 1,000-coin bankroll, 4-step spotlight guided tour, full keyboard shortcuts (`Space`, `B`, `A`, `M`, `R`, `Esc`) |
| **03** | **Part I: About The Game** | **Dynamic Winning Rule Engine** | Strict 3-of-a-kind assignment default, Classic Arcade & High Roller presets, custom rule builder |
| **04** | **Part I: About The Game** | **Bonus Mechanics** | ⚡ **Lucky Respin** (Hold & Spin 3rd reel with golden locks) & 🔔 **Free Spins Frenzy** (5 spins @ 2x multiplier) |
| **05** | **Part I: About The Game** | **Interactive 3D Lever & Celebrations** | Click-or-drag mechanical lever, ratchet clicks, cabinet micro-jolt, 120-particle jackpot fountain |
| **06** | **Part II: How We Built It** | **Clean OOP System Architecture** | 4 decoupled C# namespaces (`Core`, `Reels`, `Bonus`, `UI`) & deterministic finite state machine |
| **07** | **Part II: How We Built It** | **Cryptographic RNG & 96.5% RTP Math** | Unbiased 32-bit rejection sampling (`RNGCryptoServiceProvider` / `crypto.getRandomValues`) & pre-spin locking |
| **08** | **Part II: How We Built It** | **6-Phase Continuous Drum Tape Physics** | Virtual strip map (`Map<step, symbol>`), upward stepper kickback, 3D cylindrical foreshortening & cosine bounce |
| **09** | **Part II: How We Built It** | **3D Lever Spring-Damper ODE** | Second-order differential equation ($a = -k x - c v$, $k=220, c=16.5$) & coupled cabinet micro-jolt ($k=340, c=24$) |
| **10** | **Part II: How We Built It** | **Audio Synthesis & `file://` Pipeline** | 8 synthesized WAVs embedded in Base64 RAM buffers + real-time WebAudio procedural oscillators |
| **11** | **Part II: How We Built It** | **UI Engineering & Edge-Case Guards** | 235px zero-overlap console bar, Safari centering fixes, mid-spin/free-spin wager locks & timer cleanup |
| **12** | **Part II: How We Built It** | **Engineering Commit Timeline & Run Guide** | 14 atomic commits (`d8eabae` → `b6cb09a`) from initial Unity setup to 3D physics & GitHub deployment |

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
When launching the game for the first time on any device, an **interactive 4-step spotlight onboarding walkthrough** automatically introduces the player to the game:
- 💡 **Spotlight Focus Frame**: Dynamically tracks and illuminates the active feature on the arcade cabinet while dimming the background.
- 🎯 **Step-by-Step Guidance**:
  1. **The 3-Reel Payline**: Explains the center payline win conditions and default strict rules.
  2. **Bet Settings & Rules Configurator**: Demonstrates wager adjustments and the prominent **`⚙️ BET SETTINGS`** button on the console bar.
  3. **Interactive Lever & Spin**: Shows how to pull the mechanical lever or press Spacebar/Spin.
  4. **Bankroll & Ready to Play**: Welcomes the player with 1,000 complimentary coins and sets them ready to play.

---

### 2. Manual Bet Settings & Winning Rule Engine
The previous `MAX BET` and `PAYTABLE` clutter has been replaced with the streamlined, extra-spacious **`⚙️ BET SETTINGS`** button directly on the main console bar:

- ⚙️ **Access Bet Settings**: Click the prominent **`⚙️ BET SETTINGS`** button directly on the bottom console, or click the **TOTAL BET** display.
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
  - `P`: Toggle Bet Settings modal (rules & payouts).
  - `M`: Toggle Sound on/off.
  - `A`: Toggle Auto Spin.
  - `R`: Reload free coins when bankrupt.
  - `Escape`: Close any open modal or dismiss walkthrough.
- 📱 **Spacious Zero-Overlap Console Bar**: Clean, elegant separation between Coin Balance, Last Win, Total Bet steppers, Bet Settings (235px wide), and Sound Toggle.
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

## 📐 Thought Process, System Architecture & How We Built It

### 1. Clean OOP Separation of Concerns (4 Decoupled Subsystems)
The codebase is architected into four modular namespaces in Unity C# ([`Assets/Scripts/`](./Assets/Scripts/)) and mirrored 1:1 in the standalone WebGL runtime ([`Build/WebGL/game.js`](./Build/WebGL/game.js)):
- **`SlotMachine.Core`**:
  - [`GameManager.cs`](./Assets/Scripts/Core/GameManager.cs): Central finite state machine (`Idle` → `Spinning` → `Evaluating` → `Respinning` / `Celebrating`) and player bankroll manager.
  - [`RNGService.cs`](./Assets/Scripts/Core/RNGService.cs): Cryptographically fair random outcome generator using rejection sampling to eliminate 32-bit modulo bias.
  - [`PayoutTable.cs`](./Assets/Scripts/Core/PayoutTable.cs): Dynamic rule evaluation engine supporting live rule toggling, custom multipliers, and custom rule insertion.
  - [`SoundManager.cs`](./Assets/Scripts/Core/SoundManager.cs): Multi-channel audio playback controller.
- **`SlotMachine.Reels`**:
  - [`SlotReelManager.cs`](./Assets/Scripts/Reels/SlotReelManager.cs): Coordinates 3 reel drums, staggered stop delays (`1.25s`, `+0.45s`, `+0.45s`), suspense extensions (`1.75x` delay when Reel 1 and Reel 2 match), and single-reel isolation for Lucky Respins.
  - [`ReelStrip.cs`](./Assets/Scripts/Reels/ReelStrip.cs) & [`ReelSymbol.cs`](./Assets/Scripts/Reels/ReelSymbol.cs): Symbol object pooling, continuous vertical tape wrapping, cubic deceleration, and damped cosine bounce.
- **`SlotMachine.Bonus`**:
  - [`BonusManager.cs`](./Assets/Scripts/Bonus/BonusManager.cs): Manages the Free Spins state counter, active `2x` payout multiplier, and Lucky Respin reel locking.
- **`SlotMachine.UI`**:
  - [`UIManager.cs`](./Assets/Scripts/UI/UIManager.cs), [`LeverController.cs`](./Assets/Scripts/UI/LeverController.cs), & [`WinCelebrationUI.cs`](./Assets/Scripts/UI/WinCelebrationUI.cs): HUD counters, pointer/drag lever interaction, and quadratic ease-out coin count-up celebration tickers.

### 2. Unbiased Cryptographic RNG & Pre-Spin Outcome Locking
- Following regulated casino gaming standards, the outcome array `[R1, R2, R3]` is drawn **before** the reels begin accelerating, making client-side stopper timing manipulation impossible.
- Uses `RNGCryptoServiceProvider` in Unity C# and `window.crypto.getRandomValues(new Uint32Array(1))` in WebGL with rejection sampling (`maxValid = (1L << 32) - ((1L << 32) % diff)`) to ensure a mathematically uniform draw before applying the `10% / 20% / 30% / 40%` symbol weights.

### 3. Continuous Virtual Drum Tape & 6-Phase Reel Physics
- Each reel maintains a persistent virtual tape (`Map<step, symbol>`) with a non-repeating neighbor generator so symbols scroll seamlessly without popping or visual seams.
- **6-Phase Kinematics**:
  1. **Stepper-Motor Upward Kickback (75ms)**: $\text{pos}(t) = \text{startPos} - 0.13 \sin(\pi t / 0.075)$
  2. **Rapid Downward Acceleration**: Accelerates at $68\text{ symbols/s}^2$ up to terminal velocity ($24.5\text{ symbols/s}$).
  3. **Velocity-Proportional Motion Blur**: Multi-pass vertical alpha trails stretch proportionally to drum speed.
  4. **6-Symbol Landing Runway**: Target symbol is stamped 6 steps ahead (`Math.ceil(position) + 6`) and approached via cubic ease-out $1 - (1 - u)^{2.5}$.
  5. **Physical Overshoot (+20.5px)**: Drops past the payline, firing the mechanical latch thud (`reel_stop.wav`) and a physical cabinet micro-jolt.
  6. **Damped Cosine Rebound**: Springs back to dead-center alignment via $y(v) = \text{targetStep} + A \cos\left(\frac{\pi}{2}v\right)(1 - 0.25v)$.
- **3D Cylindrical Drum Foreshortening**: Symbols near the top and bottom curve of the reel window compress vertically ($\text{cylSquashY} = \max(0.82, 1.0 - 0.12 \cdot d^2)$) beneath a 6-stop cylindrical shadow and specular glass gradient.

### 4. Second-Order ODE Lever Spring-Damper & Cabinet Micro-Jolt
- The mechanical lever supports both instant click-to-pull and continuous downward pointer dragging across a `125px` vertical stroke, firing sawtooth ratchet clicks every `16%` of travel.
- Releasing the lever integrates a second-order spring-damper differential equation every frame:
  $$a_{\text{lever}} = -k \cdot x - c \cdot v \quad (k = 220,\; c = 16.5)$$
  allowing a natural top rebound overshoot down to `leverPos = -0.08` while transferring vertical impulse shocks to the cabinet frame ($k_{\text{cabinet}} = 340,\; c_{\text{cabinet}} = 24$).

### 5. Zero-Server Audio Synthesis & In-Memory Base64 Pipeline
- To support both local HTTP servers and direct `file://` double-clicking (where browsers block `fetch()` due to CORS), all 8 synthesized arcade `.wav` files are embedded in `AUDIO_BASE64` inside [`Build/WebGL/game.js`](./Build/WebGL/game.js) and decoded directly in RAM via `AudioContext.decodeAudioData()`, paired with real-time WebAudio `OscillatorNode` synthesis for high-frequency gear and ratchet ticks.

---

## 📜 License
Developed as part of the Slot Game Assignment. All assets provided are used for demonstration and testing purposes.
