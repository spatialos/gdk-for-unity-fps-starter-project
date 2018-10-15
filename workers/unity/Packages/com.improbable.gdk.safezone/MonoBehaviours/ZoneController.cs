using Improbable.Common;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Safezone;
using Improbable.Worker;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    private readonly EntityId zoneId = new EntityId(3);

    [Require] private SafeZone.Requirable.CommandRequestSender SafeZoneSender;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SafeZoneSender.SendStartRequest(zoneId, new Empty());
            return;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            SafeZoneSender.SendStopRequest(zoneId, new Empty());
            return;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SafeZoneSender.SendResetRequest(zoneId, new Empty());
        }
    }
}
