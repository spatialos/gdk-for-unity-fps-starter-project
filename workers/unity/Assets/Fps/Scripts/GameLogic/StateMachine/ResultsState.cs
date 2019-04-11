using Improbable.Gdk.Core;

namespace Fps
{
    public class ResultsState : SessionState
    {
        private ClientWorkerConnector connector;

        public ResultsState(ClientWorkerConnector connector, ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
            this.connector = connector;
        }

        public override void StartState()
        {
            Controller.FrontEndController.ResultsScreenController.DoneButton.onClick.AddListener(Restart);

            var trackPlayerSystem = connector.Worker.World.GetExistingManager<TrackPlayerSystem>();
            var playerName = trackPlayerSystem.PlayerName;
            var results = trackPlayerSystem.PlayerResults;
            // get player rank
            var playerRank = -1;

            for (var i = 0; i < results.Count; i++)
            {
                var result = results[i];
                result.Rank = i + 1;
                results[i] = result;

                if (playerName == result.PlayerName)
                {
                    playerRank = i;
                }
            }

            Controller.FrontEndController.ResultsScreenController.SetResults(results.ToArray(), playerRank);

            // show results screen
            Controller.ShowFrontEnd();
            Controller.FrontEndController.SwitchToResultsScreen();
            UnityObjectDestroyer.Destroy(connector.gameObject);
            connector = null;
        }

        public override void ExitState()
        {
            Controller.FrontEndController.ResultsScreenController.DoneButton.onClick.RemoveListener(Restart);
            Controller.FrontEndController.SwitchToSessionScreen();
        }

        public override void Tick()
        {
        }

        private void Restart()
        {
            Owner.SetState(new InitState(Controller, Owner));
        }
    }
}
