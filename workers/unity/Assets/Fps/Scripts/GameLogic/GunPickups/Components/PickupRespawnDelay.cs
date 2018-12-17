using Unity.Entities;

namespace Fps.GunPickups
{
	public struct PickupRespawnDelay : IComponentData
	{
		public float RechargeTime;
	}
}
