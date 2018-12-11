using System;
using System.Diagnostics;
using Unity.Entities;

namespace Fps
{
    public static class WorkerUtils
    {
        public const string UnityClient = "UnityClient";
        public const string AndroidClient = "AndroidClient";
        public const string iOSClient = "iOSClient";

        public const string UnityGameLogic = "UnityGameLogic";

        public const string SimulatedPlayerCoorindator = "SimulatedPlayerCoordinator";
        public const string SimulatedPlayer = "SimulatedPlayer";

        public static void AddClientSystems(World world)
        {
            throw new NotImplementedException();
        }
    }
}
