using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMachine.Core
{
    /// <summary>
    /// Central audio manager managing sound effects, audio pooling, volume, and muting.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Clips")]
        [SerializeField] private AudioClip leverPullClip;
        [SerializeField] private AudioClip reelSpinLoopClip;
        [SerializeField] private AudioClip reelStopClip;
        [SerializeField] private AudioClip smallWinClip;
        [SerializeField] private AudioClip jackpotWinClip;
        [SerializeField] private AudioClip coinPayoutClip;
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip respinSuspenseClip;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource loopSource;

        private bool isMuted = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeSources()
        {
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (loopSource == null)
            {
                loopSource = gameObject.AddComponent<AudioSource>();
                loopSource.loop = true;
                loopSource.playOnAwake = false;
            }
        }

        public void PlayLeverPull() => PlayOneShot(leverPullClip, 0.9f);
        public void PlayReelStop() => PlayOneShot(reelStopClip, 0.85f);
        public void PlaySmallWin() => PlayOneShot(smallWinClip, 0.9f);
        public void PlayJackpotWin() => PlayOneShot(jackpotWinClip, 1.0f);
        public void PlayCoinPayout() => PlayOneShot(coinPayoutClip, 0.75f);
        public void PlayButtonClick() => PlayOneShot(buttonClickClip, 0.6f);
        public void PlayRespinSuspense() => PlayOneShot(respinSuspenseClip, 0.95f);

        public void StartReelSpinLoop()
        {
            if (isMuted || loopSource == null || reelSpinLoopClip == null) return;
            loopSource.clip = reelSpinLoopClip;
            loopSource.volume = 0.5f;
            loopSource.Play();
        }

        public void StopReelSpinLoop()
        {
            if (loopSource != null && loopSource.isPlaying)
            {
                loopSource.Stop();
            }
        }

        private void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            if (isMuted || clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip, volume);
        }

        public bool ToggleMute()
        {
            isMuted = !isMuted;
            if (sfxSource != null) sfxSource.mute = isMuted;
            if (loopSource != null) loopSource.mute = isMuted;
            return isMuted;
        }

        public bool IsMuted => isMuted;
    }
}
