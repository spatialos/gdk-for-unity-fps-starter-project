using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fps
{
    public class ResultsData
    {
        public readonly int Rank;
        public readonly string PlayerName;
        public readonly int Kills;
        public readonly int Deaths;
        public readonly float KillDeathRatio;

        public ResultsData(int rank, string playerName, int kills, int deaths)
        {
            Rank = rank;
            PlayerName = playerName;
            Kills = kills;
            Deaths = deaths;
            KillDeathRatio = deaths == 0 ? kills : kills / (float) deaths;
        }
    }
}
