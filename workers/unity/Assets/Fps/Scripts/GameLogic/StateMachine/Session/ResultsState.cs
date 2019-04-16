using Improbable.Gdk.Core;

namespace Fps
{
    public class ResultsState : SessionState
    {
        public ResultsState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            Manager.FrontEndController.resultsScreenManager.DoneButton.onClick.AddListener(Restart);

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

            Manager.FrontEndController.resultsScreenManager.SetResults(results.ToArray(), playerRank);

            // show results screen
            Manager.ShowFrontEnd();
            Manager.FrontEndController.SwitchToResultsScreen();
            Owner.DestroyClientWorker();
        }

        public override void ExitState()
        {
            Manager.FrontEndController.resultsScreenManager.DoneButton.onClick.RemoveListener(Restart);
            Manager.FrontEndController.SwitchToSessionScreen();
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
