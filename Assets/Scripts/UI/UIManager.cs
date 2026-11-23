using System;
using System.Collections;
using SlotMachine.Core;
using SlotMachine.Data;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMachine.UI
{
    /// <summary>
    /// Manages the game UI displays: Coin balance, Current Bet, Last Win,
    /// action buttons (Spin, Auto, Bet +/-, Max Bet), Paytable modal, and Sound toggle.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Text Displays")]
        [SerializeField] private Text coinsText;
        [SerializeField] private Text betText;
        [SerializeField] private Text winText;
        [SerializeField] private Text statusBannerText;
        [SerializeField] private Text freeSpinsBadgeText;

        [Header("Primary Action Buttons")]
        [SerializeField] private Button spinButton;
        [SerializeField] private Button autoSpinButton;
        [SerializeField] private Button betSettingsButton;
        [SerializeField] private Button increaseBetButton;
        [SerializeField] private Button decreaseBetButton;
        [SerializeField] private Button paytableButton;
        [SerializeField] private Button soundToggleButton;

        [Header("Modals & Overlays")]
        [SerializeField] private GameObject betSettingsModal;
        [SerializeField] private GameObject paytableModal;
        [SerializeField] private GameObject freeSpinsBanner;
        [SerializeField] private Text soundToggleText;

        [Header("Visual Colors")]
        [SerializeField] private Color normalTextColor = Color.white;
        [SerializeField] private Color winHighlightColor = new Color(1f, 0.85f, 0.2f); // Gold

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
            // Hook up UI button listeners
            if (spinButton != null)
                spinButton.onClick.AddListener(() => GameManager.Instance.OnSpinButtonPressed());

            if (autoSpinButton != null)
                autoSpinButton.onClick.AddListener(() => GameManager.Instance.ToggleAutoSpin());

            if (betSettingsButton != null)
                betSettingsButton.onClick.AddListener(ToggleBetSettings);

            if (increaseBetButton != null)
                increaseBetButton.onClick.AddListener(() => GameManager.Instance.IncreaseBet());

            if (decreaseBetButton != null)
                decreaseBetButton.onClick.AddListener(() => GameManager.Instance.DecreaseBet());

            if (paytableButton != null)
                paytableButton.onClick.AddListener(TogglePaytable);

            if (soundToggleButton != null)
                soundToggleButton.onClick.AddListener(ToggleSound);
        }

        public void UpdateBalance(int coins)
        {
            if (coinsText != null)
                coinsText.text = coins.ToString("N0");
        }

        public void UpdateBet(int bet)
        {
            if (betText != null)
                betText.text = bet.ToString("N0");
        }

        public void UpdateWin(int winAmount)
        {
            if (winText != null)
            {
                winText.text = winAmount > 0 ? winAmount.ToString("N0") : "0";
                winText.color = winAmount > 0 ? winHighlightColor : normalTextColor;
            }
        }

        public void SetStatusText(string message)
        {
            if (statusBannerText != null)
                statusBannerText.text = message;
        }

        public void SetSpinInteractable(bool interactable)
        {
            if (spinButton != null) spinButton.interactable = interactable;
            if (increaseBetButton != null) increaseBetButton.interactable = interactable;
            if (decreaseBetButton != null) decreaseBetButton.interactable = interactable;
            if (betSettingsButton != null) betSettingsButton.interactable = interactable;
        }

        public void SetAutoSpinActive(bool isActive)
        {
            if (autoSpinButton != null)
            {
                var text = autoSpinButton.GetComponentInChildren<Text>();
                if (text != null)
                    text.text = isActive ? "STOP AUTO" : "AUTO SPIN";
            }
        }

        public void SetFreeSpinsBanner(bool active, int remainingSpins = 0)
        {
            if (freeSpinsBanner != null)
                freeSpinsBanner.SetActive(active);

            if (freeSpinsBadgeText != null && active)
                freeSpinsBadgeText.text = $"FREE SPINS: {remainingSpins} (2X MULTIPLIER)";
        }

        public void ToggleBetSettings()
        {
            if (betSettingsModal != null)
            {
                SoundManager.Instance?.PlayButtonClick();
                if (paytableModal != null) paytableModal.SetActive(false);
                betSettingsModal.SetActive(!betSettingsModal.activeSelf);
            }
        }

        public void CloseBetSettings()
        {
            if (betSettingsModal != null)
                betSettingsModal.SetActive(false);
        }

        public void TogglePaytable()
        {
            if (paytableModal != null)
            {
                SoundManager.Instance?.PlayButtonClick();
                if (betSettingsModal != null) betSettingsModal.SetActive(false);
                paytableModal.SetActive(!paytableModal.activeSelf);
            }
        }

        public void ClosePaytable()
        {
            if (paytableModal != null)
            {
                SoundManager.Instance?.PlayButtonClick();
                paytableModal.SetActive(false);
            }
        }

        public void ToggleSound()
        {
            bool isMuted = SoundManager.Instance != null && SoundManager.Instance.ToggleMute();
            if (soundToggleText != null)
                soundToggleText.text = isMuted ? "SOUND: OFF" : "SOUND: ON";
        }
    }
}
