using Improbable.Gdk.Core;

namespace Fps
{
    public class ResultsState : SessionState
    {
        public ResultsState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
        }

        public override void StartState()
        {
            Controller.FrontEndController.ResultsScreenController.DoneButton.onClick.AddListener(Restart);

            var trackPlayerSystem = Owner.ClientConnector.Worker.World.GetExistingManager<TrackPlayerSystem>();
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
            Owner.DestroyClientWorker();
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
            Owner.SetState(Owner.StartState);
        }
    }
}
