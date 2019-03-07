﻿using UnityEngine;
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

        private const int MaxVisibleRowCount = 12; // Includes header
        private Table resultsTable;
        private Color currentBgColor;

        private FrontEndUIController frontEndUiController;

        private void Awake()
        {
            Debug.Assert(DoneButton != null);
            DoneButton.onClick.AddListener(DoneButtonPressed);

            resultsTable = GetComponentInChildren<Table>();
            Debug.Assert(resultsTable != null);

            frontEndUiController = GetComponentInParent<FrontEndUIController>();
            Debug.Assert(frontEndUiController != null);
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

        private void DoneButtonPressed()
        {
            frontEndUiController.SwitchToSessionScreen();
        }

        /// <param name="playerRank">zero-based value of player's rank.</param>
        public void SetResults(ResultsData[] results, int playerRank)
        {
            resultsTable.ClearEntries();
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
            entry.SetAllTextVisuals(isLocalPlayer ? LocalPlayerTextColor : DefaultTextColor, isLocalPlayer);
            entry.SetBackgroundColor(isLocalPlayer ? LocalPlayerBackgroundColor : currentBgColor);
        }
    }
}
