using System.Collections;
using UnityEngine;

public class SessionFlowTester : MonoBehaviour
{
    public void Start()
    {
        ConnectionStateReporter.SetState(ConnectionStateReporter.State.None);
        Invoke(nameof(StartGathering), 0);
    }

    public void StartGathering()
    {
        StartCoroutine(nameof(GatherTask));
    }

    public void GatheringDone(bool succeeded)
    {
        ConnectionStateReporter.SetState(succeeded
            ? ConnectionStateReporter.State.DeploymentListAvailable
            : ConnectionStateReporter.State.FailedToGetDeploymentList);
    }

    private const float GatherTime = 4;
    private float timeGathering;

    private IEnumerator GatherTask()
    {
        ConnectionStateReporter.SetState(ConnectionStateReporter.State.GettingDeploymentList);
        while (timeGathering < GatherTime)
        {
            yield return null;
            timeGathering += Time.deltaTime;
            if (!Input.GetKeyDown(KeyCode.F))
            {
                continue;
            }

            timeGathering = GatherTime;
            GatheringDone(false);
            yield break;
        }

        GatheringDone(true);
    }
}
