using Fps;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectRepresentation;
using UnityEditor;
using UnityEngine;

namespace Improbable.Gdk.Safezone
{
    public class ZoneShrinker : MonoBehaviour
    {
        [Require] private SafeZone.Requirable.Reader SafeZoneReader;

        private Renderer sphereRenderer;

        private void Start()
        {
            sphereRenderer = GetComponentInChildren<Renderer>();
        }

        private void OnEnable()
        {
            SafeZoneReader.ScaleUpdated += _ => SyncRadius();
            SafeZoneReader.PlayingUpdated += PlayingUpdated;
            SyncRadius();
        }

        private void PlayingUpdated(BlittableBool newPlaying)
        {
            Debug.LogFormat("{0} render enabled {1}", name, newPlaying);
            sphereRenderer.enabled = newPlaying;
        }

        private void SyncRadius()
        {
            gameObject.transform.localScale = Vector3.one * SafeZoneReader.Data.Scale;
        }

        private void Update()
        {
            if (SafeZoneReader.Data.Playing && SafeZoneReader.Data.Shrink)
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
