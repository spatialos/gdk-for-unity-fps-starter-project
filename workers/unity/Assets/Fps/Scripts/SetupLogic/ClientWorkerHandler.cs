#define FORCE_MOBILE

using UnityEngine;
using UnityEngine.Serialization;

namespace Fps
{
    public class ClientWorkerHandler : MonoBehaviour
    {
        private static ClientWorkerHandler Instance;

        [SerializeField] private GameObject clientWorkerPrefab;
        [SerializeField] private GameObject canvasCameraObj;
        [FormerlySerializedAs("screenUIController")] [SerializeField] private ScreenUIController screenUIControllerDesktop;
        [SerializeField] private ScreenUIController screenUIControllerMobile;
        private ScreenUIController screenUIController;

        private GameObject currentClientWorker;
        private ConnectionController connectionController;
        private ClientWorkerConnector workerConnector;

        public static ClientWorkerConnector ClientWorkerConnector => Instance.workerConnector;
        public static ConnectionController ConnectionController => Instance.connectionController;
        public static ScreenUIController ScreenUIController => Instance.screenUIController;

        public static void CreateClient()
        {
            Instance.CreateClientWorker();
        }

        private void Awake()
        {
            Instance = this;

#if FORCE_MOBILE
            Debug.LogWarning("Mobile mode is being forced on ClientWorkerHandler");
#endif

#if (UNITY_IOS || UNITY_ANDROID || FORCE_MOBILE)
            screenUIControllerMobile.gameObject.SetActive(true);
            screenUIControllerDesktop.gameObject.SetActive(false);
            screenUIController = screenUIControllerMobile;
#else
            screenUIController = screenUIControllerDesktop;
            screenUIControllerMobile.gameObject.SetActive(false);
            screenUIControllerDesktop.gameObject.SetActive(true);
#endif
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
