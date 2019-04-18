using System.Collections.Generic;
using Improbable.Worker.CInterop.Alpha;
using Improbable.Gdk.Session;

namespace Fps
{
    [System.Serializable]
    public class Blackboard
    {
        public bool UseSessionBasedFlow;
        public Status SessionStatus;
        public string PlayerName;
        public string DevAuthToken;
        public string PlayerIdentityToken;
        public string Deployment;
        public List<LoginTokenDetails> LoginTokens;
        public ClientWorkerConnector ClientConnector;
    }
}
