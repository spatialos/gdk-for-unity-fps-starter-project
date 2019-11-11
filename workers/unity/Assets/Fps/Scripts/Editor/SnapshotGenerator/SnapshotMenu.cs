using System.IO;
using Fps.Config;
using Improbable;
using Improbable.Gdk.Core;
using UnityEditor;
using UnityEngine;

namespace Fps.Editor
{
    public class SnapshotMenu : MonoBehaviour
    {
        private static readonly string DefaultSnapshotPath =
            Path.Combine(Application.dataPath, "../../../snapshots/default.snapshot");

        private static readonly string CloudSnapshotPath =
            Path.Combine(Application.dataPath, "../../../snapshots/cloud.snapshot");

        private static readonly string SessionSnapshotPath =
            Path.Combine(Application.dataPath, "../../../snapshots/session.snapshot");

        [MenuItem("SpatialOS/Generate FPS Snapshot")]
        private static void GenerateFpsSnapshot()
        {
            SaveSnapshot(DefaultSnapshotPath, GenerateDefaultSnapshot());
            SaveSnapshot(CloudSnapshotPath, GenerateDefaultSnapshot());
            SaveSnapshot(SessionSnapshotPath, GenerateSessionSnapshot());
        }

        private static Snapshot GenerateDefaultSnapshot()
        {
            var snapshot = new Snapshot();
            snapshot.AddEntity(FpsEntityTemplates.Spawner(Coordinates.Zero));
            return snapshot;
        }

        private static Snapshot GenerateSessionSnapshot()
        {
            var snapshot = new Snapshot();
            snapshot.AddEntity(FpsEntityTemplates.Spawner(Coordinates.Zero));
            snapshot.AddEntity(FpsEntityTemplates.DeploymentState());
            return snapshot;
        }

        private static void SaveSnapshot(string path, Snapshot snapshot)
        {
            snapshot.WriteToFile(path);
            Debug.LogFormat("Successfully generated initial world snapshot at {0}", path);
        }
    }
}
