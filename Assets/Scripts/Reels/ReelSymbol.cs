using System.Collections;
using SlotMachine.Data;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMachine.Reels
{
    /// <summary>
    /// Controls visual rendering, sprite assignment, and win glow animations for an individual symbol.
    /// </summary>
    public class ReelSymbol : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Image symbolImage;
        [SerializeField] private RectTransform rectTransform;

        private SymbolType currentSymbolType;

        public SymbolType CurrentSymbolType => currentSymbolType;
        public RectTransform RectTransform => rectTransform;

        private void Awake()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            if (symbolImage == null)
                symbolImage = GetComponent<Image>();
        }

        /// <summary>
        /// Configures the symbol's logical type and sprite graphic.
        /// </summary>
        public void SetSymbol(SymbolType type, Sprite sprite)
        {
            currentSymbolType = type;
            if (symbolImage != null)
            {
                symbolImage.sprite = sprite;
                symbolImage.color = Color.white;
            }
        }

        /// <summary>
        /// Plays an attractive scale pulsing animation when this symbol participates in a winning payline.
        /// </summary>
        public void PlayWinAnimation()
        {
            StopAllCoroutines();
            StartCoroutine(PulseAnimationRoutine());
        }

        public void StopWinAnimation()
        {
            StopAllCoroutines();
            if (rectTransform != null)
                rectTransform.localScale = Vector3.one;
            if (symbolImage != null)
                symbolImage.color = Color.white;
        }

        private IEnumerator PulseAnimationRoutine()
        {
            Vector3 originalScale = Vector3.one;
            Vector3 targetScale = new Vector3(1.18f, 1.18f, 1f);

            while (true)
            {
                // Pulse up
                float elapsed = 0f;
                float duration = 0.28f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                    rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                    yield return null;
                }

                // Pulse down
                elapsed = 0f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                    rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                    yield return null;
                }
            }
        }
    }
}
