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

        private Color currentBgColor;
        private Table deploymentListTable;
        private const int maxRows = 11; // Includes header
        private int currentlyHighlightedEntry = -1;
        private DeploymentData[] deploymentList;
        private FrontEndUIController frontEndUiController;

        private void Awake()
        {
            Debug.Assert(JoinButton != null);
            JoinButton.onClick.AddListener(OnJoinButtonPressed);

            Debug.Assert(BackButton != null);
            BackButton.onClick.AddListener(OnBackButtonPressed);

            deploymentListTable = GetComponentInChildren<Table>();
            Debug.Assert(deploymentListTable != null);

            frontEndUiController = GetComponentInParent<FrontEndUIController>();
            Debug.Assert(frontEndUiController != null);

            ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChanged;
        }

        private void OnConnectionStateChanged(ConnectionStateReporter.State state, string information)
        {
            if (state == ConnectionStateReporter.State.Connecting)
            {
                JoinButton.enabled = false;
            }
            else
            {
                // Are there any cases we don't want to reenable the join button on a valid deployment?
                JoinButton.enabled = IsHighlightedAvailable();
            }
        }

        private void OnJoinButtonPressed()
        {
            ConnectionStateReporter.TryConnect(); // TODO replace with actual connect-to-deployment logic
        }

        private void OnBackButtonPressed()
        {
            frontEndUiController.SwitchToSessionScreen();
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

            deploymentList = deployments;
            currentlyHighlightedEntry = -1; // TODO Select previously-selected deployment if present in new list
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

            JoinButton.enabled = IsHighlightedAvailable();
        }

        private bool IsHighlightedAvailable()
        {
            if (currentlyHighlightedEntry < 0
                || deploymentList == null
                || currentlyHighlightedEntry >= deploymentList.Length)
            {
                return false;
            }

            return deploymentList[currentlyHighlightedEntry].IsAvailable;
        }

        private void HighlightEntry(int index)
        {
            var entry = deploymentListTable.GetEntry(index);
            entry.SetBackgroundColor(HighlightedColor);
            entry.SetAllTextVisuals(HighlightedTextColor, deploymentList[index].IsAvailable);
        }

        private void UnhighlightEntry(int index)
        {
            var entry = deploymentListTable.GetEntry(index);
            entry.SetBackgroundColor(index % 2 == 0 ? BackgroundColor1 : BackgroundColor2);
            entry.SetAllTextVisuals(deploymentList[index].IsAvailable ? DefaultTextColor : UnavailableTextColor, false);
        }
    }
}
