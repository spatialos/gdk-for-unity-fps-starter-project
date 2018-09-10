using System.Collections;
using System.Collections.Generic;
using Improbable.Gdk.GameObjectRepresentation;
using UnityEngine;

namespace Improbable.Gdk.Guns
{
    public class ClientGunChanging : MonoBehaviour
    {
        [Require] GunComponent.Requirable.CommandRequestSender gunRequestSender;
		private SpatialOSComponent spatialOSComponent;

		private void OnEnable()
		{
			spatialOSComponent = GetComponent<SpatialOSComponent>();
		}

		public void AttemptChangeSlot(int slot)
		{
			gunRequestSender.SendChangeSlotRequest(spatialOSComponent.SpatialEntityId, new ChangeCurrentSlotRequest(slot));
		}

		public void AttemptPickUpGun(int gunId)
		{
			gunRequestSender.SendPickUpGunRequest(spatialOSComponent.SpatialEntityId, new PickUpGunRequest(gunId));
		}
    }
}
