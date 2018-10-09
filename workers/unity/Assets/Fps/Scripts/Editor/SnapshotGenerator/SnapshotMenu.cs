using System.IO;
using Improbable.Gdk.Core;
using UnityEditor;
using UnityEngine;

namespace Fps
{
    public class SnapshotMenu : MonoBehaviour
    {
        public static readonly string DefaultSnapshotPath =
            Path.Combine(Application.dataPath, "../../../snapshots/default.snapshot");

        [MenuItem("SpatialOS/Generate FPS Snapshot")]
        private static void GenerateDefaultSnapshot()
        {
            var snapshot = new Snapshot();

            var spawner = FpsEntityTemplates.Spawner();
            snapshot.AddEntity(spawner);

            var SimulatedPlayerCoordinatorTrigger = FpsEntityTemplates.SimulatedPlayerCoordinatorTrigger();
            snapshot.AddEntity(SimulatedPlayerCoordinatorTrigger);

            SaveSnapshot(snapshot);
        }

        private static void SaveSnapshot(Snapshot snapshot)
        {
            snapshot.WriteToFile(DefaultSnapshotPath);
            Debug.LogFormat("Successfully generated initial world snapshot at {0}", DefaultSnapshotPath);
        }
    }
}
