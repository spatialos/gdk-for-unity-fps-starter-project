using System.Collections.Generic;
using Improbable.Worker.CInterop.Alpha;
using Improbable.Gdk.Session;

namespace Fps
{
    [System.Serializable]
    public struct Blackboard
    {
        public bool UseSessionBasedFlow;
        public Status SessionStatus;
        public string PlayerName;
        public string DevAuthToken;
        public string PlayerIdentityToken;
        public List<LoginTokenDetails> LoginTokens;
        public ClientWorkerConnector ClientConnector;
    }
}
