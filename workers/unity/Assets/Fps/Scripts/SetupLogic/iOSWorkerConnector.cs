using System.Text.RegularExpressions;
using Improbable.Gdk.Core;
using UnityEngine;

#if UNITY_IOS
using Improbable.Gdk.Mobile.iOS;
#endif

namespace Fps
{
    public class iOSWorkerConnector : MobileWorkerConnectorBase, ITileProvider
    {
        [SerializeField] private string forcedIpAddress;

        protected override async void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
            IpAddress = forcedIpAddress;

            await AttemptConnect();
        }

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
                return IpAddress;
            }

            return RuntimeConfigDefaults.ReceptionistHost;
#else
            throw new System.PlatformNotSupportedException(
                $"{nameof(iOSWorkerConnector)} can only be used for the iOS platform. Please check your build settings.");
#endif
        }
    }
}
