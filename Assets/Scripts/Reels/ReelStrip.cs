using System;
using System.Collections;
using System.Collections.Generic;
using SlotMachine.Core;
using SlotMachine.Data;
using UnityEngine;

namespace SlotMachine.Reels
{
    /// <summary>
    /// Manages an individual reel column, pooling symbols, vertical wrapping,
    /// high-speed spin physics, and realistic spring-damped bounce on stopping.
    /// </summary>
    public class ReelStrip : MonoBehaviour
    {
        public event Action<int, SymbolType> OnReelStopped;

        [Header("Reel Configuration")]
        [SerializeField] private int reelIndex = 0;
        [SerializeField] private RectTransform containerRect;
        [SerializeField] private List<ReelSymbol> symbolsPool = new List<ReelSymbol>();

        [Header("Sprites")]
        [SerializeField] private Sprite sevenSprite;
        [SerializeField] private Sprite barSprite;
        [SerializeField] private Sprite bellSprite;
        [SerializeField] private Sprite cherrySprite;

        private bool isSpinning = false;
        private SymbolType targetSymbol;
        private float currentScrollSpeed = 0f;
        private float currentYOffset = 0f;
        private const float ItemHeight = GameConfig.SYMBOL_SPACING;

        public bool IsSpinning => isSpinning;
        public int ReelIndex => reelIndex;

        public void SetReelIndex(int index) => reelIndex = index;

        /// <summary>
        /// Initializes the reel with symbols.
        /// </summary>
        public void Initialize(Sprite s7, Sprite sBar, Sprite sBell, Sprite sCherry)
        {
            sevenSprite = s7;
            barSprite = sBar;
            bellSprite = sBell;
            cherrySprite = sCherry;

            // Initialize symbol positions
            ArrangeInitialSymbols();
        }

        private void ArrangeInitialSymbols()
        {
            // Initial symbols setup: 0=Top, 1=Center (Payline), 2=Bottom
            if (symbolsPool.Count >= 3)
            {
                SetSymbolSprite(symbolsPool[0], SymbolType.Seven);
                SetSymbolSprite(symbolsPool[1], SymbolType.Bar);
                SetSymbolSprite(symbolsPool[2], SymbolType.Bell);

                symbolsPool[0].RectTransform.anchoredPosition = new Vector2(0f, ItemHeight);
                symbolsPool[1].RectTransform.anchoredPosition = new Vector2(0f, 0f);
                symbolsPool[2].RectTransform.anchoredPosition = new Vector2(0f, -ItemHeight);
            }
        }

        public void SetSymbolSprite(ReelSymbol symbol, SymbolType type)
        {
            Sprite s = cherrySprite;
            switch (type)
            {
                case SymbolType.Seven: s = sevenSprite; break;
                case SymbolType.Bar: s = barSprite; break;
                case SymbolType.Bell: s = bellSprite; break;
                case SymbolType.Cherry: s = cherrySprite; break;
            }
            symbol.SetSymbol(type, s);
        }

        /// <summary>
        /// Begins spinning the reel.
        /// </summary>
        public void StartSpin()
        {
            if (isSpinning) return;
            isSpinning = true;
            StopAllCoroutines();
            StartCoroutine(SpinRoutine());
        }

        /// <summary>
        /// Signals the reel to stop on the target symbol.
        /// </summary>
        public void StopAtSymbol(SymbolType symbol)
        {
            targetSymbol = symbol;
            shouldStop = true;
        }

        private bool shouldStop = false;

