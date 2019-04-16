namespace Fps
{
    public struct DeploymentData
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
    }
}
