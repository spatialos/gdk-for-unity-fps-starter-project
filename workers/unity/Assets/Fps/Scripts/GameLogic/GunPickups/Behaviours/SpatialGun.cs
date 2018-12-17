using System;
using Improbable.Gdk.Guns;
using UnityEngine;

namespace Fps.GunPickups
{
    [ExecuteInEditMode]
    [SelectionBase]
    public class SpatialGun : MonoBehaviour
    {
        public SpatialGunDataProperty GunData;
        public Transform SpawnParent;

        public int GunId
        {
            get => GunData.GunId;
            set => GunData.GunId = value;
        }

        public float RespawnTime = 10f;
        public Vector3 SpawnPosition => SpawnParent.position;

        private string realName = "";
        private int nameGunId = -1;
        private bool needsUpdate;

        private void OnValidate()
        {
            if (GunDictionary.Instance == null)
            {
                GunDictionary.Instance = GunData.Guns;
            }

            // Rename to gun type
            if (realName == "" || name != realName || nameGunId != GunId)
            {
                GunSettings gunSettings = GunDictionary.Get(GunId);
                var gunName = gunSettings != null ? gunSettings.name : "InvalidGunID";

                realName = string.Format("GunPickup_{0}", gunName);
                name = realName;
                nameGunId = GunId;

                needsUpdate = true;
            }
        }

        private void Update()
        {
            if (!needsUpdate)
            {
                return;
            }

            needsUpdate = false;
            // Replace the prefab
            GunId = GunId >= GunDictionary.Count ? GunDictionary.Count - 1 : GunId;
            var gun = GunDictionary.Get(GunId);
            foreach (Transform child in SpawnParent)
            {
                DestroyImmediate(child.gameObject);
            }

            if (gun != null)
            {
                GameObject gunPrefab = gun.GunModel;
                var instantiated = Instantiate(gunPrefab, SpawnParent);
                instantiated.transform.localPosition = Vector3.zero;
                instantiated.transform.localEulerAngles = Vector3.zero;
            }
        }

        private void OnDrawGizmos()
        {
            DrawGizmos(Color.magenta);
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos(Color.green);
        }

        private void DrawGizmos(Color baseColor)
        {
            var col = Gizmos.color;
            Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.4f);
            Gizmos.DrawCube(transform.position, new Vector3(1.5f, 0.1f, 1.5f));
            Gizmos.DrawSphere(transform.position, 0.05f);
            Gizmos.DrawLine(transform.position, SpawnPosition);
            Gizmos.color = col;
        }
    }

    [Serializable]
    public struct SpatialGunDataProperty
    {
        public int GunId;
        public GunDictionary Guns;
    }
}
