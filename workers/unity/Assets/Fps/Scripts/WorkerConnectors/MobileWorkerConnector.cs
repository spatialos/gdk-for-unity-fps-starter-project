using System;
using Fps.Config;
using Fps.WorkerConnectors;
using Improbable.Gdk.Core;
using Improbable.Gdk.Mobile;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps.WorkerConnectors
{
    public class MobileWorkerConnector : ClientWorkerConnector, MobileConnectionFlowInitializer.IMobileSettingsProvider
    {
#pragma warning disable 649
        [SerializeField] private string IpAddress;
#pragma warning restore 649

        protected override IConnectionHandlerBuilder GetConnectionHandlerBuilder()
        {
            var connParams = CreateConnectionParameters(WorkerUtils.MobileClient);
            connParams.Network.UseExternalIp = true;
            connParams.Network.ConnectionType = NetworkConnectionType.ModularKcp;

            var initializer = new MobileConnectionFlowInitializer(
                new MobileConnectionFlowInitializer.CommandLineSettingsProvider(),
                new MobileConnectionFlowInitializer.PlayerPrefsSettingsProvider(),
                this);

            var builder = new SpatialOSConnectionHandlerBuilder()
                .SetConnectionParameters(connParams);

            switch (initializer.GetConnectionService())
            {
                case ConnectionService.Receptionist:
                    builder.SetConnectionFlow(new ReceptionistFlow(CreateNewWorkerId(WorkerUtils.MobileClient),
                        initializer));
                    break;
                case ConnectionService.Locator:
                    builder.SetConnectionFlow(new LocatorFlow(initializer));
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
