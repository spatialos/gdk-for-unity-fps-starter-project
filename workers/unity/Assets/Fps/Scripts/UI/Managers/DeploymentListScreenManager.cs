using System.Collections.Generic;
using Fps.StateMachine;
using Fps.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Fps.UI
{
    public class DeploymentListScreenManager : MonoBehaviour
    {
        [SerializeField] private ConnectionStateMachine stateMachine;
        [SerializeField] private Color backgroundColor1;
        [SerializeField] private Color backgroundColor2;
        [SerializeField] private Color highlightedColor;
        [SerializeField] private Color highlightedTextColor;
        [SerializeField] private Color unavailableTextColor;
        [SerializeField] private Color defaultTextColor;
        [SerializeField] private Table deploymentListTable;

        private const int maxRows = 11; // Includes header
        private int highlightedIndex = -1;
        private List<DeploymentData> deployments = new List<DeploymentData>();

        public void SetDeployments(List<DeploymentData> newDeployments)
        {
            if (newDeployments == null)
            {
                return;
            }

            deployments = newDeployments;
            deployments.Sort();

            var highlightedDeploymentName = string.Empty;
            if (IsHighlightedIndexValid())
            {
                highlightedDeploymentName = deployments[highlightedIndex].Name;
            }

            highlightedIndex = -1;
            deploymentListTable.ClearEntries();

            for (var i = 0; i < Mathf.Min(maxRows - 1, deployments.Count); i++)
            {
                AddDeploymentToTable(deployments[i], i);
                if (deployments[i].Name == highlightedDeploymentName)
                {
                    SelectEntry(i);
                }
            }
        }

        private bool IsHighlightedIndexValid()
        {
            return highlightedIndex >= 0 && highlightedIndex < deployments.Count;
        }

        private void Update()
        {
            if (deployments.Count == 0)
            {
                return;
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
                if (highlightedIndex == deployments.Count - 1)
                {
                    return;
                }

                SelectEntry(highlightedIndex + 1);
            }
        }

        private void AddDeploymentToTable(DeploymentData deployment, int index)
        {
            var entry = (DeploymentTableEntry) deploymentListTable.AddEntry();
            var entryButton = entry.GetComponent<Button>();
            entryButton.onClick.AddListener(() => SelectEntry(index));
            entry.SetData(deployment);
            SetDefaultVisuals(index);
        }

        private void SelectEntry(int index)
        {
            if (IsHighlightedIndexValid())
            {
                SetDefaultVisuals(highlightedIndex);
            }

            var entry = deploymentListTable.GetEntry(index);
            entry.Background.color = highlightedColor;
            entry.SetAllTextVisuals(highlightedTextColor, deployments[index].IsAvailable);

            highlightedIndex = index;
            stateMachine.Blackboard.Deployment = deployments[index].Name;
        }

        private void SetDefaultVisuals(int index)
        {
            var entry = deploymentListTable.GetEntry(index);
            var isAvailable = deployments[index].IsAvailable;
            entry.Background.color = index % 2 == 0 ? backgroundColor1 : backgroundColor2;
            entry.SetAllTextVisuals(isAvailable ? defaultTextColor : unavailableTextColor, false);
        }
    }
}
