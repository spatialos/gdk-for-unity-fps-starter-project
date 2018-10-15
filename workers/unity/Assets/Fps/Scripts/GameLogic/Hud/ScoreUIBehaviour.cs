using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using UnityEngine;

namespace Fps
{
    public class ScoreUIBehaviour : MonoBehaviour
    {
        [Require] private ScoreComponent.Requirable.Reader ScoreReader;

        private ScreenUIController screenUIController;

        private void OnEnable()
        {
            screenUIController = FindObjectOfType<ScreenUIController>();

            ScoreReader.ComponentUpdated += SetCounts;
        }

        private void SetCounts(ScoreComponent.Update updateData)
        {
            if (screenUIController != null)
            {
                if (updateData.Kills.HasValue)
                {
                    screenUIController.SetKillCount(updateData.Kills.Value);
                }

                if (updateData.Deaths.HasValue)
                {
                    screenUIController.SetDeathCount(updateData.Deaths.Value);
                }
            }
        }
    }
}
