using Fps.GameLogic.Audio;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using UnityEngine;

namespace Fps.GameLogic.Audio
{
    /// <summary>
    /// Attached to NonAuth player, listens to shot events and plays relevant shot sounds
    /// </summary>
    public class ShotListener : MonoBehaviour
    {
        [Require] ShootingComponent.Requirable.Reader shotReader;

        public AudioRandomiser shotRandomiser;

        private void OnEnable()
        {
            shotReader.OnShots += ShotReaderOnShots;
        }

        private void ShotReaderOnShots(ShotInfo shotInfo)
        {
            shotRandomiser.Play();
        }
    }
}
