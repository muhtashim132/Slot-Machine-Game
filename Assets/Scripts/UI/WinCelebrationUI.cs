using System;
using System.Collections;
using SlotMachine.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMachine.UI
{
    /// <summary>
    /// Handles win celebratory popups, animated payout count-up tickers,
    /// and particle celebrations for Jackpots and Mega Wins.
    /// </summary>
    public class WinCelebrationUI : MonoBehaviour
    {
        public static WinCelebrationUI Instance { get; private set; }

        [Header("Celebration Modal")]
        [SerializeField] private GameObject winModal;
        [SerializeField] private Text winTitleText;
        [SerializeField] private Text winAmountText;
        [SerializeField] private Button continueButton;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (continueButton != null)
                    continueButton.onClick.AddListener(DismissCelebration);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Shows celebration modal with animated payout ticker and fanfare.
        /// </summary>
        public void ShowCelebration(string title, int amount, bool isJackpot, Action onComplete = null)
        {
            StopAllCoroutines();
            StartCoroutine(CelebrationRoutine(title, amount, isJackpot, onComplete));
        }

        private IEnumerator CelebrationRoutine(string title, int amount, bool isJackpot, Action onComplete)
        {
            if (isJackpot)
                SoundManager.Instance?.PlayJackpotWin();
            else
                SoundManager.Instance?.PlaySmallWin();

            if (winModal != null)
            {
                winModal.SetActive(true);
                if (winTitleText != null) winTitleText.text = title;
            }

            // Animate coin ticker counting up
            float duration = isJackpot ? 1.6f : 0.8f;
            float elapsed = 0f;
            SoundManager.Instance?.PlayCoinPayout();

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                // Ease out
                float ease = 1f - Mathf.Pow(1f - progress, 2f);
                int currentNumber = Mathf.RoundToInt(Mathf.Lerp(0, amount, ease));

                if (winAmountText != null)
                    winAmountText.text = $"+{currentNumber:N0}";

                yield return null;
            }

            if (winAmountText != null)
                winAmountText.text = $"+{amount:N0}";

            // Auto dismiss after a moment if auto spin is active
            yield return new WaitForSeconds(isJackpot ? 2.5f : 1.2f);
            DismissCelebration();
            onComplete?.Invoke();
        }

        public void DismissCelebration()
        {
            if (winModal != null)
                winModal.SetActive(false);
        }
    }
}
