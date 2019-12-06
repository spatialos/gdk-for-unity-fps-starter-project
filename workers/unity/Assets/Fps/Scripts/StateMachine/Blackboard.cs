using System.Collections.Generic;
using Fps.WorkerConnectors;
using Improbable.Worker.CInterop.Alpha;

namespace Fps.StateMachine
{
    [System.Serializable]
    public class Blackboard
    {
        public bool UseSessionBasedFlow;
        public string PlayerName;
        public string DevAuthToken;
        public string PlayerIdentityToken;
        public string Deployment;
        public List<LoginTokenDetails> LoginTokens;
        public ClientWorkerConnector ClientConnector;
    }
}
