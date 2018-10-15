using Boo.Lang;
using Improbable.Gdk.Safezone;
using UnityEngine;

namespace Fps
{
    public class SpawnPoints : MonoBehaviour
    {
        public class SpawnPoint
        {
            public Vector3 SpawnPosition;
            public float SpawnYaw;
            public float SpawnPitch;

            public void Deconstruct(out Vector3 position, out float yaw, out float pitch)
            {
                position = SpawnPosition;
                yaw = SpawnYaw;
                pitch = SpawnPitch;
            }
        }

        public static SpawnPoint[] spawnPointList;
        [SerializeField] private bool snapsToGround;

        public static SpawnPoint GetRandomSpawnPoint()
        {
            if (spawnPointList == null || spawnPointList.Length == 0)
            {
                Debug.LogWarning("No spawn points found - using origin.");
                return new SpawnPoint();
            }

            return spawnPointList[Random.Range(0, spawnPointList.Length)];
        }

        private void Start()
        {
            var spawnPoints = FindSpawnPoints();
            var worldOffset = transform.root.position;

            var tempList = new List<SpawnPoint>();

            for (var n = 0; n < spawnPoints.Length; n++)
            {
                var spawnPointTransform = spawnPoints[n].transform;
                var spawnPointPosition = spawnPointTransform.position;
                if (SnapToGround(spawnPointPosition, out spawnPointPosition))
                {
                    tempList.Add(new SpawnPoint
                    {
                        SpawnPosition = spawnPointPosition - worldOffset,
                        SpawnYaw = spawnPointTransform.eulerAngles.y,
                        SpawnPitch = 0
                    });
                }

                Destroy(spawnPoints[n]);
            }

            spawnPointList = tempList.ToArray();
        }

        private GameObject[] FindSpawnPoints()
        {
            var spawnPointIndicators = transform.parent.GetComponentsInChildren<SpawnPointIndicator>();

            var gameObjects = new GameObject[spawnPointIndicators.Length];

            for (var i = 0; i < spawnPointIndicators.Length; i++)
            {
                gameObjects[i] = spawnPointIndicators[i].gameObject;
            }

            return gameObjects;
        }

        private bool SnapToGround(Vector3 position, out Vector3 hitPoint)
        {
            LayerMask allLayerMask = ~0;
            if (Physics.Raycast(new Ray(position, Vector3.down), out var hitInfo, 100f,
                allLayerMask))
            {
                hitPoint = hitInfo.point;
                return true;
            }

            hitPoint = position;
            return false;
        }
    }
}
