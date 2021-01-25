using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;
using Unity.Entities;
using UnityEngine;

namespace Fps.Config
{
    public static class OneTimeInitialisation
    {
        private static bool initialized;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            // Setup template to use for player on connecting client
            PlayerLifecycleConfig.CreatePlayerEntityTemplate = FpsEntityTemplates.Player;
            PlayerLifecycleConfig.MaxNumFailedPlayerHeartbeats = 5;
            PlayerLifecycleConfig.PlayerCreatorEntityId = FpsEntityTemplates.PlayerCreatorEntityId;
        }
    }
}
