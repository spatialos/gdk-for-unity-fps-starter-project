using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class LobbyScreenController : MonoBehaviour
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
        private Table lobbyTable;
        private const int maxRows = 11; // Includes header
        private int currentlyHighlightedEntry = -1;
        private DeploymentData[] deploymentList;

        private FrontEndUIController frontEndUiController;

        private void Awake()
        {
            Debug.Assert(JoinButton != null);
            JoinButton.onClick.AddListener(JoinButtonPressed);

            Debug.Assert(BackButton != null);
            BackButton.onClick.AddListener(BackButtonPressed);

            lobbyTable = GetComponentInChildren<Table>();
            Debug.Assert(lobbyTable != null);

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
                JoinButton.enabled = HighlightedIsAvailable();
            }
        }


        private void JoinButtonPressed()
        {
            ConnectionStateReporter.TryConnect();
            Debug.Log($"Joining deployment {deploymentList[currentlyHighlightedEntry].Name}");
        }

        private void BackButtonPressed()
        {
            frontEndUiController.SwitchToSessionScreen();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentlyHighlightedEntry == -1)
                {
                    SelectEntry(0);
                    return;
                }

                if (currentlyHighlightedEntry == 0)
                {
                    return;
                }

                SelectEntry(currentlyHighlightedEntry - 1);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentlyHighlightedEntry == -1)
                {
                    SelectEntry(0);
                    return;
                }

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
            lobbyTable.ClearEntries();
            currentBgColor = BackgroundColor1;

            for (var i = 0; i < Mathf.Min(maxRows - 1, deployments.Length); i++)
            {
                AddDeploymentToTable(deployments[i], i);
                currentBgColor = i % 2 == 0 ? BackgroundColor2 : BackgroundColor1;
            }

            deploymentList = deployments;
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
            var entry = lobbyTable.AddEntry();
            var entryButton = entry.GetComponent<Button>();
            entryButton.onClick.AddListener(() => SelectEntry(index));
            entry.SetTextOnColumn(0, deployment.Name);
            entry.SetTextOnColumn(1, deployment.IsAvailable ? "Available" : "Unavailable"); // TODO: Replace with image?
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

            JoinButton.enabled = HighlightedIsAvailable();
        }

        private bool HighlightedIsAvailable()
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
            var entry = lobbyTable.GetEntry(index);
            entry.SetBackgroundColor(HighlightedColor);
            entry.SetAllTextVisuals(HighlightedTextColor, deploymentList[index].IsAvailable);
        }

        private void UnhighlightEntry(int index)
        {
            var entry = lobbyTable.GetEntry(index);
            entry.SetBackgroundColor(index % 2 == 0 ? BackgroundColor1 : BackgroundColor2);
            entry.SetAllTextVisuals(deploymentList[index].IsAvailable ? DefaultTextColor : UnavailableTextColor, false);
        }

        public class DeploymentData
        {
            public readonly string Name;
            public readonly bool IsAvailable;

            public DeploymentData(string name, bool isAvailable)
            {
                Name = name;
                IsAvailable = isAvailable;
            }
        }
    }
}
