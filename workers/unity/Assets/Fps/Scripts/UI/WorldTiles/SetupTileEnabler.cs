using UnityEngine;

namespace Fps
{
    public class SetupTileEnabler : MonoBehaviour
    {
        private void OnEnable()
        {
            foreach (var levelTile in ClientWorkerHandler.LevelTiles)
            {
                levelTile.PlayerTransform = transform;
            }
        }
    }
}
