using System;
using System.Collections.Generic;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public class ClientWorkerHandler : MonoBehaviour
    {
        public static List<TileEnabler> LevelTiles => Instance.tileProvider.LevelTiles;
        public static ScreenUIController ScreenUIController => Instance.screenUIController;

        private static ClientWorkerHandler Instance;

        [SerializeField] private GameObject clientWorkerPrefab;
        [SerializeField] private ScreenUIController screenUIController;
        [SerializeField] private bool UseSessionBasedFlow;

        private GameObject currentClientWorker;
        private ConnectionController connectionController;
        private WorkerConnector workerConnector;
        private ITileProvider tileProvider;

        public static bool IsInSessionBasedGame => Instance.UseSessionBasedFlow;
        public static string Deployment;

        private void Awake()
        {
            Instance = this;
            screenUIController.gameObject.SetActive(false);
        }

        private void Start()
        {
            screenUIController.gameObject.SetActive(true);

            if (UseSessionBasedFlow)
            {
                GetComponent<SessionFlowController>().enabled = true;
            }
            else
            {
                CreateClientWorker();
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.Connecting);
            }
        }

        private void Update()
        {
            // Check if the Client worker has been disconnected, and remove it if so.
            DisconnectCheck();
        }

        public void CreateClientWorker()
        {
            if (currentClientWorker != null)
            {
                Destroy(currentClientWorker);
            }

            currentClientWorker = Instantiate(clientWorkerPrefab, transform.position, Quaternion.identity);
            workerConnector = currentClientWorker.GetComponent<WorkerConnector>();
            tileProvider = workerConnector as ITileProvider;
            connectionController = currentClientWorker.GetComponent<ConnectionController>();
        }

        private void DisconnectCheck()
        {
            if (workerConnector != null
                && workerConnector.Worker != null
                && workerConnector.Worker.Connection.GetConnectionStatusCode() != ConnectionStatusCode.Success)
            {
                connectionController.OnDisconnected();
                Destroy(currentClientWorker);

                ConnectionStateReporter.SetState(ConnectionStateReporter.State.None);
            }
        }
    }
}
