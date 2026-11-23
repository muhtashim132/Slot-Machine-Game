using UnityEngine;

namespace SlotMachine.Data
{
    /// <summary>
    /// Global game configuration settings and gameplay balance constants.
    /// </summary>
    public static class GameConfig
    {
        // Currency & Betting Defaults
        public const int INITIAL_PLAYER_COINS = 1000;
        public const int DEFAULT_BET_AMOUNT = 10;
        public static readonly int[] BET_TIERS = { 10, 50, 100 };

        // Reel Spin Timings (Seconds)
        public const float MIN_SPIN_DURATION = 1.2f;
        public const float REEL_STAGGER_DELAY = 0.35f;
        public const float REEL_SPIN_SPEED = 2400f; // Pixels per second
        public const float ACCELERATION_DURATION = 0.25f;
        public const float DECELERATION_DURATION = 0.45f;
        public const float BOUNCE_AMPLITUDE = 18f; // Overshoot in pixels
        public const float BOUNCE_DURATION = 0.25f;

        // Visual Reel Dimensions (Pixels)
        public const float SYMBOL_SIZE = 96f;
        public const float SYMBOL_SPACING = 96f;
        public const float REEL_WINDOW_HEIGHT = 210f;
        public const float REEL_WINDOW_WIDTH = 108f;
        public const int VISIBLE_SYMBOLS_COUNT = 3;

        // Bonus Settings
        public const int FREE_SPINS_AWARDED = 5;
        public const float FREE_SPINS_MULTIPLIER = 2.0f;
    }
}
