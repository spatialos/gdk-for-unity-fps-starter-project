using Improbable.Worker.CInterop;
using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class ResultsScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject rankDivider;
        public Button DoneButton;

        public Color BackgroundStripColor1;
        public Color BackgroundStripColor2;
        public Color LocalPlayerBackgroundColor;
        public Color LocalPlayerTextColor;
        public Color DefaultTextColor;
        public Table ResultsTable;

        private const int MaxVisibleRowCount = 12; // Includes header
        private Color currentBgColor;

        public void SetResults(ResultsData[] results, int playerRank)
        {
            ResultsTable.ClearEntries();
            currentBgColor = BackgroundStripColor1;

            AdjustTableHeight(playerRank);

            for (var i = 0; i < Mathf.Min(10, results.Length); i++)
            {
                AddPlayerToTable(results[i], playerRank == i);
                currentBgColor = i % 2 == 0 ? BackgroundStripColor2 : BackgroundStripColor1;
            }

            if (playerRank >= 10)
            {
                AddPlayerToTable(results[playerRank], true);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter)
                || Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.Escape))
            {
                DoneButton.onClick.Invoke();
            }
        }

        private void AdjustTableHeight(int playerRank)
        {
            if (playerRank < 10)
            {
                var rect = ResultsTable.GetComponent<RectTransform>();
                rect.offsetMin = new Vector2(rect.offsetMin.x,
                    (MaxVisibleRowCount - 1) * ResultsTable.EntryHeight * -.5f);
                rect.offsetMax = new Vector2(rect.offsetMax.x,
                    (MaxVisibleRowCount - 1) * ResultsTable.EntryHeight * .5f);
                rankDivider.SetActive(false);
            }
            else
            {
                var rect = ResultsTable.GetComponent<RectTransform>();
                rect.offsetMin = new Vector2(rect.offsetMin.x, MaxVisibleRowCount * ResultsTable.EntryHeight * -.5f);
                rect.offsetMax = new Vector2(rect.offsetMax.x, MaxVisibleRowCount * ResultsTable.EntryHeight * .5f);
                rankDivider.SetActive(true);
            }
        }

        private void AddPlayerToTable(ResultsData data, bool isLocalPlayer)
        {
            var entry = (ResultsTableEntry) ResultsTable.AddEntry();
            entry.SetData(data);
            entry.SetAllTextVisuals(isLocalPlayer ? LocalPlayerTextColor : DefaultTextColor, isLocalPlayer);
            entry.SetBackgroundColor(isLocalPlayer ? LocalPlayerBackgroundColor : currentBgColor);
        }
    }
}
