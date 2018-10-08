using System;
using UnityEngine;

namespace Improbable.Gdk.ObjectPooling
{
    public interface IPoolableObject
    {
        GameObject GameObject { get; }

        void OnPoolEnable(Action returnToPoolMethod);
        void OnPoolDisable();
    }
}
