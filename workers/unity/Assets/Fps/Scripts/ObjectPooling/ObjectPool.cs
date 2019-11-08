using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fps.ObjectPooling
{
    public interface IObjectPool
    {
    }

    public class ObjectPool<T> : IObjectPool where T : Component, IPoolableObject
    {
        private readonly GameObject prefab;
        private readonly Transform instanceParent;
        private readonly Stack<T> instantiatedPoolableObjects = new Stack<T>();

        public ObjectPool(GameObject prefab, Transform instanceParent, int prePool = 0)
        {
            this.instanceParent = instanceParent;
            if (prefab.GetComponent<T>() == null)
            {
                Debug.LogErrorFormat("The object you are trying to pool does not contain a {0}.", typeof(T).Name);
                return;
            }

            this.prefab = prefab;

            for (var n = 0; n < prePool; n++)
            {
                var prePooledObject = CreatePoolableObject();
                prePooledObject.OnPoolDisable();
                instantiatedPoolableObjects.Push(prePooledObject);
            }
        }

        public T Get()
        {
            if (instantiatedPoolableObjects == null)
            {
                throw new InvalidOperationException("Cannot call Get(). The underlying pooled object storage is null.");
            }

            var pooledObject = instantiatedPoolableObjects.Count > 0
                ? instantiatedPoolableObjects.Pop()
                : CreatePoolableObject();

            pooledObject.OnPoolEnable(() => { Return(pooledObject); });
            return pooledObject;
        }

        private GameObject CreateInstance()
        {
            return Object.Instantiate(prefab, instanceParent);
        }

        private T CreatePoolableObject()
        {
            return CreateInstance().GetComponent<T>();
        }

        private void Return(T pooledObject)
        {
            if (instantiatedPoolableObjects == null)
            {
                throw new InvalidOperationException("The underlying pooled object storage is null.");
            }

            pooledObject.OnPoolDisable();
            pooledObject.GameObject.transform.SetParent(instanceParent);
            instantiatedPoolableObjects.Push(pooledObject);
        }
    }
}
