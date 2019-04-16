#if UNITY_ANDROID
using UnityEngine;
using Improbable.Gdk.Core;
using Improbable.Gdk.Mobile;
using Improbable.Gdk.Mobile.Android;
#endif

namespace Fps
{
    public class AndroidWorkerConnector : MobileWorkerConnector
    {
        protected override string GetWorkerType()
        {
            return WorkerUtils.AndroidClient;
        }

        protected override string GetHostIp()
        {
#if UNITY_ANDROID
            UseIpAddressFromArguments();
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

        private void UseIpAddressFromArguments()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var arguments = LaunchArguments.GetArguments();
            IpAddress =
                CommandLineUtility.GetCommandLineValue(arguments, RuntimeConfigNames.ReceptionistHost, string.Empty);

            if (string.IsNullOrEmpty(IpAddress))
            {
                IpAddress = "127.0.0.1";
            }
#endif
        }
    }
}
