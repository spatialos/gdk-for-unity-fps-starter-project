using System;
using Improbable.Gdk.Core;
using Improbable.Worker.Core;
using Unity.Entities;
using UnityEngine;

namespace Fps
{
    public class MetricSendSystem : ComponentSystem
    {
        private Connection connection;

        private DateTime timeOfNextUpdate;
        private DateTime timeOfLastUpdate;

        private const double TimeBetweenMetricUpdatesSecs = 2;
        private const int DefaultTargetFrameRate = 60;

        private double targetFps;

        private int lastFrameCount;
        private double lastSentFps;

        // We use exponential smoothing for the FPS metric
        // larger value == more smoothing, 0 = no smoothing
        // 0 <= smoothing < 1
        private const double smoothing = 0;

        private static readonly Metrics WorkerMetrics = new Metrics();

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            connection = World.GetExistingManager<WorkerSystem>().Connection;

            targetFps = Application.targetFrameRate == -1
                ? DefaultTargetFrameRate
                : Application.targetFrameRate;
        }

        protected override void OnUpdate()
        {
            if (connection == null)
            {
                return;
            }

            if (DateTime.Now >= timeOfNextUpdate)
            {
                var dynamicFps = CalculateFps();
                WorkerMetrics.GaugeMetrics["Dynamic.FPS"] = dynamicFps;
                WorkerMetrics.GaugeMetrics["Unity used heap size"] = GC.GetTotalMemory(false);
                WorkerMetrics.Load = CalculateLoad(dynamicFps);

                connection.SendMetrics(WorkerMetrics);

                lastSentFps = dynamicFps;
                timeOfLastUpdate = DateTime.Now;
                timeOfNextUpdate = DateTime.Now.AddSeconds(TimeBetweenMetricUpdatesSecs);
            }
        }

        // Load defined as performance relative to target FPS.
        // i.e. a load of 0.5 means that the worker is hitting the target FPS
        // but achieving less than half the target FPS takes load above 1.0
        private double CalculateLoad(double dynamicFps)
        {
            return Math.Max(0.0d, 0.5d * targetFps / dynamicFps);
        }


        private double CalculateFps()
        {
            var frameCount = Time.frameCount - lastFrameCount;
            lastFrameCount = Time.frameCount;
            var rawFps = frameCount / (DateTime.Now - timeOfLastUpdate).TotalSeconds;
            return (rawFps * (1 - smoothing)) + (lastSentFps * smoothing);
        }
    }
}
