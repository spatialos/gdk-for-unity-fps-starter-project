using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class DeploymentListScreenController : MonoBehaviour
    {
        public Color BackgroundColor1;
        public Color BackgroundColor2;
        public Color HighlightedColor;
        public Color HighlightedTextColor;
        public Color UnavailableTextColor;
        public Color DefaultTextColor;
        public Button JoinButton;
        public Button BackButton;
        public Table deploymentListTable;
        public ConnectionStatusUIController ConnectionStatusUIController;

        public DeploymentData[] DeploymentList { get; private set; }

        private Color currentBgColor;
        private const int maxRows = 11; // Includes header
        private int currentlyHighlightedEntry = -1;

        private void Awake()
        {
            JoinButton.enabled = false;
        }

        public bool IsAvailableDeploymentHighlighted()
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
            if (IsAvailableDeploymentHighlighted())
            {
                return DeploymentList[currentlyHighlightedEntry].Name;
            }

            return null;
        }

        public void SetDeployments(DeploymentData[] deployments)
        {
            deploymentListTable.ClearEntries();
            currentBgColor = BackgroundColor1;

            // TODO If previously-selected deployment is in list of new deployments, try and retain its position in the table
            for (var i = 0; i < Mathf.Min(maxRows - 1, deployments.Length); i++)
            {
                AddDeploymentToTable(deployments[i], i);
                currentBgColor = i % 2 == 0 ? BackgroundColor2 : BackgroundColor1;
            }

            DeploymentList = deployments;
            currentlyHighlightedEntry = -1; // TODO Select previously-selected deployment if present in new list
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
            var entry = (DeploymentTableEntry) deploymentListTable.AddEntry();
            var entryButton = entry.GetComponent<Button>();
            entryButton.onClick.AddListener(() => SelectEntry(index));
            entry.SetData(deployment);
            entry.SetAllTextVisuals(deployment.IsAvailable ? DefaultTextColor : UnavailableTextColor, false);
            entry.SetBackgroundColor(currentBgColor);
        }

        private void SelectEntry(int index)
        {
            if (currentlyHighlightedEntry >= 0)
            {
                UnhighlightEntry(currentlyHighlightedEntry);
            }

            HighlightEntry(index);

            currentlyHighlightedEntry = index;

            JoinButton.enabled = IsAvailableDeploymentHighlighted();
        }

        private void HighlightEntry(int index)
        {
            var entry = deploymentListTable.GetEntry(index);
            entry.SetBackgroundColor(HighlightedColor);
            entry.SetAllTextVisuals(HighlightedTextColor, DeploymentList[index].IsAvailable);
        }

        private void UnhighlightEntry(int index)
        {
            var entry = deploymentListTable.GetEntry(index);
            entry.SetBackgroundColor(index % 2 == 0 ? BackgroundColor1 : BackgroundColor2);
            entry.SetAllTextVisuals(DeploymentList[index].IsAvailable ? DefaultTextColor : UnavailableTextColor, false);
        }
    }
}
