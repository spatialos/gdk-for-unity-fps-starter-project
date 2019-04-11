using System;
using System.Collections;
using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
using UnityEngine;
using LocatorParameters = Improbable.Worker.CInterop.LocatorParameters;

namespace Fps
{
    public abstract class WorkerConnectorBase : DefaultWorkerConnector
    {
        public int TargetFrameRate = 60;

        [SerializeField] protected MapBuilderSettings MapBuilderSettings;

        [NonSerialized] internal GameObject LevelInstance;

        protected abstract string GetWorkerType();

        protected virtual void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
        }

        protected async Task AttemptConnect()
        {
            await Connect(GetWorkerType(), new ForwardingDispatcher()).ConfigureAwait(false);
        }

        protected override string SelectDeploymentName(DeploymentList deployments)
        {
            // This could be replaced with a splash screen asking to select a deployment or some other user-defined logic.
            return deployments.Deployments[0].DeploymentName;
        }

        protected override AlphaLocatorConfig GetAlphaLocatorConfig(string workerType)
        {
            var pit = GetDevelopmentPlayerIdentityToken(DevelopmentAuthToken, GetPlayerId(), GetDisplayName());
            var loginTokenDetails = GetDevelopmentLoginTokens(workerType, pit);
            var loginToken = SelectLoginToken(loginTokenDetails);

            return new AlphaLocatorConfig
            {
                LocatorHost = RuntimeConfigDefaults.LocatorHost,
                LocatorParameters = new Improbable.Worker.CInterop.Alpha.LocatorParameters
                {
                    PlayerIdentity = new PlayerIdentityCredentials
                    {
                        PlayerIdentityToken = pit,
                        LoginToken = loginToken,
                    },
                    UseInsecureConnection = false,
                }
            };
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            StartCoroutine(LoadWorld());
        }

        public override void Dispose()
        {
            if (LevelInstance != null)
            {
                Destroy(LevelInstance);
                LevelInstance = null;
            }

            base.Dispose();
        }

        // Get the world size from the config, and use it to generate the correct-sized level
        protected virtual IEnumerator LoadWorld()
        {
            yield return MapBuilder.GenerateMap(
                MapBuilderSettings,
                transform,
                Worker.Connection,
                Worker.WorkerType,
                Worker.LogDispatcher,
                this);
        }

        protected void LoadDevAuthToken()
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
}
