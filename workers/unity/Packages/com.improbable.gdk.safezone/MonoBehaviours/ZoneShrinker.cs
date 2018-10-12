using Fps;
using Improbable.Gdk.GameObjectRepresentation;
using UnityEngine;

namespace Improbable.Gdk.Safezone
{
    public class ZoneShrinker : MonoBehaviour
    {
        [Require] private SafeZone.Requirable.Reader SafeZoneReader;

        private void OnEnable()
        {
            SafeZoneReader.ScaleUpdated += _ => SyncRadius();
            SyncRadius();
        }

        private void SyncRadius()
        {
            gameObject.transform.localScale = Vector3.one * SafeZoneReader.Data.Scale;
        }

        private void Update()
        {
            if (SafeZoneReader.Data.Shrink)
            {
                var scale = gameObject.transform.localScale;
                var toShrink = Vector3.one * ZoneSettings.ShrinkRate * Time.deltaTime;

                if (scale.x - toShrink.x < ZoneSettings.MinRadius)
                {
                    scale = Vector3.one * ZoneSettings.MinRadius;
                }
                else
                {
                    scale -= toShrink;
                }

                gameObject.transform.localScale = scale;
            }
        }
    }
}
