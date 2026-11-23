using System;
using System.Collections;
using SlotMachine.Bonus;
using SlotMachine.Data;
using SlotMachine.Reels;
using SlotMachine.UI;
using UnityEngine;

namespace SlotMachine.Core
{
    /// <summary>
    /// Central game state controller. Orchestrates betting, spin validation,
    /// RNG generation, reel animation triggers, payout distribution, and auto-spins.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameState
        {
            Idle,
            Spinning,
            Evaluating,
            Celebrating,
            Respinning
        }

        [Header("Subsystems")]
        [SerializeField] private SlotReelManager reelManager;
        [SerializeField] private LeverController leverController;

        [Header("Player Economy")]
        [SerializeField] private int playerCoins = GameConfig.INITIAL_PLAYER_COINS;
        [SerializeField] private int currentBetIndex = 0;
        [SerializeField] private int customBet = 10;
        private int lastWinAmount = 0;

        [Header("Manual Simulator Settings")]
        private SymbolType?[] forcedOutcome = new SymbolType?[3] { null, null, null };

        [Header("State")]
        private GameState currentState = GameState.Idle;
        private bool isAutoSpinActive = false;
        private Coroutine autoSpinCoroutine;

        public GameState CurrentState => currentState;
        public int PlayerCoins => playerCoins;
        public int CurrentBet => customBet > 0 ? customBet : GameConfig.BET_TIERS[currentBetIndex];

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

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            currentState = GameState.Idle;
            UIManager.Instance?.UpdateBalance(playerCoins);
            UIManager.Instance?.UpdateBet(CurrentBet);
            UIManager.Instance?.UpdateWin(0);
            UIManager.Instance?.SetStatusText("Pull Lever or Press SPIN to Play!");

            if (leverController != null)
                leverController.OnLeverPulled += ExecuteSpin;

