using System.Linq;
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
            Manager.ScreenManager.ResultsScreenManager.DoneButton.onClick.AddListener(Restart);

            var trackPlayerSystem = Owner.Blackboard.ClientConnector.Worker.World.GetExistingManager<TrackPlayerSystem>();
            var playerName = trackPlayerSystem.PlayerName;
            var results = trackPlayerSystem.PlayerResults;
            // get player rank
            var playerRank = results.FirstOrDefault(res => res.PlayerName == playerName).Rank;

            Manager.ScreenManager.ResultsScreenManager.SetResults(results.ToArray(), playerRank);

            // show results screen
            Manager.ShowFrontEnd();
            Manager.ScreenManager.SwitchToResultsScreen();
            UnityObjectDestroyer.Destroy(Owner.Blackboard.ClientConnector);
            Owner.Blackboard.ClientConnector = null;
        }

        public override void ExitState()
        {
            Manager.ScreenManager.ResultsScreenManager.DoneButton.onClick.RemoveListener(Restart);
            Manager.ScreenManager.SwitchToSessionScreen();
        }

        private void Restart()
        {
            Owner.SetState(Owner.StartState);
        }
    }
}
