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
        private int highlightedIndex = -1;
        private DeploymentData[] deploymentList;

        private bool IsHighlightedIndexValid()
        {
            return highlightedIndex >= 0 && highlightedIndex < deploymentList.Length;
        }

        public bool IsHighlightedDeploymentAvailable()
        {
            if (!IsHighlightedIndexValid() || deploymentList == null)
            {
                return false;
            }

            return deploymentList[highlightedIndex].IsAvailable;
        }

        public string GetChosenDeployment()
        {
            if (IsHighlightedDeploymentAvailable())
            {
                return deploymentList[highlightedIndex].Name;
            }

            return null;
        }

        public void SetDeployments(DeploymentData[] deployments)
        {
            var highlightedDeploymentName = string.Empty;
            if (deploymentList != null && IsHighlightedIndexValid())
            {
                highlightedDeploymentName = deploymentList[highlightedIndex].Name;
            }

            highlightedIndex = -1;
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TryPressBackButton();
            }

            if (deploymentList.Length == 0)
            {
                return;
            }

            if (IsHighlightedIndexValid() && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
            {
                TryPressJoinButton();
            }

            if (!IsHighlightedIndexValid() && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)))
            {
                SelectEntry(0);
                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (highlightedIndex == 0)
                {
                    return;
                }

                SelectEntry(highlightedIndex - 1);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (highlightedIndex == deploymentList.Length - 1)
                {
                    return;
                }

                SelectEntry(highlightedIndex + 1);
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
            if (IsHighlightedIndexValid())
            {
                UnhighlightEntry(highlightedIndex);
            }

            var entry = DeploymentListTable.GetEntry(index);
            entry.Background.color = HighlightedColor;
            entry.SetAllTextVisuals(HighlightedTextColor, deploymentList[index].IsAvailable);

            highlightedIndex = index;
        }

        private void UnhighlightEntry(int index)
        {
            var entry = DeploymentListTable.GetEntry(index);
            entry.Background.color = index % 2 == 0 ? BackgroundColor1 : BackgroundColor2;
            entry.SetAllTextVisuals(deploymentList[index].IsAvailable ? DefaultTextColor : UnavailableTextColor, false);
        }
    }
}
