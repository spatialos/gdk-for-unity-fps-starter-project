using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class DeploymentListScreenManager : ConnectionStatusManager
    {
        public Button JoinButton;
        public Button BackButton;

        [SerializeField] private Color BackgroundColor1;
        [SerializeField] private Color BackgroundColor2;
        [SerializeField] private Color HighlightedColor;
        [SerializeField] private Color HighlightedTextColor;
        [SerializeField] private Color UnavailableTextColor;
        [SerializeField] private Color DefaultTextColor;
        [SerializeField] private Table DeploymentListTable;

        private const int maxRows = 11; // Includes header
        private int currentlyHighlightedEntry = -1;
        private DeploymentData[] deploymentList;

        public bool IsHighlightedDeploymentAvailable()
        {
            if (currentlyHighlightedEntry < 0
                || deploymentList == null
                || currentlyHighlightedEntry >= deploymentList.Length)
            {
                return false;
            }

            return deploymentList[currentlyHighlightedEntry].IsAvailable;
        }

        public string GetChosenDeployment()
        {
            if (IsHighlightedDeploymentAvailable())
            {
                return deploymentList[currentlyHighlightedEntry].Name;
            }

            return null;
        }

        public void SetDeployments(DeploymentData[] deployments)
        {
            var highlightedDeploymentName = string.Empty;
            if (deploymentList != null && currentlyHighlightedEntry != -1)
            {
                highlightedDeploymentName = deploymentList[currentlyHighlightedEntry].Name;
            }

            currentlyHighlightedEntry = -1;
            DeploymentListTable.ClearEntries();

            for (var i = 0; i < Mathf.Min(maxRows - 1, deployments.Length); i++)
            {
                AddDeploymentToTable(deployments[i], i);
                if (deployments[i].Name == highlightedDeploymentName)
                {
                    SelectEntry(i);
                }
            }

            deploymentList = deployments;
        }

        private void Update()
        {
            if (currentlyHighlightedEntry == -1
                && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                && deploymentList.Length > 0)
            {
                SelectEntry(0);
                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentlyHighlightedEntry == 0)
                {
                    return;
                }

                SelectEntry(currentlyHighlightedEntry - 1);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentlyHighlightedEntry == deploymentList.Length - 1)
                {
                    return;
                }

                SelectEntry(currentlyHighlightedEntry + 1);
            }


            if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                && currentlyHighlightedEntry >= 0)
            {
                TryPressJoinButton();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TryPressBackButton();
            }
        }

        private void TryPressJoinButton()
        {
            if (!JoinButton.enabled)
            {
                return;
            }

            JoinButton.onClick.Invoke();
        }

        private void TryPressBackButton()
        {
            if (!BackButton.enabled)
            {
                return;
            }

            BackButton.onClick.Invoke();
        }

        private void AddDeploymentToTable(DeploymentData deployment, int index)
        {
            var entry = (DeploymentTableEntry) DeploymentListTable.AddEntry();
            var entryButton = entry.GetComponent<Button>();
            entryButton.onClick.AddListener(() => SelectEntry(index));
            entry.SetData(deployment);
            entry.SetAllTextVisuals(deployment.IsAvailable ? DefaultTextColor : UnavailableTextColor, false);
            entry.Background.color = index % 2 == 0 ? BackgroundColor1 : BackgroundColor2;
        }

        private void SelectEntry(int index)
        {
            if (currentlyHighlightedEntry >= 0)
            {
                UnhighlightEntry(currentlyHighlightedEntry);
            }

            var entry = DeploymentListTable.GetEntry(index);
            entry.Background.color = HighlightedColor;
            entry.SetAllTextVisuals(HighlightedTextColor, deploymentList[index].IsAvailable);

            currentlyHighlightedEntry = index;
        }

        private void UnhighlightEntry(int index)
        {
            var entry = DeploymentListTable.GetEntry(index);
            entry.Background.color = index % 2 == 0 ? BackgroundColor1 : BackgroundColor2;
            entry.SetAllTextVisuals(deploymentList[index].IsAvailable ? DefaultTextColor : UnavailableTextColor, false);
        }
    }
}
