using System.IO;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.DeploymentManager;
using UnityEditor;
using UnityEngine;

namespace Fps
{
    public class SnapshotMenu : MonoBehaviour
    {
        private static readonly string DefaultSnapshotPath =
            Path.Combine(Application.dataPath, "../../../snapshots/default.snapshot");

        private static readonly string CloudSnapshotPath =
            Path.Combine(Application.dataPath, "../../../snapshots/cloud.snapshot");

        private static readonly string SessionSnapshotPath =
            Path.Combine(Application.dataPath, "../../../snapshots/session.snapshot");

        private static void GenerateSnapshot(Snapshot snapshot)
        {
            snapshot.AddEntity(FpsEntityTemplates.Spawner(Coordinates.Zero));
        }

        [MenuItem("SpatialOS/Generate FPS Snapshot")]
        private static void GenerateFpsSnapshot()
        {
            var localSnapshot = new Snapshot();
            var cloudSnapshot = new Snapshot();
            var sessionSnapshot = new Snapshot();

            GenerateSnapshot(localSnapshot);
            GenerateSnapshot(cloudSnapshot);
            GenerateSnapshot(sessionSnapshot);

            // The local snapshot is identical to the cloud snapshot, but also includes a simulated player coordinator
            // trigger.
            var simulatedPlayerCoordinatorTrigger = FpsEntityTemplates.SimulatedPlayerCoordinatorTrigger();
            localSnapshot.AddEntity(simulatedPlayerCoordinatorTrigger);
            sessionSnapshot.AddEntity(FpsEntityTemplates.DeploymentState());

            SaveSnapshot(DefaultSnapshotPath, localSnapshot);
            SaveSnapshot(CloudSnapshotPath, cloudSnapshot);
            SaveSnapshot(SessionSnapshotPath, sessionSnapshot);
        }

        private static void SaveSnapshot(string path, Snapshot snapshot)
        {
            snapshot.WriteToFile(path);
            Debug.LogFormat("Successfully generated initial world snapshot at {0}", path);
        }
    }
}
