using UnityEngine;

namespace Fps
{
    public class GameLogicWorkerHandler : MonoBehaviour
    {
        private static GameLogicWorkerHandler Instance;

        [SerializeField] private GameObject gameLogicWorkerPrefab;
        //[SerializeField] private GameObject canvasCameraObj;
        //[SerializeField] private ScreenUIController screenUIController;

        private GameObject currentGameLogicWorker;
        private ConnectionController connectionController;
        private GameLogicWorkerConnector workerConnector;

        public static GameLogicWorkerConnector GameLogicWorkerConnector => Instance.workerConnector;
        public static ConnectionController ConnectionController => Instance.connectionController;

        public static void CreateGameLogic()
        {
            Instance.CreateGameLogicWorker();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            // Check if the Client worker has been disconnected, and remove it if so.
            DisconnectCheck();
        }

        public void CreateGameLogicWorker()
        {
            if (currentGameLogicWorker != null)
            {
                Destroy(currentGameLogicWorker);
            }

            currentGameLogicWorker = Instantiate(gameLogicWorkerPrefab);
            workerConnector = currentGameLogicWorker.GetComponent<GameLogicWorkerConnector>();
            //connectionController = currentGameLogicWorker.GetComponent<ConnectionController>();
            //connectionController.InformOfUI(canvasCameraObj, screenUIController);
        }

        private void DisconnectCheck()
        {
            if (workerConnector != null
                && workerConnector.Worker != null
                && !workerConnector.Worker.Connection.IsConnected)
            {
                //screenUIController.OnDisconnect("GameLogic");
                Destroy(currentGameLogicWorker);
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
