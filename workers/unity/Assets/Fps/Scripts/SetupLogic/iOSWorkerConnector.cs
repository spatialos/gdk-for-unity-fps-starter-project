using System.Text.RegularExpressions;
using UnityEngine;

#if UNITY_IOS
using Improbable.Gdk.Core;
using Improbable.Gdk.Mobile.iOS;
#endif

namespace Fps
{
    public class iOSWorkerConnector : MobileWorkerConnector
    {
        [SerializeField] private string forcedIpAddress;

        private void OnValidate()
        {
            forcedIpAddress = Regex.Replace(forcedIpAddress, "[^0-9.]", "");
        }

        protected override string GetWorkerType()
        {
            return WorkerUtils.iOSClient;
        }

        protected override string GetHostIp()
        {
#if UNITY_IOS
            if (!string.IsNullOrEmpty(IpAddress))
            {
                return forcedIpAddress;
            }

            return RuntimeConfigDefaults.ReceptionistHost;
#else
            throw new System.PlatformNotSupportedException(
                $"{nameof(iOSWorkerConnector)} can only be used for the iOS platform. Please check your build settings.");
#endif
        }
    }
}
