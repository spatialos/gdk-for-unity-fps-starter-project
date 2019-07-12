using System;
using UnityEngine;

namespace Improbable.Gdk.ObjectPooling
{
    public class BasePoolableObject : MonoBehaviour, IPoolableObject
    {
        private Action returnToPool;

        public void ReturnToPool()
        {
            returnToPool();
        }

        public GameObject GameObject => gameObject;

        public virtual void OnPoolEnable(Action returnToPoolMethod)
        {
            returnToPool = returnToPoolMethod;
            gameObject.SetActive(true);
        }

        public virtual void OnPoolDisable()
        {
            gameObject.SetActive(false);
        }
    }
}
