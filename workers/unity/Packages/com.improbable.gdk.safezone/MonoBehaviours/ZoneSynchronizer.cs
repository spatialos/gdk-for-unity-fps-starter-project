using System;
using System.Collections;
using Improbable.Common;
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
        private const string flagMaxRadius = "safe_zone_max_radius";

        [Require] private SafeZone.Requirable.Writer SafeZoneWriter;
        [Require] private SafeZone.Requirable.CommandRequestHandler SafeZoneRequests;
        [Require] private WorldCommands.Requirable.WorldCommandRequestSender WorldCommandSender;

        private EntityId myEntityId;
        private long sendTime;
        private float maxRadiusOverride = ZoneSettings.MaxRadius;

        private void OnEnable()
        {
            myEntityId = GetComponent<SpatialOSComponent>().SpatialEntityId;

            sendTime = DateTime.Now.Ticks;

            SafeZoneRequests.OnStartRequest += OnStartRequest;
            SafeZoneRequests.OnStopRequest += OnStopRequest;
            SafeZoneRequests.OnResetRequest += OnResetRequest;

            StartCoroutine(CheckZoneFlags());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator CheckZoneFlags()
        {
            while (true)
            {
                var spatial = GetComponent<SpatialOSComponent>();
                if (spatial != null)
                {
                    var maxRadius = spatial.Worker.Connection.GetWorkerFlag(flagMaxRadius);
                    if (maxRadius != null && float.TryParse(maxRadius, out var newMax))
                    {
                        maxRadiusOverride = newMax;
                    }
                }

                yield return new WaitForSeconds(5);
            }
        }

        private void OnResetRequest(SafeZone.Reset.RequestResponder response)
        {
            SafeZoneWriter.Send(new SafeZone.Update()
            {
                Scale = new Option<float>(maxRadiusOverride)
            });
            response.SendResponse(new Empty());
        }

        private void OnStopRequest(SafeZone.Stop.RequestResponder response)
        {
            SafeZoneWriter.Send(new SafeZone.Update()
            {
                Playing = new Option<BlittableBool>(false)
            });
            response.SendResponse(new Empty());
        }

        private void OnStartRequest(SafeZone.Start.RequestResponder response)
        {
            SafeZoneWriter.Send(new SafeZone.Update()
            {
                Playing = new Option<BlittableBool>(true)
            });
            response.SendResponse(new Empty());
        }

        private void Update()
        {
            if (gameObject.transform.localScale.x <= ZoneSettings.MinRadius)
            {
                SafeZoneWriter.Send(new SafeZone.Update()
                {
                    Shrink = new Option<BlittableBool>(false)
                });

                if (gameObject.transform.localScale.x <= 0)
                {
                    WorldCommandSender.DeleteEntity(myEntityId);
                }

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
