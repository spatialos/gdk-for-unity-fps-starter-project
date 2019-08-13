using Improbable.Gdk.Core;
using UnityEngine;

namespace Fps.Connection
{
    public class SessionConnectionFlowInitializer : IConnectionFlowInitializer<AlphaLocatorFlow>
    {
        private readonly IConnectionFlowInitializer<AlphaLocatorFlow> initializer;

        public SessionConnectionFlowInitializer(IConnectionFlowInitializer<AlphaLocatorFlow> standaloneInitializer)
        {
            initializer = standaloneInitializer;
        }

        public void Initialize(AlphaLocatorFlow flow)
        {
            if (Application.isEditor)
            {
                if (PlayerPrefs.HasKey(RuntimeConfigNames.DevAuthTokenKey))
                {
                    flow.DevAuthToken = PlayerPrefs.GetString(RuntimeConfigNames.DevAuthTokenKey);
                    return;
                }

                var textAsset = Resources.Load<TextAsset>("DevAuthToken");

                if (textAsset == null)
                {
                    throw new MissingReferenceException("Unable to find DevAuthToken.txt in the Resources folder. " +
                        "You can generate one via SpatialOS > Generate Dev Authentication Token.");
                }

                flow.DevAuthToken = textAsset.text;
            }
            else
            {
                initializer.Initialize(flow);
            }
        }
    }
}
