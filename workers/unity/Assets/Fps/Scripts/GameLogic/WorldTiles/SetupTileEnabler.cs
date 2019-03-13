using UnityEngine;

namespace Fps
{
    public class SetupTileEnabler : MonoBehaviour
    {
        private void OnEnable()
        {
            Debug.Log("Setting player for culling");
            Debug.Log(ClientWorkerHandler.LevelTiles.Count);
            foreach (var levelTile in ClientWorkerHandler.LevelTiles)
            {
                levelTile.PlayerTransform = transform;
            }
        }
    }
}
