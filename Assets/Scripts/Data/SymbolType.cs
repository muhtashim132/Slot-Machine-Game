using System;
using UnityEngine;

namespace SlotMachine.Data
{
    /// <summary>
    /// Represents the classic slot machine symbols available in the game.
    /// </summary>
    public enum SymbolType
    {
        Seven = 0,   // Highest tier symbol (Jackpot)
        Bar = 1,     // High tier symbol
        Bell = 2,    // Mid tier symbol (Bonus / Free Spins trigger)
        Cherry = 3   // Low tier symbol (Frequent payouts)
    }

    /// <summary>
    /// Data definition for each symbol, including sprite references, display names, and relative weights.
    /// </summary>
    [Serializable]
    public class SymbolDefinition
    {
        [Tooltip("The unique identifier for the symbol.")]
        public SymbolType type;

        [Tooltip("Human-readable name of the symbol.")]
        public string displayName;

        [Tooltip("Sprite texture representing this symbol.")]
        public Sprite sprite;

        [Tooltip("Base payout multiplier when 3 matching symbols land on the payline.")]
        public int threeMatchMultiplier;

        [Tooltip("Weight for RNG outcome selection (lower = rarer).")]
        [Range(1, 100)]
        public int rngWeight = 10;
    }
}
