namespace Fps.UI
{
    public struct ResultsData
    {
        public int Rank;
        public readonly string PlayerName;
        public readonly int Kills;
        public readonly int Deaths;
        public readonly float KillDeathRatio;

        public ResultsData(string playerName, int kills, int deaths)
        {
            PlayerName = playerName;
            Kills = kills;
            Deaths = deaths;
            KillDeathRatio = deaths == 0 ? kills : kills / (float) deaths;
            Rank = -1;
        }
    }
}
