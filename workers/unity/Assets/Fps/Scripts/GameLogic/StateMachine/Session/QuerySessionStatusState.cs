using System.Linq;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Session;
using Improbable.Worker.CInterop.Query;

namespace Fps
{
    public class QuerySessionStatusState : SessionState
    {
        private readonly CommandSystem commandSystem;
        private long requestId = -1;
        private State nextState;

        public QuerySessionStatusState(State nextState, UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            commandSystem = Blackboard.ClientConnector.Worker.World.GetExistingManager<CommandSystem>();
            this.nextState = nextState;
        }

        public override void StartState()
        {
            var query = new WorldCommands.EntityQuery.Request
            {
                EntityQuery = new EntityQuery
                {
                    Constraint = new ComponentConstraint(Session.ComponentId),
                    ResultType = new SnapshotResultType()
                }
            };

            requestId = commandSystem.SendCommand(query);
        }

        public override void Tick()
        {
            var status = GetStatus();
            if (status == null)
            {
                return;
            }

            Blackboard.SessionStatus = status.Value;
            Owner.SetState(nextState);
        }


        private Status? GetStatus()
        {
            var messages = commandSystem.GetResponse<WorldCommands.EntityQuery.ReceivedResponse>(requestId);
            if (messages.Count > 0)
            {
                var message = messages[0];
                var sessionSnapshot = message.Result.First().Value.GetComponentSnapshot<Session.Snapshot>();
                return sessionSnapshot.Value.Status;
            }

            return null;
        }
    }
}
