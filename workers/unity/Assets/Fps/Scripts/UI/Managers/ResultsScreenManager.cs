﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class ResultsScreenManager : MonoBehaviour
    {
        public Button DoneButton;

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
            if (DoneButton == null)
            {
                throw new NullReferenceException("Missing reference to the done button.");
            }

            if (resultsTable == null)
            {
                throw new NullReferenceException("Missing reference to the results table.");
            }

            if (rankDivider == null)
            {
                throw new NullReferenceException("Missing reference to the rank divider.");
            }
        }

        public void SetResults(ResultsData[] results, int playerRank)
        {
            resultsTable.ClearEntries();
            currentBgColor = backgroundStripColor1;

            AdjustTableHeight(playerRank);

            for (var i = 0; i < Mathf.Min(10, results.Length); i++)
            {
                AddPlayerToTable(results[i], isLocalPlayer: playerRank == (i + 1));
                currentBgColor = i % 2 == 0 ? backgroundStripColor2 : backgroundStripColor1;
            }

            if (playerRank >= 10)
            {
                AddPlayerToTable(results[playerRank], isLocalPlayer: true);
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
            Debug.Log($"is local player {isLocalPlayer}");
            var entry = (ResultsTableEntry) resultsTable.AddEntry();
            entry.SetData(data);
            entry.SetAllTextVisuals(isLocalPlayer ? localPlayerTextColor : defaultTextColor, isLocalPlayer);
            entry.Background.color = isLocalPlayer ? localPlayerBackgroundColor : currentBgColor;
        }
    }
}
