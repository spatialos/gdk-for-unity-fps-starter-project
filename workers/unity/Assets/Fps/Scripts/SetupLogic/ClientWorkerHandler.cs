using UnityEngine;

namespace Fps
{
    public class ClientWorkerHandler : MonoBehaviour
    {
        private static ClientWorkerHandler Instance;

        [SerializeField] private GameObject clientWorkerPrefab;
        [SerializeField] private GameObject canvasCameraObj;
        [SerializeField] private ScreenUIController screenUIController;

        private GameObject currentClientWorker;
        private ConnectionController connectionController;
        private ClientWorkerConnector workerConnector;

        public static ClientWorkerConnector ClientWorkerConnector => Instance.workerConnector;
        public static ConnectionController ConnectionController => Instance.connectionController;

        public static void CreateClient()
        {
            Instance.CreateClientWorker();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            CreateClientWorker();
        }

        private void Update()
        {
            // Check if the Client worker has been disconnected, and remove it if so.
            DisconnectCheck();
        }

        private void CreateClientWorker()
        {
            if (currentClientWorker != null)
            {
                Destroy(currentClientWorker);
            }

            currentClientWorker = Instantiate(clientWorkerPrefab);
            workerConnector = currentClientWorker.GetComponent<ClientWorkerConnector>();
            connectionController = currentClientWorker.GetComponent<ConnectionController>();
            connectionController.InformOfUI(canvasCameraObj, screenUIController);
        }

        private void DisconnectCheck()
        {
            if (workerConnector != null
                && workerConnector.Worker != null
                && !workerConnector.Worker.Connection.IsConnected)
            {
                screenUIController.OnDisconnect();
                Destroy(currentClientWorker);
            }
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
