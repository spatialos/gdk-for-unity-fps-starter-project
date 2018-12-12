using System.Collections.Generic;
using Improbable.Gdk.Core;
using Improbable.Gdk.Mobile;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fps
{
    public class ClientWorkerHandler : MonoBehaviour
    {
        // TODO Rename this to AndroidWorkerHandler (AndroidClientWorkerHandler?), remove mobile directive switch,
        // TODO and restore original ClientWorkerHandler class.

        private static ClientWorkerHandler Instance;

        [SerializeField] private GameObject clientWorkerPrefab;
        [SerializeField] private GameObject canvasCameraObj;

        [FormerlySerializedAs("screenUIController")] [SerializeField]
        private ScreenUIController screenUIControllerDesktop;

        [SerializeField] private ScreenUIController screenUIControllerMobile;
        private ScreenUIController screenUIController;

        private GameObject currentClientWorker;
        private ConnectionController connectionController;
        private AndroidWorkerConnector androidWorkerConnector;

        public static List<TileEnabler> LevelTiles => Instance.androidWorkerConnector.LevelTiles;

        public static WorkerConnector ClientWorkerConnector => Instance.androidWorkerConnector;
        public static ConnectionController ConnectionController => Instance.connectionController;
        public static ScreenUIController ScreenUIController => Instance.screenUIController;
        public bool ForceMobileMode;

        public static void CreateClient()
        {
            Instance.CreateClientWorker();
        }

        private void Awake()
        {
            Instance = this;
            var mobileMode = false;

#if (UNITY_IOS || UNITY_ANDROID)
            mobileMode = true;
#endif

            if (mobileMode || ForceMobileMode)
            {
                if (ForceMobileMode)
                {
                    Debug.LogWarning("Mobile mode is being forced on ClientWorkerHandler");
                }

                screenUIControllerMobile.gameObject.SetActive(true);
                screenUIControllerDesktop?.gameObject.SetActive(false);
                screenUIController = screenUIControllerMobile;
            }
            else
            {
                screenUIController = screenUIControllerDesktop;
                screenUIControllerMobile?.gameObject.SetActive(false);
                screenUIControllerDesktop.gameObject.SetActive(true);
            }
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
            androidWorkerConnector = currentClientWorker.GetComponent<AndroidWorkerConnector>();
            connectionController = currentClientWorker.GetComponent<ConnectionController>();
            connectionController.InformOfUI(canvasCameraObj, screenUIController);
        }

        private void DisconnectCheck()
        {
            if (androidWorkerConnector != null
                && androidWorkerConnector.Worker != null
                && !androidWorkerConnector.Worker.Connection.IsConnected)
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
