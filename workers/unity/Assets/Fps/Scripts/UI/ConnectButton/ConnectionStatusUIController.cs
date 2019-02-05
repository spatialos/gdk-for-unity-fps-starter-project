using System;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionStatusUIController : MonoBehaviour
{
    public GameObject SpinnerSymbol;
    public GameObject ErrorSymbol;
    public GameObject SuccessSymbol;
    public Text StatusText;

    public string GatheringDeploymentsText = "Getting deployment list...";
    public string GatheringDeploymentsFailedText = "Failed to get deployment list!";
    public string SearchingText = "Searching for deployments...";
    public string SearchFailedText = "Failed to find deployments!";
    public string JoiningText = "Joining deployment...";
    public string SpawnFailedText = "Failed to spawn player!";
    public string WorkerDisconnectedText = "Worker was disconnected";

    private void Awake()
    {
        ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChange;
    }

    public void OnEnable()
    {
        OnConnectionStateChange(ConnectionStateReporter.CurrentState, ConnectionStateReporter.CurrentInformation);
    }

    public void OnConnectionStateChange(ConnectionStateReporter.State state, string information)
    {
        switch (state)
        {
            case ConnectionStateReporter.State.None:
                Initialize();
                break;
            case ConnectionStateReporter.State.GettingDeploymentList:
                GatheringDeployments();
                break;
            case ConnectionStateReporter.State.DeploymentListAvailable:
                DeploymentsAvailable();
                break;
            case ConnectionStateReporter.State.FailedToGetDeploymentList:
                FailedToGetDeployments(information);
                break;
            case ConnectionStateReporter.State.Connecting:
                SearchForDeployment();
                break;
            case ConnectionStateReporter.State.Connected:
                ConnectionStateReporter.TrySpawn();
                break;
            case ConnectionStateReporter.State.ConnectionFailed:
                SearchForDeploymentFailed();
                break;
            case ConnectionStateReporter.State.Spawning:
                Spawning();
                break;
            case ConnectionStateReporter.State.Spawned:
                break;
            case ConnectionStateReporter.State.SpawningFailed:
                SpawnFailed();
                break;
            case ConnectionStateReporter.State.WorkerDisconnected:
                WorkerDisconnected();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void Initialize()
    {
        StatusText.text = "";
        SetSymbol(null);
    }

    private void GatheringDeployments()
    {
        StatusText.text = GatheringDeploymentsText;
        SetSymbol(SpinnerSymbol);
    }

    private void DeploymentsAvailable()
    {
        StatusText.text = "Deployments available";
        SetSymbol(SuccessSymbol);
    }

    private void FailedToGetDeployments(string error)
    {
        StatusText.text = $"{GatheringDeploymentsFailedText} {error}";
        SetSymbol(ErrorSymbol);
    }

    private void SearchForDeployment()
    {
        StatusText.text = SearchingText;
        SetSymbol(SpinnerSymbol);
    }

    private void SearchForDeploymentFailed()
    {
        StatusText.text = SearchFailedText;
        SetSymbol(ErrorSymbol);
    }

    private void Spawning()
    {
        StatusText.text = JoiningText;
    }

    private void SpawnFailed()
    {
        StatusText.text = SpawnFailedText;
        SetSymbol(ErrorSymbol);
    }

    private void WorkerDisconnected()
    {
        StatusText.text = WorkerDisconnectedText;
        SetSymbol(ErrorSymbol);
    }

    private void SetSymbol(GameObject symbol)
    {
        Debug.Assert(symbol == null
            || symbol == ErrorSymbol
            || symbol == SpinnerSymbol
            || symbol == SuccessSymbol);

        if (ErrorSymbol != null)
        {
            ErrorSymbol.SetActive(symbol == ErrorSymbol);
        }

        if (SpinnerSymbol != null)
        {
            SpinnerSymbol.SetActive(symbol == SpinnerSymbol);
        }

        if (SuccessSymbol != null)
        {
            SuccessSymbol.SetActive(symbol == SuccessSymbol);
        }
    }
}
