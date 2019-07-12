using UnityEngine;

namespace Improbable.Gdk.Movement
{
    public interface IMotorExtension
    {
        void BeforeMove(Vector3 toMove);

        bool IsOverrideAir();
    }
}