        private IEnumerator SpinRoutine()
        {
            shouldStop = false;
            float targetSpeed = GameConfig.REEL_SPIN_SPEED;
            currentScrollSpeed = 0f;

            // 1. Acceleration Phase
            float accelTime = GameConfig.ACCELERATION_DURATION;
            float elapsed = 0f;
            while (elapsed < accelTime)
            {
                elapsed += Time.deltaTime;
                currentScrollSpeed = Mathf.Lerp(0f, targetSpeed, elapsed / accelTime);
                AdvanceSymbols(currentScrollSpeed * Time.deltaTime);
                yield return null;
            }

            currentScrollSpeed = targetSpeed;

            // 2. High-speed sustained spin
            while (!shouldStop)
            {
                AdvanceSymbols(currentScrollSpeed * Time.deltaTime);
                yield return null;
            }

            // 3. Deceleration Phase to Target Symbol
            // Prepare center symbol to be targetSymbol
            yield return StartCoroutine(DecelerateAndAlignRoutine(targetSymbol));

            isSpinning = false;
            SoundManager.Instance?.PlayReelStop();
            OnReelStopped?.Invoke(reelIndex, targetSymbol);
        }

        private void AdvanceSymbols(float deltaY)
        {
            // Move symbols downwards
            currentYOffset += deltaY;

            int count = symbolsPool.Count;
            if (count == 0) return;

            float totalSpan = count * ItemHeight;

            for (int i = 0; i < count; i++)
            {
                var sym = symbolsPool[i];
                float basePos = (1 - i) * ItemHeight;
                float currentPos = (basePos - currentYOffset) % totalSpan;
                if (currentPos < -ItemHeight * 1.5f)
                {
                    currentPos += totalSpan;
                    // Randomize symbol as it wraps to top
                    SetSymbolSprite(sym, (SymbolType)UnityEngine.Random.Range(0, 4));
                }
                sym.RectTransform.anchoredPosition = new Vector2(0f, currentPos);
            }
        }

        private IEnumerator DecelerateAndAlignRoutine(SymbolType finalSymbol)
        {
            // Smoothly align the center symbol to exactly Y = 0 with a realistic spring overshoot
            float decelDuration = GameConfig.DECELERATION_DURATION;
            float elapsed = 0f;

            // Assign final symbols: Center is finalSymbol
            SymbolType topSymbol = (SymbolType)(((int)finalSymbol + 1) % 4);
            SymbolType bottomSymbol = (SymbolType)(((int)finalSymbol + 3) % 4);

            if (symbolsPool.Count >= 3)
            {
                SetSymbolSprite(symbolsPool[0], topSymbol);
                SetSymbolSprite(symbolsPool[1], finalSymbol);
                SetSymbolSprite(symbolsPool[2], bottomSymbol);
            }

            // Smooth spring damping: overshoot downward, then bounce back up to resting position
            float bounceAmp = GameConfig.BOUNCE_AMPLITUDE;
            float bounceDur = GameConfig.BOUNCE_DURATION;

            // Pull down with overshoot
            elapsed = 0f;
            while (elapsed < decelDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / decelDuration;
                // Ease out cubic
                float ease = 1f - Mathf.Pow(1f - t, 3f);
                float yOffset = Mathf.Lerp(ItemHeight * 1.5f, -bounceAmp, ease);

                ApplyRestPositions(yOffset);
                yield return null;
            }

            // Spring bounce back to 0
            elapsed = 0f;
            while (elapsed < bounceDur)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / bounceDur;
                // Elastic bounce equation
                float bounce = -bounceAmp * Mathf.Cos(t * Mathf.PI * 0.5f);
                ApplyRestPositions(bounce);
                yield return null;
            }

            // Settle exactly at pixel-perfect resting position
            ApplyRestPositions(0f);
        }

        private void ApplyRestPositions(float offset)
        {
            if (symbolsPool.Count >= 3)
            {
                symbolsPool[0].RectTransform.anchoredPosition = new Vector2(0f, ItemHeight + offset);
                symbolsPool[1].RectTransform.anchoredPosition = new Vector2(0f, offset); // Center Payline
                symbolsPool[2].RectTransform.anchoredPosition = new Vector2(0f, -ItemHeight + offset);
            }
        }

        public ReelSymbol GetCenterSymbol()
        {
            return symbolsPool.Count >= 2 ? symbolsPool[1] : null;
        }
    }
}
