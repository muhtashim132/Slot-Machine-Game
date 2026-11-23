using System;
using System.Collections.Generic;
using SlotMachine.Data;
using UnityEngine;

namespace SlotMachine.Core
{
    /// <summary>
    /// Type of win matching condition.
    /// </summary>
    [Serializable]
    public enum RuleConditionType
    {
        AllMatchSpecific,   // All slots match a specific symbol (e.g. 7-7-7)
        CountSpecific,      // At least N of a specific symbol appear (e.g. 2 Cherries)
        AllMatchAny,        // All slots match each other regardless of symbol
        AnyPairRespin       // Two matching symbols trigger lucky respin
    }

    /// <summary>
    /// Type of bonus triggered upon winning combination.
    /// </summary>
    [Serializable]
    public enum BonusType
    {
        None,
        FreeSpins,
        Respin
    }

    /// <summary>
    /// A configurable winning rule that defines combination criteria, payout multiplier, and bonus triggers.
    /// Allows runtime manual configuration of game rules.
    /// </summary>
    [Serializable]
    public class WinRule
    {
        public string id;
        public string name;
        public bool isEnabled;
        public RuleConditionType condition;
        public SymbolType symbol;
        public int requiredCount;
        public int multiplier;
        public BonusType bonus;
        public string winTitle;

        public WinRule() { }

        public WinRule(string id, string name, bool isEnabled, RuleConditionType condition, SymbolType symbol, int requiredCount, int multiplier, BonusType bonus, string winTitle)
        {
            this.id = id;
            this.name = name;
            this.isEnabled = isEnabled;
            this.condition = condition;
            this.symbol = symbol;
            this.requiredCount = requiredCount;
            this.multiplier = multiplier;
            this.bonus = bonus;
            this.winTitle = winTitle;
        }
    }

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
    /// Dynamic, customizable payout table and rule engine.
    /// Allows manual rule toggling, multiplier editing, and custom rule insertion.
    /// </summary>
    public static class PayoutTable
    {
        private static List<WinRule> activeRules = new List<WinRule>();

        static PayoutTable()
        {
            ResetToAssignmentRules();
        }

        /// <summary>
        /// Resets rules to the strict assignment specification:
        /// Player wins when ALL slots have the SAME symbol (3-matching only).
        /// Secondary rules (2-cherries, respin) are disabled by default.
        /// </summary>
        public static void ResetToAssignmentRules()
        {
            activeRules = new List<WinRule>
            {
                new WinRule("rule_777", "★ 7-7-7 (Triple Seven)", true, RuleConditionType.AllMatchSpecific, SymbolType.Seven, 3, 100, BonusType.None, "★ GRAND JACKPOT! (3x Seven) ★"),
                new WinRule("rule_bar", "✦ BAR-BAR-BAR", true, RuleConditionType.AllMatchSpecific, SymbolType.Bar, 3, 50, BonusType.None, "✦ MEGA WIN! (3x BAR) ✦"),
                new WinRule("rule_bell", "🔔 BELL-BELL-BELL", true, RuleConditionType.AllMatchSpecific, SymbolType.Bell, 3, 25, BonusType.FreeSpins, "🔔 BELL BONANZA! (+5 FREE SPINS) 🔔"),
                new WinRule("rule_cherry_3", "🍒 CHERRY-CHERRY-CHERRY", true, RuleConditionType.AllMatchSpecific, SymbolType.Cherry, 3, 10, BonusType.None, "🍒 SWEET CHERRY WIN! 🍒"),
                new WinRule("rule_cherry_2", "🍒 Double Cherries", false, RuleConditionType.CountSpecific, SymbolType.Cherry, 2, 2, BonusType.None, "🍒 DOUBLE CHERRIES WIN! 🍒"),
                new WinRule("rule_respin", "⚡ Lucky Respin (Matching Pair)", false, RuleConditionType.AnyPairRespin, SymbolType.Seven, 2, 0, BonusType.Respin, "⚡ LUCKY RESPIN! ⚡")
            };
        }

        /// <summary>
        /// Loads the Arcade Rule Preset with 2-cherries and lucky respin enabled.
        /// </summary>
        public static void ApplyArcadeRules()
        {
            ResetToAssignmentRules();
            SetRuleEnabled("rule_cherry_2", true);
            SetRuleEnabled("rule_respin", true);
        }

