using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fps.GameLogic.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioRandomiser : MonoBehaviour
    {
        public AudioClip[] AudioClips;
        private AudioSource audioSource;
        private bool sequencer;

        private void Awake()
        {
            audioSource = this.GetComponent<AudioSource>();
        }

        private void Start()
        {
            sequencer = false;
        }

        public void Play()
        {
            audioSource?.PlayOneShot(NextClip());
        }

        public AudioClip NextClip()
        {
            var mid = (int) (AudioClips.Length / 2);

            sequencer = !sequencer;

            if (sequencer)
            {
                return AudioClips[Random.Range(0, mid)];
            }
            else
            {
                return AudioClips[Random.Range(mid, AudioClips.Length)];
            }
        }
    }
}
