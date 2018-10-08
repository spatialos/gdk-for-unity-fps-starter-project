using UnityEditor;
using UnityEngine;

namespace Improbable.Gdk.Guns
{
    [RequireComponent(typeof(Animator))]
    public class GunIK : MonoBehaviour, IRecoil
    {
        private Animator animator;

        private bool recoil;
        private float recoilDistance;
        private float elapsedRecoil;
        private float currentRecoilDistance;

        [SerializeField] private bool applyRecoil = true;
        [SerializeField] private bool userFirstPersonGrip;

        private GunSettings.ThirdPersonRecoilSettings hipRecoil;
        private GunSettings.ThirdPersonRecoilSettings aimRecoil;
        private GunSettings.ThirdPersonRecoilSettings currentRecoilSettings;

        [SerializeField] private GunSocket gunSocket;

        public void Recoil(bool aiming)
        {
            currentRecoilSettings = aiming ? aimRecoil : hipRecoil;
            recoilDistance = currentRecoilSettings.RecoilStrength * 0.05f;
            elapsedRecoil = 0.0f;
            if (recoil)
            {
                var currentRecoilPercentage = recoilDistance <= 0.0f
                    ? 0.0f
                    : Mathf.Clamp(currentRecoilDistance / recoilDistance, 0.0f, 1.0f);
                elapsedRecoil = currentRecoilPercentage * currentRecoilSettings.RecoilTime;
            }

            recoil = true;
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (!gunSocket)
            {
                gunSocket = GetComponent<GunSocket>();
            }

            if (!gunSocket)
            {
                enabled = false;
            }
        }

        private void OnAnimatorIK(int layer)
        {
            var recoilOffset = GetRecoilOffset();
            if (applyRecoil)
            {
                var rightHandBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
                SetSocketIK(AvatarIKGoal.RightHand, rightHandBone.position, gunSocket.Gun.Handle.rotation, recoilOffset);
            }

            var gripToUse = userFirstPersonGrip ? gunSocket.Gun.FirstPersonGrip : gunSocket.Gun.Grip;
            SetSocketIK(AvatarIKGoal.LeftHand, gripToUse, applyRecoil ? recoilOffset : Vector3.zero);
        }

        private Vector3 GetRecoilOffset()
        {
            if (!recoil)
            {
                return Vector3.zero;
            }

            float currentRecoilPercentage;
            if (elapsedRecoil < currentRecoilSettings.RecoilTime)
            {
                currentRecoilPercentage = elapsedRecoil / currentRecoilSettings.RecoilTime;
            }
            else
            {
                currentRecoilPercentage =
                    1 - (elapsedRecoil - currentRecoilSettings.RecoilTime) / currentRecoilSettings.ResetTime;
            }

            currentRecoilDistance = currentRecoilPercentage * recoilDistance;

            var rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            var recoilOffset = rightHand.right * -currentRecoilDistance;

            elapsedRecoil += Time.deltaTime;
            if (elapsedRecoil > currentRecoilSettings.RecoilTime + currentRecoilSettings.ResetTime)
            {
                recoil = false;
            }

            return recoilOffset;
        }

        private void SetSocketIK(AvatarIKGoal goal, UnityEngine.Transform socket, Vector3 offset)
        {
            SetSocketIK(goal, socket.position, socket.rotation, offset);
        }

        private void SetSocketIK(AvatarIKGoal goal, Vector3 position, Quaternion rotation, Vector3 offset)
        {
            animator.SetIKPositionWeight(goal, 1);
            animator.SetIKRotationWeight(goal, 1);
            animator.SetIKPosition(goal, position + offset);
            animator.SetIKRotation(goal, rotation);
        }

        public void SetRecoilSettings(GunSettings.RecoilSettings hipRecoilSettings,
            GunSettings.RecoilSettings aimRecoilSettings)
        {
            hipRecoil = hipRecoilSettings.ThirdPersonRecoil;
            aimRecoil = aimRecoilSettings.ThirdPersonRecoil;
        }
    }
}
