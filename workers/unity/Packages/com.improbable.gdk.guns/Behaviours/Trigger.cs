using UnityEngine;

namespace Improbable.Gdk.Guns
{
    public class Trigger
    {
        private float triggerExpires;
        private readonly float timeout;

        public Trigger(float timeout)
        {
            this.timeout = timeout;
        }

        public void Fire()
        {
            triggerExpires = Time.time + timeout;
        }

        public bool Consume()
        {
            var live = Peek();
            triggerExpires = 0;
            return live;
        }

        public bool Peek()
        {
            return triggerExpires >= Time.time;
        }
    }
}
