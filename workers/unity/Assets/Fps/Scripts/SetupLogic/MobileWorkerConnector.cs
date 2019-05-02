using System;
using Improbable.Gdk.Core;
using Improbable.Gdk.Mobile;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public abstract class MobileWorkerConnector : ClientWorkerConnector
    {
        private const string HostIpPlayerPrefsKey = "SpatialOSHostIp";

#pragma warning disable 649
        [SerializeField] private string IpAddress;
        [SerializeField] private bool ShouldConnectLocally;
#pragma warning restore 649

        #region Connection Configuration

        private void Awake()
        {
            InitializeClient();
        }

        protected override string GetAuthPlayerPrefabPath()
        {
            return "Prefabs/MobileClient/Authoritative/Player";
        }

        protected override string GetNonAuthPlayerPrefabPath()
        {
            return "Prefabs/MobileClient/NonAuthoritative/Player";
        }

        protected override AlphaLocatorConfig GetAlphaLocatorConfig(string workerType)
        {
            return GetAlphaLocatorConfigViaDevAuthFlow(workerType);
        }

        protected override ConnectionService GetConnectionService()
        {
            if (UseSessionFlow)
            {
                return ConnectionService.AlphaLocator;
            }

            return ShouldConnectLocally ? ConnectionService.Receptionist : ConnectionService.AlphaLocator;
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
            throw new NotImplementedException("The locator flow is not available for mobile workers.");
        }

        #endregion

        private void InitializeClient()
        {
            var arguments = LaunchArguments.GetArguments();
            var environment = CommandLineUtility.GetCommandLineValue(arguments, RuntimeConfigNames.Environment, string.Empty);
            if (string.IsNullOrEmpty(environment))
            {
                environment = PlayerPrefs.GetString(RuntimeConfigNames.Environment, string.Empty);
            }
            else
            {
                PlayerPrefs.SetString(RuntimeConfigNames.Environment, environment);
            }

            if (!string.IsNullOrEmpty(environment))
            {
                ShouldConnectLocally = environment == RuntimeConfigDefaults.LocalEnvironment;
            }

            if (ShouldConnectLocally)
            {
                IpAddress = GetHostIp();
                PlayerPrefs.SetString(HostIpPlayerPrefsKey, IpAddress);
            }
            else
            {
                PlayerPrefs.DeleteKey(HostIpPlayerPrefsKey);
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Extracts the Ip address that should be used to connect via the receptionist. The order is as follows:
        /// 1. Try to extract the ip address from command line arguments passed in. This currently only works for Android.
        /// 2. If we are running on an Android Emulator: Use the Ip address necessary to connect locally.
        /// 3. If we are on a physical device (Android & iOS): Try to extract the value from the stored player preferences.
        /// 4. Check if we stored anything inside the IpAddress field and use it, if we have.
        /// 5. Return the default ReceptionistHost (localhost).
        /// </summary>
        /// <returns></returns>
        private string GetHostIp()
        {
            var arguments = LaunchArguments.GetArguments();
            var hostIp =
                CommandLineUtility.GetCommandLineValue(arguments, RuntimeConfigNames.ReceptionistHost, string.Empty);
            if (!string.IsNullOrEmpty(hostIp))
            {
                return hostIp;
            }

            if (Application.isMobilePlatform)
            {
                switch (DeviceInfo.ActiveDeviceType)
                {
                    case MobileDeviceType.Virtual:
#if UNITY_ANDROID
                        return DeviceInfo.AndroidEmulatorDefaultCallbackIp;
#else
                        break;
#endif
                    case MobileDeviceType.Physical:
                        return PlayerPrefs.GetString(HostIpPlayerPrefsKey, IpAddress);
                }
            }

            if (!string.IsNullOrEmpty(IpAddress))
            {
                return IpAddress;
            }

            return RuntimeConfigDefaults.ReceptionistHost;
        }
    }
}
