using System;
using SlotMachine.Data;
using UnityEngine;

namespace SlotMachine.Core
{
    /// <summary>
    /// Represents the evaluation result of a spin.
    /// </summary>
    [Serializable]
    public struct WinEvaluationResult
    {
        public bool isWin;
        public int winAmount;
        public int multiplier;
        public string winTitle;
        public bool isJackpot;
        public bool triggersFreeSpins;
        public bool triggersLuckyRespin;
        public int respinReelIndex; // 0, 1, or 2 if Lucky Respin triggered (-1 if none)
    }

    /// <summary>
    /// Evaluates reel outcomes, applies paytable rules, and calculates simulated payouts.
    /// </summary>
    public static class PayoutTable
    {
        // Standard Multipliers for 3 matching symbols on center payline
        public const int MULTIPLIER_SEVEN = 100;    // 7-7-7 (Grand Jackpot)
        public const int MULTIPLIER_BAR = 50;       // BAR-BAR-BAR (Mega Win)
        public const int MULTIPLIER_BELL = 25;      // BELL-BELL-BELL (Big Win + Free Spins)
        public const int MULTIPLIER_CHERRY_3 = 10;  // CHERRY-CHERRY-CHERRY
        public const int MULTIPLIER_CHERRY_2 = 2;   // Any 2 Cherries

        /// <summary>
        /// Evaluates the 3 center payline symbols and calculates winnings.
        /// </summary>
        /// <param name="r1">Reel 1 symbol</param>
        /// <param name="r2">Reel 2 symbol</param>
        /// <param name="r3">Reel 3 symbol</param>
        /// <param name="bet">Player current bet amount</param>
        /// <param name="isFreeSpinActive">Whether spin occurred during Free Spins mode</param>
        /// <returns>Comprehensive win evaluation result</returns>
        public static WinEvaluationResult Evaluate(SymbolType r1, SymbolType r2, SymbolType r3, int bet, bool isFreeSpinActive = false)
        {
            var result = new WinEvaluationResult
            {
                isWin = false,
                winAmount = 0,
                multiplier = 0,
                winTitle = string.Empty,
                isJackpot = false,
                triggersFreeSpins = false,
                triggersLuckyRespin = false,
                respinReelIndex = -1
            };

            float bonusMultiplier = isFreeSpinActive ? GameConfig.FREE_SPINS_MULTIPLIER : 1.0f;

            // 1. Check for 3 matching symbols (Primary Win Condition)
            if (r1 == r2 && r2 == r3)
            {
                result.isWin = true;

                switch (r1)
                {
                    case SymbolType.Seven:
                        result.multiplier = MULTIPLIER_SEVEN;
                        result.winTitle = "★ GRAND JACKPOT! ★";
                        result.isJackpot = true;
                        break;

                    case SymbolType.Bar:
                        result.multiplier = MULTIPLIER_BAR;
                        result.winTitle = "✦ MEGA WIN! ✦";
                        break;

                    case SymbolType.Bell:
                        result.multiplier = MULTIPLIER_BELL;
                        result.winTitle = "🔔 BELL BONANZA! (+5 FREE SPINS) 🔔";
                        result.triggersFreeSpins = true;
                        break;

                    case SymbolType.Cherry:
                        result.multiplier = MULTIPLIER_CHERRY_3;
                        result.winTitle = "🍒 SWEET CHERRY WIN! 🍒";
                        break;
                }

                result.winAmount = Mathf.RoundToInt(bet * result.multiplier * bonusMultiplier);
                return result;
            }

            // 2. Check for 2 Cherries (Secondary Win Condition)
            int cherryCount = 0;
            if (r1 == SymbolType.Cherry) cherryCount++;
            if (r2 == SymbolType.Cherry) cherryCount++;
            if (r3 == SymbolType.Cherry) cherryCount++;

            if (cherryCount == 2)
            {
                result.isWin = true;
                result.multiplier = MULTIPLIER_CHERRY_2;
                result.winTitle = "🍒 DOUBLE CHERRIES! 🍒";
                result.winAmount = Mathf.RoundToInt(bet * result.multiplier * bonusMultiplier);
                return result;
            }

            // 3. Check for Lucky Respin Bonus (Two matching non-cherry symbols)
            // If reels 1 and 2 match, reel 3 respins.
            // If reels 2 and 3 match, reel 1 respins.
            // If reels 1 and 3 match, reel 2 respins.
            if (!isFreeSpinActive)
            {
                if (r1 == r2 && r1 != SymbolType.Cherry)
                {
                    result.triggersLuckyRespin = true;
                    result.respinReelIndex = 2; // Reel 3 respins
                    result.winTitle = "⚡ LUCKY RESPIN! (Reel 3) ⚡";
                }
                else if (r2 == r3 && r2 != SymbolType.Cherry)
                {
                    result.triggersLuckyRespin = true;
                    result.respinReelIndex = 0; // Reel 1 respins
                    result.winTitle = "⚡ LUCKY RESPIN! (Reel 1) ⚡";
                }
                else if (r1 == r3 && r1 != SymbolType.Cherry)
                {
                    result.triggersLuckyRespin = true;
                    result.respinReelIndex = 1; // Reel 2 respins
                    result.winTitle = "⚡ LUCKY RESPIN! (Reel 2) ⚡";
                }
            }

            return result;
        }
    }
}
