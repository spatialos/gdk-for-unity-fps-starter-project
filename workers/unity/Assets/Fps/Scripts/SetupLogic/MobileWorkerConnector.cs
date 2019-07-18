using System;
using Improbable.Gdk.Core;
using Improbable.Gdk.Mobile;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public class MobileWorkerConnector : ClientWorkerConnector, MobileConnectionFlowInitializer.IMobileSettingsProvider
    {
#pragma warning disable 649
        [SerializeField] private string IpAddress;
#pragma warning restore 649

        protected override string GetAuthPlayerPrefabPath()
        {
            return "Prefabs/MobileClient/Authoritative/Player";
        }

        protected override string GetNonAuthPlayerPrefabPath()
        {
            return "Prefabs/MobileClient/NonAuthoritative/Player";
        }

        protected override IConnectionHandlerBuilder GetConnectionHandlerBuilder()
        {
            var connParams = CreateConnectionParameters(WorkerUtils.MobileClient);
            connParams.Network.UseExternalIp = true;
            connParams.Network.ConnectionType = NetworkConnectionType.Kcp;
            connParams.Network.Kcp = new KcpNetworkParameters
            {
                // These are the last tested values
                Heartbeat = new HeartbeatParameters
                {
                    IntervalMillis = 5000,
                    TimeoutMillis = 10000
                }
            };

            var initializer = new MobileConnectionFlowInitializer(
                new MobileConnectionFlowInitializer.CommandLineSettingsProvider(),
                new MobileConnectionFlowInitializer.PlayerPrefsSettingsProvider(),
                this);

            var builder = new SpatialOSConnectionHandlerBuilder()
                .SetConnectionParameters(connParams);

            if (UseSessionFlow)
            {
                builder.SetConnectionFlow(new ChosenDeploymentAlphaLocatorFlow(deployment,
                    new SessionConnectionFlowInitializer(initializer)));
                return builder;
            }

            switch (initializer.GetConnectionService())
            {
                case ConnectionService.Receptionist:
                    builder.SetConnectionFlow(new ReceptionistFlow(CreateNewWorkerId(WorkerUtils.MobileClient),
                        initializer));
                    break;
                case ConnectionService.AlphaLocator:
                    builder.SetConnectionFlow(new AlphaLocatorFlow(initializer));
                    break;
                default:
                    throw new ArgumentException("Received unsupported connection service.");
            }

            return builder;
        }


        public Option<string> GetReceptionistHostIp()
        {
            return IpAddress;
        }

        public Option<string> GetDevAuthToken()
        {
            var token = Resources.Load<TextAsset>("DevAuthToken")?.text.Trim();
            return token ?? Option<string>.Empty;
        }

        public Option<ConnectionService> GetConnectionService()
        {
            return Option<ConnectionService>.Empty;
        }
    }
}
