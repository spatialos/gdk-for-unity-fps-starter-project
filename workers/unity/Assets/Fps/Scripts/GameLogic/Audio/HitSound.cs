using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fps.GameLogic.Audio
{
    /// <summary>
    ///  Implements impact sounds. Attach to HitVFX prefab 
    /// </summary>
    [RequireComponent(typeof(AudioRandomiser))] 
    public class HitSound : MonoBehaviour
    {
        private AudioRandomiser audioRandomiser;

        private void Awake()
        {
            audioRandomiser = gameObject.GetComponent<AudioRandomiser>();
        }

        private void OnEnable()
        {
            audioRandomiser.Play();
        }
    }
}
