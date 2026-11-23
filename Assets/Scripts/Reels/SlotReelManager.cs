using System;
using System.Collections;
using System.Collections.Generic;
using SlotMachine.Core;
using SlotMachine.Data;
using UnityEngine;

namespace SlotMachine.Reels
{
    /// <summary>
    /// Coordinates all 3 reels: staggered start, staggered stop,
    /// suspense timing, and individual reel locking for bonus respins.
    /// </summary>
    public class SlotReelManager : MonoBehaviour
    {
        public event Action<SymbolType[]> OnAllReelsStopped;

        [Header("Reel References")]
        [SerializeField] private ReelStrip[] reels;

        private SymbolType[] currentOutcomes = new SymbolType[3];
        private int stoppedReelsCount = 0;
        private bool isSpinActive = false;

        public bool IsSpinActive => isSpinActive;

        private void Awake()
        {
            if (reels != null)
            {
                for (int i = 0; i < reels.Length; i++)
                {
                    if (reels[i] != null)
                    {
                        reels[i].SetReelIndex(i);
                        reels[i].OnReelStopped += HandleReelStopped;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (reels != null)
            {
                for (int i = 0; i < reels.Length; i++)
                {
                    if (reels[i] != null)
                    {
                        reels[i].OnReelStopped -= HandleReelStopped;
                    }
                }
            }
        }

        /// <summary>
        /// Begins a standard 3-reel spin with staggered stop timings.
        /// </summary>
        public void SpinAllReels(SymbolType[] outcomes)
        {
            if (isSpinActive) return;
            isSpinActive = true;
            stoppedReelsCount = 0;
            currentOutcomes = outcomes;

            SoundManager.Instance?.StartReelSpinLoop();

            // Start all reels spinning
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].StartSpin();
            }

            StartCoroutine(StaggeredStopRoutine(outcomes));
        }

        /// <summary>
        /// Spins only a single targeted reel (used for Lucky Respin bonus).
        /// </summary>
        public void RespinSingleReel(int targetReelIndex, SymbolType outcome)
        {
            if (isSpinActive) return;
            isSpinActive = true;
            stoppedReelsCount = 2; // Only 1 reel will stop
            currentOutcomes[targetReelIndex] = outcome;

            SoundManager.Instance?.PlayRespinSuspense();
            SoundManager.Instance?.StartReelSpinLoop();

            reels[targetReelIndex].StartSpin();

            StartCoroutine(SingleReelStopRoutine(targetReelIndex, outcome));
        }

        private IEnumerator StaggeredStopRoutine(SymbolType[] outcomes)
        {
            // Base spin duration
            yield return new WaitForSeconds(GameConfig.MIN_SPIN_DURATION);

            // Reel 1 Stop
            reels[0].StopAtSymbol(outcomes[0]);
            yield return new WaitForSeconds(GameConfig.REEL_STAGGER_DELAY);

            // Reel 2 Stop
            reels[1].StopAtSymbol(outcomes[1]);

            // If reels 1 and 2 match, add suspense delay before reel 3 stops!
            float thirdReelDelay = (outcomes[0] == outcomes[1])
                ? GameConfig.REEL_STAGGER_DELAY * 1.75f
                : GameConfig.REEL_STAGGER_DELAY;

            yield return new WaitForSeconds(thirdReelDelay);

            // Reel 3 Stop
            reels[2].StopAtSymbol(outcomes[2]);
        }

        private IEnumerator SingleReelStopRoutine(int reelIndex, SymbolType outcome)
        {
            // Suspense duration for single reel respin
            yield return new WaitForSeconds(1.6f);
            reels[reelIndex].StopAtSymbol(outcome);
        }

        private void HandleReelStopped(int reelIndex, SymbolType outcome)
        {
            stoppedReelsCount++;
            if (stoppedReelsCount >= 3)
            {
                isSpinActive = false;
                SoundManager.Instance?.StopReelSpinLoop();
                OnAllReelsStopped?.Invoke(currentOutcomes);
            }
        }

        /// <summary>
        /// Plays win pulse animations across all symbols on the payline.
        /// </summary>
        public void HighlightWinningSymbols()
        {
            for (int i = 0; i < reels.Length; i++)
            {
                var sym = reels[i].GetCenterSymbol();
                sym?.PlayWinAnimation();
            }
        }

        /// <summary>
        /// Stops any ongoing symbol animations.
        /// </summary>
        public void ClearHighlights()
        {
            for (int i = 0; i < reels.Length; i++)
            {
                var sym = reels[i].GetCenterSymbol();
                sym?.StopWinAnimation();
            }
        }
    }
}
