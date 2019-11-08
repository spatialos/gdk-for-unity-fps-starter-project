using System;
using UnityEngine;

namespace Fps.ObjectPooling
{
    [RequireComponent(typeof(LineRenderer))]
    public class PoolableLineRenderer : BasePoolableObject
    {
        private LineRenderer lineRenderer;

        public LineRenderer Renderer
        {
            get
            {
                if (lineRenderer == null)
                {
                    lineRenderer = GetComponentInChildren<LineRenderer>();
                }

                return lineRenderer;
            }
        }

        public override void OnPoolEnable(Action returnToPoolMethod)
        {
            base.OnPoolEnable(returnToPoolMethod);
            Renderer.SetPosition(0, Vector3.zero);
            Renderer.SetPosition(1, Vector3.zero);
        }
    }
}
