using System;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public abstract class MobileWorkerConnector : ClientWorkerConnector
    {
        protected string IpAddress;

        [SerializeField] private bool shouldConnectLocally;

        #region Connection Configuration

        protected abstract string GetHostIp();

        protected override string GetAuthPlayerPrefabPath()
        {
            return "Prefabs/MobileClient/Authoritative/Player";
        }

        protected override string GetNonAuthPlayerPrefabPath()
        {
            return "Prefabs/MobileClient/NonAuthoritative/Player";
        }

        protected override ConnectionService GetConnectionService()
        {
            if (!shouldConnectLocally)
            {
                LoadDevAuthToken();
                return ConnectionService.AlphaLocator;
            }

            return ConnectionService.Receptionist;
        }

        protected override ConnectionParameters GetConnectionParameters(string workerType, ConnectionService service)
        {
            return new ConnectionParameters
            {
                WorkerType = workerType,
                Network =
                {
                    ConnectionType = NetworkConnectionType.Kcp,
                    UseExternalIp = true,
                    Kcp = new KcpNetworkParameters
                    {
                        Heartbeat = new HeartbeatParameters()
                        {
                            IntervalMillis = 5000,
                            TimeoutMillis = 10000
                        }
                    }
                },
                EnableProtocolLoggingAtStartup = false,
                DefaultComponentVtable = new ComponentVtable(),
            };
        }

        protected override ReceptionistConfig GetReceptionistConfig(string workerType)
        {
            return new ReceptionistConfig
            {
                ReceptionistHost = GetHostIp(),
                ReceptionistPort = RuntimeConfigDefaults.ReceptionistPort,
                WorkerId = CreateNewWorkerId(workerType)
            };
        }

        protected override LocatorConfig GetLocatorConfig()
        {
            throw new NotImplementedException("The locator flow is currently not available for mobile workers.");
        }

        #endregion
    }
}
