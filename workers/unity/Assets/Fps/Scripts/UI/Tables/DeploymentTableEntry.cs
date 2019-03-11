using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class DeploymentTableEntry : TableEntry
    {
        [SerializeField] private Text DeploymentNameText;
        [SerializeField] private Text PlayersText;
        [SerializeField] private Text MaxPlayersText;
        [SerializeField] private GameObject AvailableSymbol;
        [SerializeField] private GameObject UnavailableSymbol;


        public void SetData(DeploymentData data)
        {
            DeploymentNameText.text = data.Name;
            PlayersText.text = data.CurrentPlayers.ToString();
            MaxPlayersText.text = data.MaxPlayers.ToString();
            AvailableSymbol.SetActive(data.IsAvailable);
            UnavailableSymbol.SetActive(!data.IsAvailable);
        }
    }
}
