using System;
using System.Collections.Generic;
using System.Linq;

namespace Fps
{
    public readonly struct DeploymentData : IComparable<DeploymentData>
    {
        public readonly string Name;
        public readonly int CurrentPlayers;
        public readonly int MaxPlayers;
        public readonly bool IsAvailable;

        private DeploymentData(string name, int currentPlayers, int maxPlayers, bool isAvailable)
        {
            Name = name;
            CurrentPlayers = currentPlayers;
            MaxPlayers = maxPlayers;
            IsAvailable = isAvailable;
        }

        public static bool TryFromTags(string deploymentName, IReadOnlyList<string> tags, out DeploymentData data)
        {
            int? playerCount = null;
            int? maxPlayerCount = null;
            bool? isAvailable = null;
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

            if (!playerCount.HasValue || !maxPlayerCount.HasValue || !isAvailable.HasValue)
            {
                data = new DeploymentData();
                return false;
            }

            data = new DeploymentData(deploymentName, playerCount.Value, maxPlayerCount.Value, isAvailable.Value);
            return true;
        }

        public int CompareTo(DeploymentData other)
        {
            return String.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }
}
