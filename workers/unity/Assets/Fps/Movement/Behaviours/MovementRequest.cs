using UnityEngine;

namespace Fps.Movement
{
    public struct MovementRequest
    {
        public int Timestamp;
        public Vector3 Movement;

        public MovementRequest(int timestamp, Vector3 movement)
        {
            Timestamp = timestamp;
            Movement = movement;
        }
    }
}
