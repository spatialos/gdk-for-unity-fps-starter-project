using System;
using UnityEngine;

namespace Fps.ObjectPooling
{
    public interface IPoolableObject
    {
        GameObject GameObject { get; }

        void OnPoolEnable(Action returnToPoolMethod);
        void OnPoolDisable();
    }
}
