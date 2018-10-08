using System;
using System.Collections;
using System.Collections.Generic;
using Improbable.Gdk.Health;
using Improbable.Gdk.ObjectPooling;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

namespace Improbable.Gdk.Ragdoll
{
    public class PoolableRagdoll : BasePoolableObject
    {
        private struct InitialBonePosition
        {
            public UnityEngine.Transform Bone;
            public Vector3 LocalPosition;
            public Quaternion LocalRotation;

            public void ResetBone()
            {
                Bone.localPosition = LocalPosition;
                Bone.localRotation = LocalRotation;
            }
        }

        [SerializeField] private float timeUntilCleanup = 10.0f;
        [SerializeField] private float ragdollLaunchForce = 50.0f;

        public float TimeUntilCleanup => timeUntilCleanup;

        public delegate void RagdollCleanedUp();

        public RagdollCleanedUp OnCleanup;

        private Collider[] childColliders;
        private Renderer[] childRenderers;
        private List<InitialBonePosition> initialBonePositions = new List<InitialBonePosition>();

        private Coroutine cleanup;

        [SerializeField] private Rigidbody centreBone;
        public Rigidbody CentreBone => centreBone;

        private void Awake()
        {
            childColliders = GetComponentsInChildren<Collider>();
            childRenderers = GetComponentsInChildren<Renderer>();

            StoreBoneTransforms();
        }

        public override void OnPoolEnable(Action returnToPoolMethod)
        {
            base.OnPoolEnable(returnToPoolMethod);
            cleanup = StartCoroutine(DelayedCleanup(timeUntilCleanup));
            SetState(true);
        }

        public override void OnPoolDisable()
        {
            base.OnPoolDisable();
            SetState(false);
            RestoreBoneTransforms();
            if (cleanup != null)
            {
                StopCoroutine(cleanup);
            }
        }

        private void StoreBoneTransforms()
        {
            initialBonePositions.Clear();

            foreach (var childTransform in GetComponentsInChildren<UnityEngine.Transform>())
            {
                initialBonePositions.Add(new InitialBonePosition
                {
                    Bone = childTransform,
                    LocalPosition = childTransform.localPosition,
                    LocalRotation = childTransform.localRotation
                });
            }
        }

        private void RestoreBoneTransforms()
        {
            foreach (var bone in initialBonePositions)
            {
                bone.ResetBone();
            }
        }

        private IEnumerator DelayedCleanup(float delay)
        {
            yield return new WaitForSeconds(delay);
            OnCleanup?.Invoke();
            ReturnToPool();
        }

        private void SetState(bool targetState)
        {
            foreach (var childCollider in childColliders)
            {
                childCollider.enabled = targetState;
            }

            foreach (var childRenderer in childRenderers)
            {
                childRenderer.enabled = targetState;
            }
        }

        public void MatchTransforms(UnityEngine.Transform reference)
        {
            MatchTransforms(reference, transform);
        }

        // Recursively match the skeleton (transforms) of the ragdoll to the reference.
        private void MatchTransforms(UnityEngine.Transform reference, UnityEngine.Transform ragdoll)
        {
            ragdoll.position = reference.position;
            ragdoll.rotation = reference.rotation;
            for (var i = 0; i < Math.Min(reference.childCount, ragdoll.childCount); i++)
            {
                var referenceChild = reference.GetChild(i);
                var ragdollChild = ragdoll.GetChild(i);
                MatchTransforms(referenceChild, ragdollChild);
            }
        }

        public void LaunchRagdoll(HealthModifier deathDetails)
        {
            // Launch the ragdoll in direction of modifier.
            var launchVector = MakeLaunchVector(
                deathDetails.Origin.ToVector3(),
                deathDetails.AppliedLocation.ToVector3());

            centreBone.AddForce(launchVector, ForceMode.Impulse);
        }

        private Vector3 MakeLaunchVector(Vector3 start, Vector3 end)
        {
            var launchVector = (end - start).normalized;
            return launchVector * ragdollLaunchForce;
        }
    }
}