            if (reelManager != null)
                reelManager.OnAllReelsStopped += HandleAllReelsStopped;
        }

        private void OnDestroy()
        {
            if (leverController != null)
                leverController.OnLeverPulled -= ExecuteSpin;

            if (reelManager != null)
                reelManager.OnAllReelsStopped -= HandleAllReelsStopped;
        }

        public void OnSpinButtonPressed()
        {
            SoundManager.Instance?.PlayButtonClick();
            if (currentState == GameState.Idle)
            {
                // Pull lever visually and execute spin
                leverController?.PullLever();
            }
        }

        /// <summary>
        /// Initiates a spin cycle.
        /// </summary>
        public void ExecuteSpin()
        {
            if (currentState != GameState.Idle) return;

            bool isFreeSpin = BonusManager.Instance != null && BonusManager.Instance.IsFreeSpinsActive;

            // Validate balance if not a free spin
            if (!isFreeSpin)
            {
                if (playerCoins < CurrentBet)
                {
                    UIManager.Instance?.SetStatusText("Insufficient Coins! Lower bet or reset.");
                    StopAutoSpin();
                    return;
                }

                // Deduct Bet
                playerCoins -= CurrentBet;
                UIManager.Instance?.UpdateBalance(playerCoins);
            }
            else
            {
                BonusManager.Instance.ConsumeFreeSpin();
            }

            currentState = GameState.Spinning;
            reelManager?.ClearHighlights();
            UIManager.Instance?.SetSpinInteractable(false);
            leverController?.SetInteractable(false);
            UIManager.Instance?.SetStatusText(isFreeSpin ? "★ FREE SPIN IN PROGRESS! ★" : "Reels Spinning... Good Luck!");

            // 1. Get outcome (Use manually set outcome if specified, else fair RNG)
            SymbolType[] outcomes = new SymbolType[3];
            for (int i = 0; i < 3; i++)
            {
                outcomes[i] = forcedOutcome[i].HasValue ? forcedOutcome[i].Value : RNGService.GetWeightedSymbol();
            }

            // 2. Trigger physical reel spinning
            reelManager?.SpinAllReels(outcomes);
        }

        private void HandleAllReelsStopped(SymbolType[] outcomes)
        {
            currentState = GameState.Evaluating;

            // Evaluate payout against payout table
            bool isFreeSpin = BonusManager.Instance != null && BonusManager.Instance.IsFreeSpinsActive;
            WinEvaluationResult result = PayoutTable.Evaluate(outcomes[0], outcomes[1], outcomes[2], CurrentBet, isFreeSpin);

            // 1. Handle Lucky Respin Bonus condition (Near Miss)
            if (result.triggersLuckyRespin && !isFreeSpin)
            {
                currentState = GameState.Respinning;
                UIManager.Instance?.SetStatusText(result.winTitle);
                StartCoroutine(ExecuteLuckyRespinRoutine(result.respinReelIndex, outcomes));
                return;
            }

            // 2. Handle Win
            if (result.isWin)
            {
                currentState = GameState.Celebrating;
                lastWinAmount = result.winAmount;
                playerCoins += lastWinAmount;

                UIManager.Instance?.UpdateBalance(playerCoins);
                UIManager.Instance?.UpdateWin(lastWinAmount);
                UIManager.Instance?.SetStatusText(result.winTitle);
                reelManager?.HighlightWinningSymbols();

                // Check Free Spins trigger
                if (result.triggersFreeSpins)
                {
                    BonusManager.Instance?.AwardFreeSpins(GameConfig.FREE_SPINS_AWARDED);
                }

                // Trigger Celebration UI
                WinCelebrationUI.Instance?.ShowCelebration(result.winTitle, lastWinAmount, result.isJackpot, () =>
                {
                    ConcludeSpinCycle();
                });
            }
            else
            {
                // No win
                UIManager.Instance?.SetStatusText("Better luck next spin! Try again.");
                ConcludeSpinCycle();
            }
        }

        private IEnumerator ExecuteLuckyRespinRoutine(int respinReelIndex, SymbolType[] initialOutcomes)
        {
            yield return new WaitForSeconds(0.6f);

            // Generate new outcome for the single respinning reel
            SymbolType newSymbol = RNGService.GenerateSingleReelOutcome();
            initialOutcomes[respinReelIndex] = newSymbol;

            reelManager?.RespinSingleReel(respinReelIndex, newSymbol);
            // Result will be reported back via HandleAllReelsStopped
        }

        private void ConcludeSpinCycle()
        {
            currentState = GameState.Idle;
            UIManager.Instance?.SetSpinInteractable(true);
            leverController?.SetInteractable(true);

            // Auto-spin next cycle if enabled
            if (isAutoSpinActive)
            {
                if (playerCoins >= CurrentBet || (BonusManager.Instance != null && BonusManager.Instance.IsFreeSpinsActive))
                {
                    StartCoroutine(AutoSpinDelayRoutine());
                }
                else
                {
                    StopAutoSpin();
                }
            }
        }

        private IEnumerator AutoSpinDelayRoutine()
        {
            yield return new WaitForSeconds(1.0f);
            if (isAutoSpinActive && currentState == GameState.Idle)
            {
                ExecuteSpin();
            }
        }

        public void IncreaseBet()
        {
            if (currentState != GameState.Idle) return;
            SoundManager.Instance?.PlayButtonClick();
            currentBetIndex = (currentBetIndex + 1) % GameConfig.BET_TIERS.Length;
            UIManager.Instance?.UpdateBet(CurrentBet);
        }

        public void DecreaseBet()
        {
            if (currentState != GameState.Idle) return;
            SoundManager.Instance?.PlayButtonClick();
            currentBetIndex = (currentBetIndex - 1 + GameConfig.BET_TIERS.Length) % GameConfig.BET_TIERS.Length;
            UIManager.Instance?.UpdateBet(CurrentBet);
        }

        public void OpenBetSettings()
        {
            SoundManager.Instance?.PlayButtonClick();
            UIManager.Instance?.ToggleBetSettings();
        }

        public void ToggleAutoSpin()
        {
            SoundManager.Instance?.PlayButtonClick();
            isAutoSpinActive = !isAutoSpinActive;
            UIManager.Instance?.SetAutoSpinActive(isAutoSpinActive);

            if (isAutoSpinActive && currentState == GameState.Idle)
            {
                ExecuteSpin();
            }
        }

        public void StopAutoSpin()
        {
            isAutoSpinActive = false;
            UIManager.Instance?.SetAutoSpinActive(false);
        }

        public void ResetCoins()
        {
            playerCoins = GameConfig.INITIAL_PLAYER_COINS;
            UIManager.Instance?.UpdateBalance(playerCoins);
            UIManager.Instance?.SetStatusText("Coins reloaded to 1,000!");
        }

        /// <summary>
        /// Manually sets any custom bet amount.
        /// </summary>
        public void SetCustomBet(int amount)
        {
            if (currentState != GameState.Idle) return;
            customBet = Mathf.Max(1, amount);
            UIManager.Instance?.UpdateBet(CurrentBet);
        }

        /// <summary>
        /// Manually toggles a winning rule on or off.
        /// </summary>
        public void SetWinningRuleEnabled(string ruleId, bool enabled)
        {
            PayoutTable.SetRuleEnabled(ruleId, enabled);
        }

        /// <summary>
        /// Manually edits the payout multiplier for any rule.
        /// </summary>
        public void SetWinningRuleMultiplier(string ruleId, int multiplier)
        {
            PayoutTable.SetRuleMultiplier(ruleId, multiplier);
        }

        [ContextMenu("Apply Assignment Rules (3-Matching Only)")]
        public void ApplyAssignmentRulesPreset() => PayoutTable.ResetToAssignmentRules();

        [ContextMenu("Apply Arcade Rules (+2 Cherries & Respin)")]
        public void ApplyArcadeRulesPreset() => PayoutTable.ApplyArcadeRules();

        [ContextMenu("Apply High Roller Rules (250x, 100x)")]
        public void ApplyHighRollerRulesPreset() => PayoutTable.ApplyHighRollerRules();

        /// <summary>
        /// Manually forces a specific reel outcome to simulate winning combinations and verify payouts.
        /// </summary>
        public void SetForcedOutcome(SymbolType? r1, SymbolType? r2, SymbolType? r3)
        {
            forcedOutcome[0] = r1;
            forcedOutcome[1] = r2;
            forcedOutcome[2] = r3;
        }

        [ContextMenu("Simulate 7-7-7 Jackpot (100x)")]
        public void SimulateJackpot777() => SetForcedOutcome(SymbolType.Seven, SymbolType.Seven, SymbolType.Seven);

        [ContextMenu("Simulate BAR-BAR-BAR (50x)")]
        public void SimulateMegaWinBar() => SetForcedOutcome(SymbolType.Bar, SymbolType.Bar, SymbolType.Bar);

        [ContextMenu("Simulate BELL-BELL-BELL (25x + Free Spins)")]
        public void SimulateBellBonanza() => SetForcedOutcome(SymbolType.Bell, SymbolType.Bell, SymbolType.Bell);

        [ContextMenu("Simulate 3x Cherries (10x)")]
        public void SimulateTripleCherries() => SetForcedOutcome(SymbolType.Cherry, SymbolType.Cherry, SymbolType.Cherry);

        [ContextMenu("Simulate 2x Cherries (2x)")]
        public void SimulateDoubleCherries() => SetForcedOutcome(SymbolType.Cherry, SymbolType.Cherry, SymbolType.Seven);

        [ContextMenu("Simulate Lucky Respin (7-7-Cherry)")]
        public void SimulateLuckyRespin() => SetForcedOutcome(SymbolType.Seven, SymbolType.Seven, SymbolType.Cherry);

        [ContextMenu("Reset to Random RNG")]
        public void ResetToRandomRNG() => SetForcedOutcome(null, null, null);
    }
}
