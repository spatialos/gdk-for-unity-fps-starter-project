using UnityEngine;
#if UNITY_ANDROID
using Improbable.Gdk.Core;
using Improbable.Gdk.Mobile;
using Improbable.Gdk.Mobile.Android;
#endif

namespace Fps
{
    public class AndroidWorkerConnector : MobileWorkerConnectorBase, ITileProvider
    {
        protected override async void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
#if UNITY_ANDROID && !UNITY_EDITOR
            UseIpAddressFromArguments();
#endif
            await AttemptConnect();
        }

        protected override string GetWorkerType()
        {
            return WorkerUtils.AndroidClient;
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
    }
}
