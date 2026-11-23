using System;
using System.Security.Cryptography;
using SlotMachine.Data;
using UnityEngine;

namespace SlotMachine.Core
{
    /// <summary>
    /// Provides cryptographically secure, verifiable Random Number Generation (RNG)
    /// for slot machine reel outcomes and fairness simulation.
    /// </summary>
    public class RNGService
    {
        private static readonly RNGCryptoServiceProvider CryptoProvider = new RNGCryptoServiceProvider();

        // Default weights: Cherry=40%, Bell=30%, Bar=20%, Seven=10% (RTP ~96.5%)
        private static readonly int[] DefaultWeights = { 10, 20, 30, 40 };

        /// <summary>
        /// Generates a cryptographically strong pseudo-random integer in the range [minValue, maxValue).
        /// </summary>
        public static int GetSecureInt(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), "minValue must be strictly less than maxValue");

            long diff = (long)maxValue - minValue;
            byte[] uint32Buffer = new byte[4];

            while (true)
            {
                CryptoProvider.GetBytes(uint32Buffer);
                uint rand = BitConverter.ToUInt32(uint32Buffer, 0);

                long maxValid = (1L << 32) - ((1L << 32) % diff);
                if (rand < maxValid)
                {
                    return (int)(minValue + (rand % diff));
                }
            }
        }

        /// <summary>
        /// Selects a random symbol based on weighted probability distribution.
        /// </summary>
        public static SymbolType GetWeightedSymbol(int[] customWeights = null)
        {
            int[] weights = customWeights ?? DefaultWeights;
            int totalWeight = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                totalWeight += weights[i];
            }

            int randomValue = GetSecureInt(0, totalWeight);
            int cumulative = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                cumulative += weights[i];
                if (randomValue < cumulative)
                {
                    return (SymbolType)i;
                }
            }

            return SymbolType.Cherry;
        }

        /// <summary>
        /// Generates a fair 3-reel spin outcome.
        /// </summary>
        public static SymbolType[] GenerateSpinOutcome()
        {
            return new SymbolType[]
            {
                GetWeightedSymbol(),
                GetWeightedSymbol(),
                GetWeightedSymbol()
            };
        }

        /// <summary>
        /// Generates an outcome for a single reel (used during Lucky Respin bonus).
        /// </summary>
        public static SymbolType GenerateSingleReelOutcome()
        {
            return GetWeightedSymbol();
        }
    }
}
