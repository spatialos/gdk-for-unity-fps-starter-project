using System.Collections.Generic;
using UnityEngine;

namespace Fps
{
    public class ResultsScreenManager : MonoBehaviour
    {
        [SerializeField] private Color backgroundStripColor1;
        [SerializeField] private Color backgroundStripColor2;
        [SerializeField] private Color localPlayerBackgroundColor;
        [SerializeField] private Color localPlayerTextColor;
        [SerializeField] private Color defaultTextColor;
        [SerializeField] private Table resultsTable;
        [SerializeField] private GameObject rankDivider;

        private const int MaxVisibleRowCount = 12; // Includes header
        private Color currentBgColor;

        private void OnValidate()
        {
            if (resultsTable == null)
            {
                throw new MissingReferenceException("Missing reference to the results table.");
            }

            if (rankDivider == null)
            {
                throw new MissingReferenceException("Missing reference to the rank divider.");
            }
        }

        public void SetResults(List<ResultsData> results, int playerRank)
        {
            resultsTable.ClearEntries();

            AdjustTableHeight(playerRank);

            for (var i = 1; i <= Mathf.Min(10, results.Count); i++)
            {
                AddPlayerToTable(results[i - 1], isLocalPlayer: playerRank == i);
                currentBgColor = i % 2 == 0 ? backgroundStripColor1 : backgroundStripColor2;
            }

            if (playerRank > 10)
            {
                AddPlayerToTable(results[playerRank - 1], isLocalPlayer: true);
            }
        }

        private void AdjustTableHeight(int playerRank)
        {
            if (playerRank < 10)
            {
                var rect = resultsTable.GetComponent<RectTransform>();
                rect.offsetMin = new Vector2(rect.offsetMin.x,
                    (MaxVisibleRowCount - 1) * resultsTable.EntryHeight * -.5f);
                rect.offsetMax = new Vector2(rect.offsetMax.x,
                    (MaxVisibleRowCount - 1) * resultsTable.EntryHeight * .5f);
                rankDivider.SetActive(false);
            }
            else
            {
                var rect = resultsTable.GetComponent<RectTransform>();
                rect.offsetMin = new Vector2(rect.offsetMin.x, MaxVisibleRowCount * resultsTable.EntryHeight * -.5f);
                rect.offsetMax = new Vector2(rect.offsetMax.x, MaxVisibleRowCount * resultsTable.EntryHeight * .5f);
                rankDivider.SetActive(true);
            }
        }

        private void AddPlayerToTable(ResultsData data, bool isLocalPlayer)
        {
            var entry = (ResultsTableEntry) resultsTable.AddEntry();
            entry.SetData(data);
            entry.SetAllTextVisuals(isLocalPlayer ? localPlayerTextColor : defaultTextColor, isLocalPlayer);
            entry.Background.color = isLocalPlayer ? localPlayerBackgroundColor : currentBgColor;
        }
    }
}