        /// <summary>
        /// Loads the High Roller Rule Preset with increased multipliers.
        /// </summary>
        public static void ApplyHighRollerRules()
        {
            activeRules = new List<WinRule>
            {
                new WinRule("rule_777", "★ 7-7-7 (Mega Jackpot)", true, RuleConditionType.AllMatchSpecific, SymbolType.Seven, 3, 250, BonusType.None, "★ GRAND JACKPOT! (250x) ★"),
                new WinRule("rule_bar", "✦ BAR-BAR-BAR (Super Win)", true, RuleConditionType.AllMatchSpecific, SymbolType.Bar, 3, 100, BonusType.None, "✦ SUPER MEGA WIN! (100x) ✦"),
                new WinRule("rule_bell", "🔔 BELL-BELL-BELL", true, RuleConditionType.AllMatchSpecific, SymbolType.Bell, 3, 50, BonusType.FreeSpins, "🔔 BELL BONANZA! (50x + Free Spins) 🔔"),
                new WinRule("rule_cherry_3", "🍒 CHERRY-CHERRY-CHERRY", true, RuleConditionType.AllMatchSpecific, SymbolType.Cherry, 3, 20, BonusType.None, "🍒 SWEET CHERRIES! (20x) 🍒"),
                new WinRule("rule_cherry_2", "🍒 Double Cherries", false, RuleConditionType.CountSpecific, SymbolType.Cherry, 2, 5, BonusType.None, "🍒 DOUBLE CHERRIES WIN! 🍒"),
                new WinRule("rule_respin", "⚡ Lucky Respin", false, RuleConditionType.AnyPairRespin, SymbolType.Seven, 2, 0, BonusType.Respin, "⚡ LUCKY RESPIN! ⚡")
            };
        }

        public static IReadOnlyList<WinRule> ActiveRules => activeRules;

        /// <summary>
        /// Manually enables or disables a specific winning rule.
        /// </summary>
        public static void SetRuleEnabled(string ruleId, bool enabled)
        {
            var rule = activeRules.Find(r => r.id == ruleId);
            if (rule != null)
            {
                rule.isEnabled = enabled;
            }
        }

        /// <summary>
        /// Manually sets a custom multiplier for a specific winning rule.
        /// </summary>
        public static void SetRuleMultiplier(string ruleId, int multiplier)
        {
            var rule = activeRules.Find(r => r.id == ruleId);
            if (rule != null)
            {
                rule.multiplier = Mathf.Max(0, multiplier);
            }
        }

        /// <summary>
        /// Adds a custom winning rule dynamically.
        /// </summary>
        public static void AddCustomRule(WinRule rule)
        {
            if (rule != null)
            {
                activeRules.Add(rule);
            }
        }

        /// <summary>
        /// Evaluates reel outcomes against active, manually configured winning rules.
        /// </summary>
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
            WinRule matchedWinRule = null;
            WinRule matchedRespinRule = null;
            int respinTarget = -1;

            foreach (var rule in activeRules)
            {
                if (!rule.isEnabled) continue;

                // Condition 1: All 3 match specific symbol
                if (rule.condition == RuleConditionType.AllMatchSpecific)
                {
                    if (r1 == rule.symbol && r2 == rule.symbol && r3 == rule.symbol)
                    {
                        matchedWinRule = rule;
                        break;
                    }
                }
                // Condition 2: All 3 match any identical symbol
                else if (rule.condition == RuleConditionType.AllMatchAny)
                {
                    if (r1 == r2 && r2 == r3)
                    {
                        matchedWinRule = rule;
                        break;
                    }
                }
                // Condition 3: Count of specific symbol
                else if (rule.condition == RuleConditionType.CountSpecific)
                {
                    int count = 0;
                    if (r1 == rule.symbol) count++;
                    if (r2 == rule.symbol) count++;
                    if (r3 == rule.symbol) count++;

                    if (count == rule.requiredCount)
                    {
                        matchedWinRule = rule;
                        break;
                    }
                }
                // Condition 4: Matching pair triggers respin
                else if (rule.condition == RuleConditionType.AnyPairRespin)
                {
                    if (!isFreeSpinActive)
                    {
                        if (r1 == r2)
                        {
                            matchedRespinRule = rule;
                            respinTarget = 2; // Reel 3 respins
                        }
                        else if (r2 == r3)
                        {
                            matchedRespinRule = rule;
                            respinTarget = 0; // Reel 1 respins
                        }
                        else if (r1 == r3)
                        {
                            matchedRespinRule = rule;
                            respinTarget = 1; // Reel 2 respins
                        }
                    }
                }
            }

            // 1. Check Winning Rule
            if (matchedWinRule != null)
            {
                result.isWin = true;
                result.multiplier = matchedWinRule.multiplier;
                result.winTitle = matchedWinRule.winTitle;
                result.isJackpot = matchedWinRule.multiplier >= 100;
                result.triggersFreeSpins = matchedWinRule.bonus == BonusType.FreeSpins;
                result.winAmount = Mathf.RoundToInt(bet * matchedWinRule.multiplier * bonusMultiplier);
                return result;
            }

            // 2. Check Respin Rule
            if (matchedRespinRule != null && respinTarget != -1)
            {
                result.triggersLuckyRespin = true;
                result.respinReelIndex = respinTarget;
                result.winTitle = $"⚡ LUCKY RESPIN! (Reel {respinTarget + 1}) ⚡";
                return result;
            }

            return result;
        }
    }
}
