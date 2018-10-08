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
            Instance = this;
            objectPools = new Dictionary<GameObject, IObjectPool>();
        }

        public static ObjectPool<T> GetOrCreateObjectPool<T>(GameObject prefab, int prePoolCount = 0)
            where T : IPoolableObject
        {
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
