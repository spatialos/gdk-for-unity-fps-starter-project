using System.Collections.Generic;
using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Mobile;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;
#if UNITY_ANDROID
using Improbable.Gdk.Mobile.Android;
#endif

namespace Fps
{
    [RequireComponent(typeof(ConnectionController))]
    public class AndroidWorkerConnector : MobileWorkerConnectorBase, ITileProvider
    {
        public bool ShouldConnectLocally;

        private void Awake()
        {
            connectionController = GetComponent<ConnectionController>();

            if (!ShouldConnectLocally)
            {
                var textAsset = Resources.Load<TextAsset>("DevAuthToken");
                if (textAsset != null)
                {
                    DevelopmentAuthToken = textAsset.text.Trim();
                }
                else
                {
                    Debug.LogWarning("Unable to find DevAuthToken.txt in the Resources folder.");
                }
            }
        }

        protected override string GetWorkerType()
        {
            return WorkerUtils.AndroidClient;
        }

        protected virtual async void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
#if UNITY_ANDROID && !UNITY_EDITOR
            UseIpAddressFromArguments();
#endif
            await AttemptConnect();
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        public void UseIpAddressFromArguments()
        {
            IpAddress = GetReceptionistHostFromArguments();

            if (string.IsNullOrEmpty(IpAddress))
            {
                IpAddress = "127.0.0.1";
            }
        }

        private string GetReceptionistHostFromArguments()
        {
            var arguments = LaunchArguments.GetArguments();
            var hostIp =
                CommandLineUtility.GetCommandLineValue(arguments, RuntimeConfigNames.ReceptionistHost, string.Empty);
            return hostIp;
        }
#endif

        protected override string GetHostIp()
        {
#if UNITY_ANDROID
            if (!string.IsNullOrEmpty(IpAddress))
            {
                return IpAddress;
            }

            if (Application.isMobilePlatform && AndroidDeviceInfo.ActiveDeviceType == MobileDeviceType.Virtual)
            {
                return AndroidDeviceInfo.EmulatorDefaultCallbackIp;
            }

            return RuntimeConfigDefaults.ReceptionistHost;
#else
            throw new System.PlatformNotSupportedException(
                $"{nameof(AndroidWorkerConnector)} can only be used for the Android platform. Please check your build settings.");
#endif
        }

        private async Task AttemptConnect()
        {
            await Connect(WorkerUtils.AndroidClient, new ForwardingDispatcher()).ConfigureAwait(false);
        }

        protected override ConnectionService GetConnectionService()
        {
            if (ShouldConnectLocally)
            {
                return ConnectionService.Receptionist;
            }

            return ConnectionService.AlphaLocator;
        }
    }
}
