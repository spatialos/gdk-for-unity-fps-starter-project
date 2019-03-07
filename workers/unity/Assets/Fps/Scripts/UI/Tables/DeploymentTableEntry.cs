using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class DeploymentTableEntry : TableEntry
    {
        [SerializeField] private Text DeploymentNameText;
        [SerializeField] private Text PlayersText;
        [SerializeField] private Text MaxPlayersText;
        [SerializeField] private Text AvailabilityText;


        public void SetData(DeploymentData data)
        {
            DeploymentNameText.text = data.Name;
            PlayersText.text = data.CurrentPlayers.ToString();
            MaxPlayersText.text = data.MaxPlayers.ToString();
            AvailabilityText.text = data.IsAvailable ? "Available" : "Unavailable";
        }
    }
}
