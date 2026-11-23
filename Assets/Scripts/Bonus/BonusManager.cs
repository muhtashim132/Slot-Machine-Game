using System;
using SlotMachine.Core;
using SlotMachine.Data;
using SlotMachine.UI;
using UnityEngine;

namespace SlotMachine.Bonus
{
    /// <summary>
    /// Coordinates classic bonus features:
    /// 1. Lucky Respin: Locks two matching reels and spins the 3rd reel for a free second chance.
    /// 2. Free Spins Frenzy: 5 Free Spins with a 2x Payout Multiplier triggered by 3 Bells.
    /// </summary>
    public class BonusManager : MonoBehaviour
    {
        public static BonusManager Instance { get; private set; }

        private int remainingFreeSpins = 0;
        public bool IsFreeSpinsActive => remainingFreeSpins > 0;
        public int RemainingFreeSpins => remainingFreeSpins;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Awards free spins to the player.
        /// </summary>
        public void AwardFreeSpins(int count)
        {
            remainingFreeSpins += count;
            UIManager.Instance?.SetFreeSpinsBanner(true, remainingFreeSpins);
            UIManager.Instance?.SetStatusText($"★ {count} FREE SPINS AWARDED! 2X MULTIPLIER! ★");
        }

        /// <summary>
        /// Consumes one free spin.
        /// </summary>
        public void ConsumeFreeSpin()
        {
            if (remainingFreeSpins > 0)
            {
                remainingFreeSpins--;
                UIManager.Instance?.SetFreeSpinsBanner(remainingFreeSpins > 0, remainingFreeSpins);
                if (remainingFreeSpins <= 0)
                {
                    UIManager.Instance?.SetStatusText("Free Spins Concluded! Back to normal mode.");
                }
            }
        }

        public void ResetBonus()
        {
            remainingFreeSpins = 0;
            UIManager.Instance?.SetFreeSpinsBanner(false);
        }
    }
}
