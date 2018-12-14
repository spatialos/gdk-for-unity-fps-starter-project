using System.Collections.Generic;
using Improbable.Gdk.Core;
using Improbable.Gdk.Mobile;
using UnityEngine;
using UnityEngine.Serialization;

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
        private WorkerConnector workerConnector;
        private ITileProvider tileProvider;

        public static List<TileEnabler> LevelTiles => Instance.tileProvider.LevelTiles;

        public static WorkerConnector ClientWorkerConnector => Instance.workerConnector;
        public static ConnectionController ConnectionController => Instance.connectionController;
        public static ScreenUIController ScreenUIController => Instance.screenUIController;

        public static void CreateClient()
        {
            Instance.CreateClientWorker();
        }

        private void Start()
        {
            Instance = this;
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
            workerConnector = currentClientWorker.GetComponent<WorkerConnector>();
            tileProvider = workerConnector as ITileProvider;
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
