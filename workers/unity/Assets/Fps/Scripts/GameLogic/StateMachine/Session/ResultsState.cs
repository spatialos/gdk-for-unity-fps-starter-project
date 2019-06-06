using System.Linq;
using Improbable.Gdk.Core;
using UnityEngine;

namespace Fps
{
    public class ResultsState : SessionState
    {
        private readonly TrackPlayerSystem trackPlayerSystem;

        public ResultsState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            trackPlayerSystem = Blackboard.ClientConnector.Worker.World.GetExistingSystem<TrackPlayerSystem>();
        }

        public override void StartState()
        {
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

            if (Blackboard.ClientConnector == null)
            {
                return;
            }

            if (trackPlayerSystem.PlayerResults.Count == 0)
            {
                return;
            }

            var results = trackPlayerSystem.PlayerResults;
            var playerRank = results.FirstOrDefault(res => res.PlayerName == Blackboard.PlayerName).Rank;
            Debug.Log($"playerRank {playerRank}");
            Debug.Log($"player name {Blackboard.PlayerName}");

            ScreenManager.ResultsScreenDoneButton.onClick.AddListener(Restart);
            ScreenManager.InformOfResults(results, playerRank);
            Manager.ShowFrontEnd();
            ScreenManager.SwitchToResultsScreen();

            UnityObjectDestroyer.Destroy(Blackboard.ClientConnector);
            Blackboard.ClientConnector = null;
        }
    }
}
