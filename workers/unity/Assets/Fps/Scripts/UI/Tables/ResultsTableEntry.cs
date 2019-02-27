using Fps;
using UnityEngine;
using UnityEngine.UI;

public class ResultsTableEntry : TableEntry
{
    [SerializeField] private Text RankText;
    [SerializeField] private Text PlayerNameText;
    [SerializeField] private Text KillsText;
    [SerializeField] private Text DeathsText;
    [SerializeField] private Text KDText;


    public void SetData(ResultsData data)
    {
        RankText.text = data.Rank.ToString();
        PlayerNameText.text = data.PlayerName;
        KillsText.text = data.Kills.ToString();
        DeathsText.text = data.Deaths.ToString();
        KDText.text = data.KillDeathRatio.ToString("N2");
    }
}
