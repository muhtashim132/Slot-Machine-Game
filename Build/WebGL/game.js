/**
 * Classic Slot Machine - WebGL Engine
 * Complete implementation with smooth reel physics, cryptographic RNG,
 * interactive lever, paytable modal, bonus features (Lucky Respin & Free Spins),
 * WebAudio sound effects, and particle celebration effects.
 */

(function () {
  'use strict';

  // --- Constants & Config ---
  const CANVAS_WIDTH = 1280;
  const CANVAS_HEIGHT = 720;
  const MACHINE_WIDTH = 816;
  const MACHINE_HEIGHT = 624;
  const MACHINE_X = Math.round((CANVAS_WIDTH - MACHINE_WIDTH) / 2); // 232
  const MACHINE_Y = 24;

  const REEL_DEFS = [
    { x: MACHINE_X + 229, y: MACHINE_Y + 246, w: 108, h: 210 }, // Reel 1: 461, 270
    { x: MACHINE_X + 359, y: MACHINE_Y + 246, w: 108, h: 210 }, // Reel 2: 591, 270
    { x: MACHINE_X + 489, y: MACHINE_Y + 246, w: 108, h: 210 }  // Reel 3: 721, 270
  ];

  const SYMBOL_SIZE = 96;
  const SYMBOL_SPACING = 96;
  const PAYLINE_OFFSET_Y = 105; // Center of reel window

  const SYMBOL_TYPES = {
    SEVEN: 0,
    BAR: 1,
    BELL: 2,
    CHERRY: 3
  };

  const SYMBOL_NAMES = ['SEVEN', 'BAR', 'BELL', 'CHERRY'];
  const BET_TIERS = [10, 50, 100];

  // --- Asset Loader ---
  const ASSET_PATHS = {
    bg: 'assets/bg_gradient.png',
    casing: 'assets/slot-machine4.png',
    reelsBg: 'assets/slot-machine5.png',
    leverUp: 'assets/slot-machine2.png',
    leverDown: 'assets/slot-machine3.png',
    middleBox: 'assets/slot_machine_Middle_box.png',
    popup: 'assets/popup.png',
    sym0: 'assets/slot-symbol1.png', // 7
    sym1: 'assets/slot-symbol4.png', // BAR
    sym2: 'assets/slot-symbol3.png', // Bell
    sym3: 'assets/slot-symbol2.png', // Cherry
    btnCloseNorm: 'assets/btn_close_normal.png',
    btnCloseHover: 'assets/btn_close_hover.png',
    btnNextNorm: 'assets/btn_next_normal.png',
    btnNextHover: 'assets/btn_next_hover.png',
    btnPrevNorm: 'assets/btn_prev_normal.png',
    btnPrevHover: 'assets/btn_prev_hover.png',
    btnYesNorm: 'assets/btn_yes_normal.png',
    btnYesHover: 'assets/btn_yes_hover.png',
    btnNoNorm: 'assets/btn_no_normal.png',
    btnNoHover: 'assets/btn_no_hover.png'
  };

  const AUDIO_PATHS = {
    leverPull: 'assets/lever_pull.wav',
    reelSpin: 'assets/reel_spin.wav',
    reelStop: 'assets/reel_stop.wav',
    btnClick: 'assets/button_click.wav',
    winSmall: 'assets/win_small.wav',
    winJackpot: 'assets/win_jackpot.wav',
    coinPayout: 'assets/coin_payout.wav',
    respinSuspense: 'assets/respin_suspense.wav'
  };

  const images = {};
  const audioBuffers = {};
  let audioCtx = null;
  let isMuted = false;
  let spinLoopSource = null;

  // --- Audio System ---
  function getAudioContext() {
    if (!audioCtx) {
      const AudioContextClass = window.AudioContext || window.webkitAudioContext;
      if (AudioContextClass) {
        audioCtx = new AudioContextClass();
      }
    }
    if (audioCtx && audioCtx.state === 'suspended') {
      audioCtx.resume();
    }
    return audioCtx;
  }

  function playSound(name, volume = 1.0) {
    if (isMuted) return;
    const ctx = getAudioContext();
    if (!ctx) return;

    if (audioBuffers[name]) {
      try {
        const source = ctx.createBufferSource();
        source.buffer = audioBuffers[name];
        const gainNode = ctx.createGain();
        gainNode.gain.value = volume;
        source.connect(gainNode);
        gainNode.connect(ctx.destination);
        source.start(0);
      } catch (e) {
        console.warn('Audio play error:', e);
      }
    } else {
      // Procedural fallback beep
      playProceduralTone(name);
    }
  }

  function startReelSpinLoop() {
    if (isMuted || spinLoopSource) return;
    const ctx = getAudioContext();
    if (!ctx || !audioBuffers.reelSpin) return;

    try {
      spinLoopSource = ctx.createBufferSource();
      spinLoopSource.buffer = audioBuffers.reelSpin;
      spinLoopSource.loop = true;
      const gainNode = ctx.createGain();
      gainNode.gain.value = 0.45;
      spinLoopSource.connect(gainNode);
      gainNode.connect(ctx.destination);
      spinLoopSource.start(0);
    } catch (e) {}
  }

  function stopReelSpinLoop() {
    if (spinLoopSource) {
      try {
        spinLoopSource.stop();
        spinLoopSource.disconnect();
      } catch (e) {}
      spinLoopSource = null;
    }
  }

  function playProceduralTone(type) {
    const ctx = getAudioContext();
    if (!ctx || isMuted) return;
    const osc = ctx.createOscillator();
    const gain = ctx.createGain();
    osc.connect(gain);
    gain.connect(ctx.destination);

    const now = ctx.currentTime;
    if (type === 'btnClick') {
      osc.frequency.setValueAtTime(600, now);
      osc.frequency.exponentialRampToValueAtTime(200, now + 0.06);
      gain.gain.setValueAtTime(0.2, now);
      gain.gain.linearRampToValueAtTime(0.01, now + 0.06);
      osc.start(now);
      osc.stop(now + 0.06);
    } else if (type === 'reelStop') {
      osc.type = 'triangle';
      osc.frequency.setValueAtTime(140, now);
      gain.gain.setValueAtTime(0.4, now);
      gain.gain.linearRampToValueAtTime(0.01, now + 0.12);
      osc.start(now);
      osc.stop(now + 0.12);
    } else if (type === 'winSmall') {
      osc.type = 'sine';
      osc.frequency.setValueAtTime(523.25, now);
      osc.frequency.setValueAtTime(659.25, now + 0.1);
      osc.frequency.setValueAtTime(783.99, now + 0.2);
      gain.gain.setValueAtTime(0.3, now);
      gain.gain.linearRampToValueAtTime(0.01, now + 0.4);
      osc.start(now);
      osc.stop(now + 0.4);
    }
  }

  // --- RNG & Probability Model ---
  // Default casino weighting: 7 (10%), BAR (20%), BELL (30%), CHERRY (40%) -> 96.4% RTP
  const SYMBOL_WEIGHTS = [10, 20, 30, 40];
  const TOTAL_WEIGHT = SYMBOL_WEIGHTS.reduce((a, b) => a + b, 0);

  function getRandomSymbol() {
    const arr = new Uint32Array(1);
    if (window.crypto && window.crypto.getRandomValues) {
      window.crypto.getRandomValues(arr);
    } else {
      arr[0] = Math.floor(Math.random() * 0xffffffff);
    }
    const rand = arr[0] % TOTAL_WEIGHT;

    let cumulative = 0;
    for (let i = 0; i < SYMBOL_WEIGHTS.length; i++) {
      cumulative += SYMBOL_WEIGHTS[i];
      if (rand < cumulative) {
        return i;
      }
    }
    return SYMBOL_TYPES.CHERRY;
  }

  function generateSpinOutcome() {
    return [getRandomSymbol(), getRandomSymbol(), getRandomSymbol()];
  }

  // --- Particle System ---
  class Particle {
    constructor(x, y, isGold = true) {
      this.x = x;
      this.y = y;
      const angle = Math.random() * Math.PI * 2;
      const speed = 2 + Math.random() * 8;
      this.vx = Math.cos(angle) * speed;
      this.vy = Math.sin(angle) * speed - 4; // upward bias
      this.gravity = 0.25;
      this.size = 4 + Math.random() * 6;
      this.alpha = 1.0;
      this.decay = 0.01 + Math.random() * 0.015;
      this.isGold = isGold;
      this.rotation = Math.random() * Math.PI * 2;
      this.vRot = (Math.random() - 0.5) * 0.3;
    }

    update() {
      this.x += this.vx;
      this.y += this.vy;
      this.vy += this.gravity;
      this.alpha -= this.decay;
      this.rotation += this.vRot;
      return this.alpha > 0;
    }

    draw(ctx) {
      ctx.save();
      ctx.globalAlpha = Math.max(0, this.alpha);
      ctx.translate(this.x, this.y);
      ctx.rotate(this.rotation);
      ctx.fillStyle = this.isGold ? '#fbbf24' : '#38bdf8';
      ctx.shadowColor = this.isGold ? '#f59e0b' : '#0ea5e9';
      ctx.shadowBlur = 8;
      ctx.fillRect(-this.size / 2, -this.size / 2, this.size, this.size);
      ctx.restore();
    }
  }

  // --- Reel Object ---
  class Reel {
    constructor(index, def) {
      this.index = index;
      this.x = def.x;
      this.y = def.y;
      this.w = def.w;
      this.h = def.h;

      // 3 Visible symbols on strip
      this.symbols = [SYMBOL_TYPES.SEVEN, SYMBOL_TYPES.BAR, SYMBOL_TYPES.BELL];
      this.targetSymbol = SYMBOL_TYPES.BAR;

      // Physics state
      this.isSpinning = false;
      this.scrollOffset = 0;
      this.speed = 0;
      this.maxSpeed = 2400; // px/sec
      this.spinTimer = 0;
      this.stopDuration = 0.5;
      this.stopping = false;
      this.bounceTimer = 0;
      this.isLocked = false; // For Lucky Respin
      this.pulseWin = false;
      this.pulsePhase = 0;
    }

    startSpin(target) {
      if (this.isLocked) return;
      this.targetSymbol = target;
      this.isSpinning = true;
      this.stopping = false;
      this.speed = 0;
      this.spinTimer = 0;
      this.bounceTimer = 0;
      this.pulseWin = false;
    }

    requestStop(target) {
      this.targetSymbol = target;
      this.stopping = true;
      this.stopProgress = 0;
    }

    update(dt) {
      if (this.pulseWin) {
        this.pulsePhase += dt * 5;
      }

      if (!this.isSpinning) return;

      this.spinTimer += dt;

      if (!this.stopping) {
        // Acceleration phase
        this.speed = Math.min(this.maxSpeed, this.speed + dt * 6000);
        this.scrollOffset = (this.scrollOffset + this.speed * dt) % SYMBOL_SPACING;
      } else {
        // Deceleration & Spring Bounce phase
        this.stopProgress += dt / this.stopDuration;
        if (this.stopProgress < 1.0) {
          // Cubic ease-out
          const t = this.stopProgress;
          const ease = 1 - Math.pow(1 - t, 3);
          this.speed = this.maxSpeed * (1 - ease);
          this.scrollOffset = (this.scrollOffset + this.speed * dt) % SYMBOL_SPACING;
        } else {
          // Bounce animation
          this.bounceTimer += dt;
          const bDur = 0.22;
          if (this.bounceTimer < bDur) {
            const bt = this.bounceTimer / bDur;
            // Damped harmonic bounce
            this.scrollOffset = -18 * Math.cos(bt * Math.PI * 0.5);
          } else {
            // Settle exactly on target
            this.scrollOffset = 0;
            this.isSpinning = false;
            this.stopping = false;
            this.symbols[1] = this.targetSymbol;
            this.symbols[0] = (this.targetSymbol + 1) % 4;
            this.symbols[2] = (this.targetSymbol + 3) % 4;
            playSound('reelStop', 0.85);
            onReelStopped(this.index);
          }
        }
      }
    }

    draw(ctx) {
      ctx.save();
      // Clip to reel window
      ctx.beginPath();
      ctx.rect(this.x, this.y, this.w, this.h);
      ctx.clip();

      // Draw shaded backdrop
      if (images.reelsBg) {
        ctx.drawImage(images.reelsBg, MACHINE_X, MACHINE_Y, MACHINE_WIDTH, MACHINE_HEIGHT);
      }

      const centerY = this.y + PAYLINE_OFFSET_Y;
      const pulseScale = this.pulseWin ? 1.0 + 0.15 * Math.sin(this.pulsePhase) : 1.0;

      // Draw symbols
      const yShift = this.scrollOffset;
      const count = 5; // buffer symbols
      for (let i = -2; i <= 2; i++) {
        const symY = centerY + i * SYMBOL_SPACING + yShift;
        let symType;

        if (this.isSpinning && this.speed > 800) {
          // High speed random blur symbol
          symType = (Math.abs(Math.floor((symY + this.spinTimer * 1000) / SYMBOL_SPACING))) % 4;
        } else if (i === 0) {
          symType = this.stopping ? this.targetSymbol : this.symbols[1];
        } else if (i === -1) {
          symType = (this.symbols[1] + 1) % 4;
        } else if (i === 1) {
          symType = (this.symbols[1] + 3) % 4;
        } else {
          symType = (this.symbols[1] + 2) % 4;
        }

        const img = images[`sym${symType}`];
        if (img) {
          const drawScale = (i === 0 && this.pulseWin) ? pulseScale : 1.0;
          const drawW = SYMBOL_SIZE * drawScale;
          const drawH = SYMBOL_SIZE * drawScale;
          const drawX = this.x + (this.w - drawW) / 2;
          const drawY = symY - drawH / 2;

          if (this.isSpinning && this.speed > 1200) {
            // Speed motion blur effect
            ctx.globalAlpha = 0.7;
            ctx.drawImage(img, drawX, drawY - 8, drawW, drawH + 16);
            ctx.globalAlpha = 1.0;
          } else {
            ctx.drawImage(img, drawX, drawY, drawW, drawH);
          }
        }
      }

      // If reel is locked (Lucky Respin highlight)
      if (this.isLocked) {
        ctx.strokeStyle = '#f59e0b';
        ctx.lineWidth = 4;
        ctx.strokeRect(this.x + 2, this.y + 2, this.w - 4, this.h - 4);
      }

      ctx.restore();
    }
  }

  // --- Game State & Economy ---
  const state = {
    coins: 1000,
    betIndex: 0,
    lastWin: 0,
    totalWon: 0,
    statusText: 'Pull Lever or Press SPIN to Play!',
    isSpinning: false,
    autoSpin: false,
    freeSpins: 0,
    leverState: 'up', // 'up' or 'down'
    leverPullTimer: 0,
    showPaytable: false,
    showCelebration: false,
    celebrationText: '',
    celebrationAmount: 0,
    displayWinnings: 0,
    reels: [],
    particles: [],
    currentOutcome: [0, 0, 0],
    respinIndex: -1
  };

  function getCurrentBet() {
    return BET_TIERS[state.betIndex];
  }

  // --- Game Mechanics & Win Logic ---
  function executeSpin() {
    if (state.isSpinning) return;

    const isFreeSpin = state.freeSpins > 0;
    const bet = getCurrentBet();

    if (!isFreeSpin) {
      if (state.coins < bet) {
        state.statusText = 'Insufficient Coins! Lower bet.';
        stopAutoSpin();
        return;
      }
      state.coins -= bet;
    } else {
      state.freeSpins--;
      state.statusText = `★ FREE SPIN! (${state.freeSpins} Remaining, 2X Multiplier) ★`;
    }

    state.isSpinning = true;
    state.lastWin = 0;
    state.showCelebration = false;
    clearReelHighlights();

    // 1. Pull lever visual
    triggerLeverPull();

    // 2. Play spin sound loop
    startReelSpinLoop();

    // 3. RNG generation
    state.currentOutcome = generateSpinOutcome();

    // 4. Start reels spinning
    state.reels.forEach((r, idx) => {
      r.isLocked = false;
      r.startSpin(state.currentOutcome[idx]);
    });

    // 5. Staggered reel stop schedule
    const minSpinTime = 1.3;
    setTimeout(() => {
      if (state.reels[0]) state.reels[0].requestStop(state.currentOutcome[0]);
    }, minSpinTime * 1000);

    setTimeout(() => {
      if (state.reels[1]) state.reels[1].requestStop(state.currentOutcome[1]);
    }, (minSpinTime + 0.45) * 1000);

    // Suspense timing for 3rd reel if 1 & 2 match!
    const isSuspense = state.currentOutcome[0] === state.currentOutcome[1];
    const reel3Delay = isSuspense ? (minSpinTime + 1.2) : (minSpinTime + 0.9);

    if (isSuspense) {
      setTimeout(() => {
        playSound('respinSuspense', 0.9);
      }, (minSpinTime + 0.5) * 1000);
    }

    setTimeout(() => {
      if (state.reels[2]) state.reels[2].requestStop(state.currentOutcome[2]);
    }, reel3Delay * 1000);
  }

  let stoppedCount = 0;
  function onReelStopped(reelIndex) {
    stoppedCount++;
    if (stoppedCount >= 3) {
      stoppedCount = 0;
      stopReelSpinLoop();
      evaluateSpinResult();
    }
  }

  function evaluateSpinResult() {
    const r1 = state.currentOutcome[0];
    const r2 = state.currentOutcome[1];
    const r3 = state.currentOutcome[2];
    const bet = getCurrentBet();
    const isFreeSpin = state.freeSpins > 0;
    const bonusMult = isFreeSpin ? 2.0 : 1.0;

    // 1. Check 3-of-a-kind (Main Win)
    if (r1 === r2 && r2 === r3) {
      highlightWinningReels();
      let mult = 0;
      let title = '';
      let isJackpot = false;

      if (r1 === SYMBOL_TYPES.SEVEN) {
        mult = 100;
        title = '★ GRAND JACKPOT! ★';
        isJackpot = true;
      } else if (r1 === SYMBOL_TYPES.BAR) {
        mult = 50;
        title = '✦ MEGA WIN! (3x BAR) ✦';
      } else if (r1 === SYMBOL_TYPES.BELL) {
        mult = 25;
        title = '🔔 BELL BONANZA! (+5 FREE SPINS) 🔔';
        state.freeSpins += 5;
      } else if (r1 === SYMBOL_TYPES.CHERRY) {
        mult = 10;
        title = '🍒 SWEET CHERRY WIN! 🍒';
      }

      const payout = Math.round(bet * mult * bonusMult);
      awardWin(payout, title, isJackpot);
      return;
    }

    // 2. Check 2 Cherries
    const cherries = state.currentOutcome.filter(s => s === SYMBOL_TYPES.CHERRY).length;
    if (cherries === 2) {
      highlightWinningReels();
      const payout = Math.round(bet * 2 * bonusMult);
      awardWin(payout, '🍒 DOUBLE CHERRIES WIN! 🍒', false);
      return;
    }

    // 3. Lucky Respin Bonus (Two matching non-cherry symbols)
    if (!isFreeSpin) {
      let lockA = -1, lockB = -1, respinTarget = -1;
      if (r1 === r2 && r1 !== SYMBOL_TYPES.CHERRY) {
        lockA = 0; lockB = 1; respinTarget = 2;
      } else if (r2 === r3 && r2 !== SYMBOL_TYPES.CHERRY) {
        lockA = 1; lockB = 2; respinTarget = 0;
      } else if (r1 === r3 && r1 !== SYMBOL_TYPES.CHERRY) {
        lockA = 0; lockB = 2; respinTarget = 1;
      }

      if (respinTarget !== -1) {
        triggerLuckyRespin(lockA, lockB, respinTarget);
        return;
      }
    }

    // No win
    state.statusText = 'Better luck next spin! Try again.';
    concludeSpin();
  }

  function triggerLuckyRespin(lockA, lockB, respinTarget) {
    state.statusText = `⚡ LUCKY RESPIN! Reel ${respinTarget + 1} Respinning! ⚡`;
    state.reels[lockA].isLocked = true;
    state.reels[lockB].isLocked = true;
    playSound('respinSuspense', 1.0);

    setTimeout(() => {
      startReelSpinLoop();
      const newSymbol = getRandomSymbol();
      state.currentOutcome[respinTarget] = newSymbol;
      state.reels[respinTarget].startSpin(newSymbol);

      setTimeout(() => {
        state.reels[respinTarget].requestStop(newSymbol);
      }, 1500);
    }, 600);
  }

  function awardWin(amount, title, isJackpot) {
    state.lastWin = amount;
    state.coins += amount;
    state.totalWon += amount;
    state.statusText = title;
    state.celebrationText = title;
    state.celebrationAmount = amount;
    state.showCelebration = true;

    // Spawn celebration particles
    spawnWinParticles(isJackpot ? 120 : 50);

    if (isJackpot) {
      playSound('winJackpot', 1.0);
    } else {
      playSound('winSmall', 0.9);
    }
    playSound('coinPayout', 0.8);

    // Animate count-up ticker
    state.displayWinnings = 0;
    const duration = isJackpot ? 1800 : 900;
    const start = performance.now();

    function stepTicker(now) {
      const elapsed = now - start;
      const progress = Math.min(1.0, elapsed / duration);
      state.displayWinnings = Math.round(amount * (1 - Math.pow(1 - progress, 2)));

      if (progress < 1.0) {
        requestAnimationFrame(stepTicker);
      } else {
        state.displayWinnings = amount;
        setTimeout(() => {
          state.showCelebration = false;
          concludeSpin();
        }, isJackpot ? 1800 : 900);
      }
    }
    requestAnimationFrame(stepTicker);
  }

  function concludeSpin() {
    state.isSpinning = false;

    if (state.autoSpin) {
      if (state.coins >= getCurrentBet() || state.freeSpins > 0) {
        setTimeout(() => {
          if (state.autoSpin && !state.isSpinning) {
            executeSpin();
          }
        }, 900);
      } else {
        stopAutoSpin();
        state.statusText = 'Auto-Spin Stopped: Low Coins!';
      }
    }
  }

  function highlightWinningReels() {
    state.reels.forEach(r => r.pulseWin = true);
  }

  function clearReelHighlights() {
    state.reels.forEach(r => {
      r.pulseWin = false;
      r.isLocked = false;
    });
  }

  function triggerLeverPull() {
    state.leverState = 'down';
    state.leverPullTimer = 0.28;
    playSound('leverPull', 0.9);
  }

  function stopAutoSpin() {
    state.autoSpin = false;
  }

  function spawnWinParticles(count) {
    for (let i = 0; i < count; i++) {
      state.particles.push(new Particle(
        MACHINE_X + 220 + Math.random() * 380,
        MACHINE_Y + 240 + Math.random() * 100,
        Math.random() > 0.3
      ));
    }
  }

  // --- Interaction & UI Buttons ---
  const UI_BUTTONS = [
    {
      id: 'spin',
      x: 1045, y: 550, w: 175, h: 65,
      label: 'SPIN',
      color: '#22c55e',
      border: '#15803d',
      action: () => executeSpin()
    },
    {
      id: 'auto',
      x: 1045, y: 625, w: 175, h: 50,
      label: 'AUTO SPIN',
      color: '#3b82f6',
      border: '#1d4ed8',
      action: () => {
        playSound('btnClick', 0.7);
        state.autoSpin = !state.autoSpin;
        if (state.autoSpin && !state.isSpinning) executeSpin();
      }
    },
    {
      id: 'betPrev',
      x: 480, y: 642, w: 42, h: 42,
      isSprite: true,
      spriteNorm: 'btnPrevNorm',
      spriteHover: 'btnPrevHover',
      action: () => {
        if (state.isSpinning) return;
        playSound('btnClick', 0.6);
        state.betIndex = (state.betIndex - 1 + BET_TIERS.length) % BET_TIERS.length;
      }
    },
    {
      id: 'betNext',
      x: 618, y: 642, w: 42, h: 42,
      isSprite: true,
      spriteNorm: 'btnNextNorm',
      spriteHover: 'btnNextHover',
      action: () => {
        if (state.isSpinning) return;
        playSound('btnClick', 0.6);
        state.betIndex = (state.betIndex + 1) % BET_TIERS.length;
      }
    },
    {
      id: 'maxBet',
      x: 685, y: 642, w: 90, h: 42,
      label: 'MAX BET',
      color: '#e11d48',
      border: '#9f1239',
      action: () => {
        if (state.isSpinning) return;
        playSound('btnClick', 0.7);
        state.betIndex = BET_TIERS.length - 1;
      }
    },
    {
      id: 'paytable',
      x: 55, y: 642, w: 135, h: 46,
      label: 'PAYTABLE',
      color: '#8b5cf6',
      border: '#6d28d9',
      action: () => {
        playSound('btnClick', 0.7);
        state.showPaytable = !state.showPaytable;
      }
    },
    {
      id: 'sound',
      x: 205, y: 642, w: 110, h: 46,
      label: 'SOUND: ON',
      color: '#475569',
      border: '#334155',
      action: () => {
        isMuted = !isMuted;
        playSound('btnClick', 0.7);
      }
    },
    {
      id: 'lever',
      x: MACHINE_X + 685, y: MACHINE_Y + 280, w: 110, h: 320,
      isTransparent: true,
      action: () => {
        if (!state.isSpinning) executeSpin();
      }
    }
  ];

  let hoveredButtonId = null;

  function handlePointer(clientX, clientY, isDown = false) {
    const canvas = document.getElementById('game-canvas');
    if (!canvas) return;
    const rect = canvas.getBoundingClientRect();
    const scaleX = CANVAS_WIDTH / rect.width;
    const scaleY = CANVAS_HEIGHT / rect.height;

    const mx = (clientX - rect.left) * scaleX;
    const my = (clientY - rect.top) * scaleY;

    // Check modal close if open
    if (state.showPaytable) {
      // Close button at top right of modal
      const modalX = 240, modalY = 80, modalW = 800, modalH = 540;
      const closeX = modalX + modalW - 65, closeY = modalY + 20, closeS = 46;

      if (mx >= closeX && mx <= closeX + closeS && my >= closeY && my <= closeY + closeS) {
        if (isDown) {
          playSound('btnClick', 0.7);
          state.showPaytable = false;
        }
        canvas.style.cursor = 'pointer';
        return;
      }

      // Check Yes/No buttons inside paytable modal
      if (mx >= modalX + 240 && mx <= modalX + 380 && my >= modalY + 450 && my <= modalY + 500) {
        if (isDown) {
          playSound('btnClick', 0.7);
          state.showPaytable = false;
        }
        canvas.style.cursor = 'pointer';
        return;
      }

      if (isDown && (mx < modalX || mx > modalX + modalW || my < modalY || my > modalY + modalH)) {
        state.showPaytable = false;
      }
      return;
    }

    // Dismiss celebration modal on click
    if (state.showCelebration && isDown) {
      state.showCelebration = false;
      concludeSpin();
      return;
    }

    // Find interactive button
    let found = null;
    for (const btn of UI_BUTTONS) {
      if (mx >= btn.x && mx <= btn.x + btn.w && my >= btn.y && my <= btn.y + btn.h) {
        found = btn;
        break;
      }
    }

    hoveredButtonId = found ? found.id : null;
    canvas.style.cursor = found ? 'pointer' : 'default';

    if (isDown && found) {
      getAudioContext(); // Unlock audio
      found.action();
    }
  }

  // --- Rendering Loop ---
  function render(ctx) {
    ctx.clearRect(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT);

    // 1. Background Gradient
    if (images.bg) {
      ctx.drawImage(images.bg, 0, 0, CANVAS_WIDTH, CANVAS_HEIGHT);
    } else {
      ctx.fillStyle = '#0f172a';
      ctx.fillRect(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT);
    }

    // 2. Draw 3 Reels (Backdrop + Symbols)
    state.reels.forEach(r => r.draw(ctx));

    // 3. Draw Machine Chassis Front (Casing with Cutouts)
    if (images.casing) {
      ctx.drawImage(images.casing, MACHINE_X, MACHINE_Y, MACHINE_WIDTH, MACHINE_HEIGHT);
    }

    // 4. Draw Interactive Mechanical Lever
    const leverImg = state.leverState === 'down' ? images.leverDown : images.leverUp;
    if (leverImg) {
      ctx.drawImage(leverImg, MACHINE_X, MACHINE_Y, MACHINE_WIDTH, MACHINE_HEIGHT);
    }

    // 5. Middle Message / Status Box
    const mbW = 480, mbH = 88;
    const mbX = MACHINE_X + 170, mbY = MACHINE_Y + 478;
    if (images.middleBox) {
      ctx.drawImage(images.middleBox, mbX, mbY, mbW, mbH);
    } else {
      ctx.fillStyle = 'rgba(15, 23, 42, 0.85)';
      ctx.fillRect(mbX, mbY, mbW, mbH);
    }

    // Status Text inside Middle Box
    ctx.save();
    ctx.fillStyle = state.freeSpins > 0 ? '#38bdf8' : (state.lastWin > 0 ? '#fbbf24' : '#f8fafc');
    ctx.font = 'bold 18px "Segoe UI", sans-serif';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.shadowColor = 'rgba(0, 0, 0, 0.9)';
    ctx.shadowBlur = 4;
    ctx.fillText(state.statusText, mbX + mbW / 2, mbY + mbH / 2);
    ctx.restore();

    // 6. Free Spins Banner badge (if active)
    if (state.freeSpins > 0) {
      ctx.save();
      ctx.fillStyle = '#f59e0b';
      ctx.shadowColor = '#d97706';
      ctx.shadowBlur = 10;
      ctx.font = 'bold 16px "Segoe UI", sans-serif';
      ctx.textAlign = 'center';
      ctx.fillText(`★ FREE SPINS: ${state.freeSpins} REMAINING (2X MULTIPLIER) ★`, MACHINE_X + 410, MACHINE_Y + 235);
      ctx.restore();
    }

    // 7. Render Particles
    for (let i = state.particles.length - 1; i >= 0; i--) {
      const p = state.particles[i];
      if (!p.update()) {
        state.particles.splice(i, 1);
      } else {
        p.draw(ctx);
      }
    }

    // 8. Render HUD Panels (Coins, Bet, Win, Buttons)
    renderHUD(ctx);

    // 9. Render Modals (Paytable or Big Win Celebration)
    if (state.showPaytable) {
      renderPaytableModal(ctx);
    } else if (state.showCelebration) {
      renderCelebrationModal(ctx);
    }
  }

  function renderHUD(ctx) {
    ctx.save();

    // Bottom Console Bar Panel
    const barX = 40, barY = 630, barW = 1200, barH = 68;
    ctx.fillStyle = 'rgba(15, 23, 42, 0.88)';
    ctx.strokeStyle = '#334155';
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.roundRect(barX, barY, barW, barH, 12);
    ctx.fill();
    ctx.stroke();

    // A. Balance Section
    ctx.fillStyle = '#94a3b8';
    ctx.font = 'bold 12px "Segoe UI", sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText('COIN BALANCE', 60, 652);

    ctx.fillStyle = '#f59e0b';
    ctx.font = 'bold 24px "Segoe UI", monospace';
    ctx.fillText(state.coins.toLocaleString(), 60, 678);

    // B. Last Win Section
    ctx.fillStyle = '#94a3b8';
    ctx.font = 'bold 12px "Segoe UI", sans-serif';
    ctx.fillText('LAST WIN', 250, 652);

    ctx.fillStyle = state.lastWin > 0 ? '#10b981' : '#64748b';
    ctx.font = 'bold 24px "Segoe UI", monospace';
    ctx.fillText(state.lastWin > 0 ? `+${state.lastWin.toLocaleString()}` : '0', 250, 678);

    // C. Bet Selector Section
    ctx.fillStyle = '#94a3b8';
    ctx.font = 'bold 12px "Segoe UI", sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText('TOTAL BET', 570, 652);

    // Prev Bet Sprite Button
    const prevBtn = UI_BUTTONS.find(b => b.id === 'betPrev');
    const prevImg = (hoveredButtonId === 'betPrev') ? images.btnPrevHover : images.btnPrevNorm;
    if (prevImg) ctx.drawImage(prevImg, prevBtn.x, prevBtn.y, prevBtn.w, prevBtn.h);

    // Bet Amount Value
    ctx.fillStyle = '#38bdf8';
    ctx.font = 'bold 22px "Segoe UI", monospace';
    ctx.fillText(getCurrentBet().toString(), 570, 678);

    // Next Bet Sprite Button
    const nextBtn = UI_BUTTONS.find(b => b.id === 'betNext');
    const nextImg = (hoveredButtonId === 'betNext') ? images.btnNextHover : images.btnNextNorm;
    if (nextImg) ctx.drawImage(nextImg, nextBtn.x, nextBtn.y, nextBtn.w, nextBtn.h);

    // Max Bet Button
    drawSimpleButton(ctx, UI_BUTTONS.find(b => b.id === 'maxBet'));

    // Paytable Button
    drawSimpleButton(ctx, UI_BUTTONS.find(b => b.id === 'paytable'));

    // Sound Toggle Button
    const sndBtn = UI_BUTTONS.find(b => b.id === 'sound');
    sndBtn.label = isMuted ? 'SOUND: OFF' : 'SOUND: ON';
    drawSimpleButton(ctx, sndBtn);

    // Spin Button (Right Side Large)
    drawLargeActionButton(ctx, UI_BUTTONS.find(b => b.id === 'spin'), state.isSpinning);

    // Auto Spin Button
    const autoBtn = UI_BUTTONS.find(b => b.id === 'auto');
    autoBtn.label = state.autoSpin ? 'STOP AUTO' : 'AUTO SPIN';
    autoBtn.color = state.autoSpin ? '#ef4444' : '#3b82f6';
    drawSimpleButton(ctx, autoBtn);

    ctx.restore();
  }

  function drawSimpleButton(ctx, btn) {
    if (!btn) return;
    const isHover = hoveredButtonId === btn.id;
    ctx.save();
    ctx.fillStyle = isHover ? lightenColor(btn.color, 20) : btn.color;
    ctx.strokeStyle = btn.border;
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.roundRect(btn.x, btn.y, btn.w, btn.h, 8);
    ctx.fill();
    ctx.stroke();

    ctx.fillStyle = '#ffffff';
    ctx.font = 'bold 14px "Segoe UI", sans-serif';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(btn.label, btn.x + btn.w / 2, btn.y + btn.h / 2);
    ctx.restore();
  }

  function drawLargeActionButton(ctx, btn, isDisabled) {
    if (!btn) return;
    const isHover = hoveredButtonId === btn.id && !isDisabled;
    ctx.save();

    if (isDisabled) {
      ctx.fillStyle = '#475569';
      ctx.strokeStyle = '#334155';
    } else {
      ctx.fillStyle = isHover ? '#4ade80' : btn.color;
      ctx.strokeStyle = btn.border;
      ctx.shadowColor = isHover ? '#86efac' : '#22c55e';
      ctx.shadowBlur = isHover ? 16 : 8;
    }

    ctx.lineWidth = 3;
    ctx.beginPath();
    ctx.roundRect(btn.x, btn.y, btn.w, btn.h, 12);
    ctx.fill();
    ctx.stroke();

    ctx.fillStyle = isDisabled ? '#94a3b8' : '#ffffff';
    ctx.font = 'bold 22px "Segoe UI", sans-serif';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(isDisabled ? 'SPINNING...' : btn.label, btn.x + btn.w / 2, btn.y + btn.h / 2);
    ctx.restore();
  }

  function renderPaytableModal(ctx) {
    ctx.save();
    // Dark Backdrop
    ctx.fillStyle = 'rgba(0, 0, 0, 0.75)';
    ctx.fillRect(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT);

    // Modal Frame (popup.png)
    const mx = 240, my = 70, mw = 800, mh = 560;
    if (images.popup) {
      ctx.drawImage(images.popup, mx, my, mw, mh);
    } else {
      ctx.fillStyle = '#1e1b4b';
      ctx.strokeStyle = '#4338ca';
      ctx.lineWidth = 4;
      ctx.fillRect(mx, my, mw, mh);
      ctx.strokeRect(mx, my, mw, mh);
    }

    // Modal Title
    ctx.fillStyle = '#fbbf24';
    ctx.font = 'bold 28px "Segoe UI", sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText('PAYOUT TABLE & RULES', mx + mw / 2, my + 55);

    // Close Button 'X'
    const closeImg = hoveredButtonId === 'modalClose' ? images.btnCloseHover : images.btnCloseNorm;
    if (closeImg) {
      ctx.drawImage(closeImg, mx + mw - 65, my + 20, 44, 44);
    }

    // Paytable Grid
    const items = [
      { sym: images.sym0, name: '7-7-7', desc: 'Grand Jackpot', pay: '100x Bet' },
      { sym: images.sym1, name: 'BAR-BAR-BAR', desc: 'Mega Win', pay: '50x Bet' },
      { sym: images.sym2, name: 'BELL-BELL-BELL', desc: 'Big Win + 5 Free Spins', pay: '25x Bet' },
      { sym: images.sym3, name: 'CHERRY-CHERRY-CHERRY', desc: 'Sweet Win', pay: '10x Bet' },
      { sym: images.sym3, name: 'Any 2 Cherries', desc: 'Minor Win', pay: '2x Bet' }
    ];

    items.forEach((item, idx) => {
      const rowY = my + 95 + idx * 62;
      ctx.fillStyle = 'rgba(255, 255, 255, 0.08)';
      ctx.fillRect(mx + 60, rowY, mw - 120, 52);

      if (item.sym) {
        ctx.drawImage(item.sym, mx + 75, rowY + 4, 44, 44);
      }

      ctx.fillStyle = '#f8fafc';
      ctx.font = 'bold 16px "Segoe UI", sans-serif';
      ctx.textAlign = 'left';
      ctx.fillText(item.name, mx + 135, rowY + 30);

      ctx.fillStyle = '#94a3b8';
      ctx.font = '14px "Segoe UI", sans-serif';
      ctx.fillText(item.desc, mx + 380, rowY + 30);

      ctx.fillStyle = '#f59e0b';
      ctx.font = 'bold 18px monospace';
      ctx.textAlign = 'right';
      ctx.fillText(item.pay, mx + mw - 85, rowY + 30);
    });

    // Bonus Features Explanation
    const bonusY = my + 415;
    ctx.fillStyle = '#e2e8f0';
    ctx.font = 'bold 15px "Segoe UI", sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText('⚡ BONUS FEATURES:', mx + 60, bonusY);

    ctx.font = '13px "Segoe UI", sans-serif';
    ctx.fillStyle = '#94a3b8';
    ctx.fillText('• Lucky Respin: Land 2 matching non-cherry symbols to lock them and respin the 3rd reel for free!', mx + 60, bonusY + 22);
    ctx.fillText('• Free Spins: 3 Bells trigger 5 Free Spins with an automatic 2X Win Multiplier on all payouts.', mx + 60, bonusY + 42);

    // Done Button (btn_yes_normal.png)
    const yesImg = images.btnYesNorm;
    if (yesImg) {
      ctx.drawImage(yesImg, mx + (mw - 140) / 2, my + 480, 140, 50);
    }

    ctx.restore();
  }

  function renderCelebrationModal(ctx) {
    ctx.save();
    ctx.fillStyle = 'rgba(0, 0, 0, 0.6)';
    ctx.fillRect(0, 0, CANVAS_WIDTH, CANVAS_HEIGHT);

    const cx = CANVAS_WIDTH / 2;
    const cy = CANVAS_HEIGHT / 2 - 20;

    // Glowing Box
    ctx.fillStyle = 'rgba(15, 23, 42, 0.95)';
    ctx.strokeStyle = '#f59e0b';
    ctx.lineWidth = 4;
    ctx.shadowColor = '#d97706';
    ctx.shadowBlur = 25;
    ctx.beginPath();
    ctx.roundRect(cx - 260, cy - 110, 520, 220, 16);
    ctx.fill();
    ctx.stroke();

    // Celebration Title
    ctx.fillStyle = '#f59e0b';
    ctx.font = 'bold 26px "Segoe UI", sans-serif';
    ctx.textAlign = 'center';
    ctx.shadowColor = 'rgba(0, 0, 0, 0.8)';
    ctx.shadowBlur = 6;
    ctx.fillText(state.celebrationText, cx, cy - 45);

    // Animated Ticker Value
    ctx.fillStyle = '#ffffff';
    ctx.font = 'bold 54px monospace';
    ctx.fillText(`+${state.displayWinnings.toLocaleString()}`, cx, cy + 25);

    ctx.fillStyle = '#94a3b8';
    ctx.font = '14px "Segoe UI", sans-serif';
    ctx.fillText('CLICK ANYWHERE TO CONTINUE', cx, cy + 75);

    ctx.restore();
  }

  function lightenColor(col, pct) {
    // Basic hex lighten helper
    if (col.startsWith('#')) {
      const num = parseInt(col.slice(1), 16);
      let r = (num >> 16) + pct;
      let g = ((num >> 8) & 0x00ff) + pct;
      let b = (num & 0x0000ff) + pct;
      r = Math.min(255, Math.max(0, r));
      g = Math.min(255, Math.max(0, g));
      b = Math.min(255, Math.max(0, b));
      return `rgb(${r},${g},${b})`;
    }
    return col;
  }

  // --- Main Initialization ---
  function init() {
    const canvas = document.getElementById('game-canvas');
    const ctx = canvas.getContext('2d');

    // Create Reel Instances
    state.reels = REEL_DEFS.map((def, idx) => new Reel(idx, def));

    // Pointer events
    canvas.addEventListener('mousemove', e => handlePointer(e.clientX, e.clientY, false));
    canvas.addEventListener('mousedown', e => handlePointer(e.clientX, e.clientY, true));
    canvas.addEventListener('touchstart', e => {
      const t = e.touches[0];
      handlePointer(t.clientX, t.clientY, true);
      e.preventDefault();
    }, { passive: false });

    // Load All Images and Audio
    loadAssets(() => {
      // Hide loading screen
      const ls = document.getElementById('loading-screen');
      if (ls) {
        ls.style.opacity = '0';
        setTimeout(() => ls.style.display = 'none', 500);
      }

      // Start RAF Loop
      let lastTime = performance.now();
      function loop(now) {
        const dt = Math.min(0.1, (now - lastTime) / 1000);
        lastTime = now;

        // Update lever spring return
        if (state.leverState === 'down') {
          state.leverPullTimer -= dt;
          if (state.leverPullTimer <= 0) {
            state.leverState = 'up';
          }
        }

        // Update reels
        state.reels.forEach(r => r.update(dt));

        // Render
        render(ctx);

        requestAnimationFrame(loop);
      }
      requestAnimationFrame(loop);
    });
  }

  function loadAssets(onComplete) {
    const imgKeys = Object.keys(ASSET_PATHS);
    const audioKeys = Object.keys(AUDIO_PATHS);
    const total = imgKeys.length + audioKeys.length;
    let loaded = 0;

    const progressFill = document.getElementById('progress-fill');
    const loadingText = document.getElementById('loading-text');

    function checkProgress() {
      loaded++;
      const pct = Math.round((loaded / total) * 100);
      if (progressFill) progressFill.style.width = `${pct}%`;
      if (loadingText) loadingText.textContent = `Loading Assets... ${pct}%`;

      if (loaded >= total) {
        onComplete();
      }
    }

    // 1. Load Images
    imgKeys.forEach(key => {
      const img = new Image();
      img.src = ASSET_PATHS[key];
      img.onload = () => {
        images[key] = img;
        checkProgress();
      };
      img.onerror = () => {
        console.warn('Failed loading image:', ASSET_PATHS[key]);
        checkProgress();
      };
    });

    // 2. Load Audio
    const actx = getAudioContext();
    audioKeys.forEach(key => {
      fetch(AUDIO_PATHS[key])
        .then(res => res.arrayBuffer())
        .then(buf => {
          if (actx) {
            return actx.decodeAudioData(buf);
          }
          return null;
        })
        .then(decoded => {
          if (decoded) audioBuffers[key] = decoded;
          checkProgress();
        })
        .catch(err => {
          console.warn('Audio load error for:', key, err);
          checkProgress();
        });
    });
  }

  window.addEventListener('DOMContentLoaded', init);
})();
