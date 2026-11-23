using System;
using System.Collections;
using SlotMachine.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlotMachine.UI
{
    /// <summary>
    /// Interactive mechanical lever on the slot machine.
    /// Supports click and downward drag gestures, playing a mechanical ratchet sound,
    /// swapping to the pulled-down sprite, triggering the spin, and springing back up smoothly.
    /// </summary>
    public class LeverController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public event Action OnLeverPulled;

        [Header("Sprites")]
        [SerializeField] private Image leverImage;
        [SerializeField] private Sprite uprightSprite; // slot-machine2
        [SerializeField] private Sprite pulledDownSprite; // slot-machine3

        [Header("State")]
        private bool isInteractable = true;
        private bool isDown = false;
        private Vector2 dragStartPosition;

        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isInteractable || isDown) return;
            dragStartPosition = eventData.position;
            PullLever();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isInteractable || isDown) return;
            if (eventData.position.y - dragStartPosition.y < -30f)
            {
                PullLever();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Lever springs back automatically via coroutine
        }

        /// <summary>
        /// Executes the physical pull-down and spring-return animation.
        /// </summary>
        public void PullLever()
        {
            if (!isInteractable || isDown) return;
            isDown = true;
            isInteractable = false;

            StopAllCoroutines();
            StartCoroutine(LeverPullRoutine());
        }

        private IEnumerator LeverPullRoutine()
        {
            // 1. Play mechanical pull audio
            SoundManager.Instance?.PlayLeverPull();

            // 2. Switch to pulled-down frame
            if (leverImage != null && pulledDownSprite != null)
            {
                leverImage.sprite = pulledDownSprite;
            }

            // 3. Notify game manager that lever was pulled
            OnLeverPulled?.Invoke();

            // 4. Hold at bottom for brief mechanical dwell
            yield return new WaitForSeconds(0.22f);

            // 5. Spring back up to upright position
            if (leverImage != null && uprightSprite != null)
            {
                leverImage.sprite = uprightSprite;
            }

            // Small overshoot rebound delay
            yield return new WaitForSeconds(0.12f);
            isDown = false;
        }
    }
}
