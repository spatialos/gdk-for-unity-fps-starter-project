using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class DeploymentListScreenManager : ConnectionStatusManager
    {
        public Color BackgroundColor1;
        public Color BackgroundColor2;
        public Color HighlightedColor;
        public Color HighlightedTextColor;
        public Color UnavailableTextColor;
        public Color DefaultTextColor;
        public Button JoinButton;
        public Button BackButton;
        public Table DeploymentListTable;

        public DeploymentData[] DeploymentList { get; private set; }

        private Color currentBgColor;
        private const int maxRows = 11; // Includes header
        private int currentlyHighlightedEntry = -1;

        private void Awake()
        {
            JoinButton.enabled = false;
        }

        public bool IsHighlightedDeploymentAvailable()
        {
            if (currentlyHighlightedEntry < 0
                || DeploymentList == null
                || currentlyHighlightedEntry >= DeploymentList.Length)
            {
                return false;
            }

            return DeploymentList[currentlyHighlightedEntry].IsAvailable;
        }

        public string GetChosenDeployment()
        {
            if (IsHighlightedDeploymentAvailable())
            {
                return DeploymentList[currentlyHighlightedEntry].Name;
            }

            return null;
        }

        public void SetDeployments(DeploymentData[] deployments)
        {
            var highlightedDeploymentName = string.Empty;
            if (DeploymentList != null && currentlyHighlightedEntry != -1)
            {
                highlightedDeploymentName = DeploymentList[currentlyHighlightedEntry].Name;
            }

            currentlyHighlightedEntry = -1;
            DeploymentListTable.ClearEntries();
            currentBgColor = BackgroundColor1;

            for (var i = 0; i < Mathf.Min(maxRows - 1, deployments.Length); i++)
            {
                AddDeploymentToTable(deployments[i], i);
                currentBgColor = i % 2 == 0 ? BackgroundColor2 : BackgroundColor1;
                if (deployments[i].Name == highlightedDeploymentName)
                {
                    currentlyHighlightedEntry = i;
                }
            }

            DeploymentList = deployments;
        }

        private void Update()
        {
            if (currentlyHighlightedEntry == -1
                && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                && DeploymentList.Length > 0)
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
                if (currentlyHighlightedEntry == DeploymentList.Length - 1)
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

        private void OnEnable()
        {
            if (currentlyHighlightedEntry >= 0)
            {
                UnhighlightEntry(currentlyHighlightedEntry);
            }

            currentlyHighlightedEntry = -1;
        }

        private void AddDeploymentToTable(DeploymentData deployment, int index)
        {
            var entry = (DeploymentTableEntry) DeploymentListTable.AddEntry();
            var entryButton = entry.GetComponent<Button>();
            entryButton.onClick.AddListener(() => SelectEntry(index));
            entry.SetData(deployment);
            entry.SetAllTextVisuals(deployment.IsAvailable ? DefaultTextColor : UnavailableTextColor, false);
            entry.Background.color = currentBgColor;
        }

        private void SelectEntry(int index)
        {
            if (currentlyHighlightedEntry >= 0)
            {
                UnhighlightEntry(currentlyHighlightedEntry);
            }

            var entry = DeploymentListTable.GetEntry(index);
            entry.Background.color = HighlightedColor;
            entry.SetAllTextVisuals(HighlightedTextColor, DeploymentList[index].IsAvailable);

            currentlyHighlightedEntry = index;
            JoinButton.enabled = IsHighlightedDeploymentAvailable();
        }

        private void UnhighlightEntry(int index)
        {
            var entry = DeploymentListTable.GetEntry(index);
            entry.Background.color = index % 2 == 0 ? BackgroundColor1 : BackgroundColor2;
            entry.SetAllTextVisuals(DeploymentList[index].IsAvailable ? DefaultTextColor : UnavailableTextColor, false);
        }
    }
}
