using System.Linq;
using Improbable.Gdk.Core;
using UnityEngine;

namespace Fps
{
    public class ResultsState : SessionState
    {
        public ResultsState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            var trackPlayerSystem = Blackboard.ClientConnector.Worker.World.GetExistingManager<TrackPlayerSystem>();
            var results = trackPlayerSystem.PlayerResults;
            var playerRank = results.FirstOrDefault(res => res.PlayerName == Blackboard.PlayerName).Rank;

            ScreenManager.ResultsScreenDoneButton.onClick.AddListener(Restart);
            ScreenManager.InformOfResults(results, playerRank);
            Manager.ShowFrontEnd();
            ScreenManager.SwitchToResultsScreen();

            UnityObjectDestroyer.Destroy(Blackboard.ClientConnector);
            Blackboard.ClientConnector = null;
        }

        public override void ExitState()
        {
            ScreenManager.ResultsScreenDoneButton.onClick.RemoveListener(Restart);
            ScreenManager.SwitchToStartScreen();
        }

        private void Restart()
        {
            Owner.SetState(Owner.StartState);
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter)
                || Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.Escape))
            {
                ScreenManager.ResultsScreenDoneButton.onClick.Invoke();
            }
        }
    }
}
