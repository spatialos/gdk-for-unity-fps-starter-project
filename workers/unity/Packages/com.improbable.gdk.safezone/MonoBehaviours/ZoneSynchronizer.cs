using System;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Safezone;
using Improbable.Worker;
using UnityEngine;

namespace Fps
{
    public class ZoneSynchronizer : MonoBehaviour
    {
        [Require] private SafeZone.Requirable.Writer SafeZoneWriter;
        [Require] private WorldCommands.Requirable.WorldCommandRequestSender WorldCommandSender;

        private EntityId myEntityId;
        private long sendTime;

        private void OnEnable()
        {
            myEntityId = GetComponent<SpatialOSComponent>().SpatialEntityId;

            sendTime = DateTime.Now.Ticks;
        }

        private void Update()
        {
            if (gameObject.transform.localScale.x <= ZoneSettings.MinRadius)
            {
                SafeZoneWriter.Send(new SafeZone.Update()
                {
                    Shrink = new Option<BlittableBool>(false)
                });

                WorldCommandSender.DeleteEntity(myEntityId);
                return;
            }

            if (DateTime.Now.Ticks > sendTime)
            {
                sendTime = DateTime.Now.AddSeconds(ZoneSettings.SyncInterval).Ticks;
                SafeZoneWriter.Send(new SafeZone.Update()
                {
                    Scale = gameObject.transform.localScale.x
                });
            }
        }
    }
}
