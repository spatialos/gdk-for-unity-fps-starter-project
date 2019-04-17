using System.Collections.Generic;
using System.Linq;

namespace Fps
{
    public readonly struct DeploymentData
    {
        public readonly string Name;
        public readonly int CurrentPlayers;
        public readonly int MaxPlayers;
        public readonly bool IsAvailable;

        public DeploymentData(string name, int currentPlayers, int maxPlayers, bool isAvailable)
        {
            Name = name;
            CurrentPlayers = currentPlayers;
            MaxPlayers = maxPlayers;
            IsAvailable = isAvailable;
        }

        public static DeploymentData CreateFromTags(string deploymentName, IReadOnlyList<string> tags)
        {
            var playerCount = 0;
            var maxPlayerCount = 0;
            var isAvailable = false;
            foreach (var tag in tags)
            {
                if (tag.StartsWith("players"))
                {
                    playerCount = int.Parse(tag.Split('_').Last());
                }
                else if (tag.StartsWith("max_players"))
                {
                    maxPlayerCount = int.Parse(tag.Split('_').Last());
                }
                else if (tag.StartsWith("status"))
                {
                    var state = tag.Split('_').Last();
                    isAvailable = state == "lobby" || state == "running";
                }
            }

            return new DeploymentData(deploymentName, playerCount, maxPlayerCount, isAvailable);
        }
    }
}
