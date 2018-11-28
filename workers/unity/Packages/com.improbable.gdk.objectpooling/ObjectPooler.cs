using System.Collections.Generic;
using UnityEngine;

namespace Improbable.Gdk.ObjectPooling
{
    public class ObjectPooler : MonoBehaviour
    {
        private static ObjectPooler Instance { get; set; }

        private Dictionary<GameObject, IObjectPool> objectPools;

        private void Awake()
        {
            // Destroy the ObjectPooler component being created if an Instance already exists
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            objectPools = new Dictionary<GameObject, IObjectPool>();
        }

        public static ObjectPool<T> GetOrCreateObjectPool<T>(GameObject prefab, int prePoolCount = 0)
            where T : IPoolableObject
        {
            if (Instance == null)
            {
                // Creation of the ObjectPooler component sets the Instance field
                new GameObject("ObjectPool").AddComponent<ObjectPooler>();
            }

            return Instance.GetOrCreateObjectPoolInternal<T>(prefab, prePoolCount);
        }

        private ObjectPool<T> GetOrCreateObjectPoolInternal<T>(GameObject prefab, int prePoolCount)
            where T : IPoolableObject
        {
            if (!objectPools.TryGetValue(prefab, out var objectPool))
            {
                objectPool = new ObjectPool<T>(prefab, transform, prePoolCount);
                objectPools.Add(prefab, objectPool);
            }
            else
            {
                if (objectPool.GetType() != typeof(ObjectPool<T>))
                {
                    Debug.LogErrorFormat("There is already a pool of type {0} for prefab {1}",
                        objectPool.GetType().Name, prefab.name);
                    return null;
                }
            }

            return (ObjectPool<T>) objectPool;
        }
    }
}
