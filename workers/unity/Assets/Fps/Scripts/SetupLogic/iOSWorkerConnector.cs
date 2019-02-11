using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Mobile;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

#if UNITY_IOS
using Improbable.Gdk.Mobile.iOS;
#endif

namespace Fps
{
    [RequireComponent(typeof(ConnectionController))]
    public class iOSWorkerConnector : MobileWorkerConnectorBase, ITileProvider
    {
        public string forcedIpAddress;

        private void OnValidate()
        {
            forcedIpAddress = Regex.Replace(forcedIpAddress, "[^0-9.]", "");
        }

        public async void TryConnect()
        {
            await Connect(WorkerUtils.iOSClient, new ForwardingDispatcher()).ConfigureAwait(false);
        }

        private void Awake()
        {
            connectionController = GetComponent<ConnectionController>();
        }

        protected override string GetWorkerType()
        {
            return WorkerUtils.iOSClient;
        }

        protected virtual async void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
            IpAddress = forcedIpAddress;

            await AttemptConnect();
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

        public async void Reconnect()
        {
            await AttemptConnect();
        }

        protected async Task AttemptConnect()
        {
            await Connect(WorkerUtils.iOSClient, new ForwardingDispatcher()).ConfigureAwait(false);
        }

        protected override ConnectionService GetConnectionService()
        {
            // TODO UTY-1590: add cloud deployment option
            return ConnectionService.Receptionist;
        }
    }
}
