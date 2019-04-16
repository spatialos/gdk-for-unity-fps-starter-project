using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using Improbable.Worker.CInterop.Alpha;
using UnityEngine;

namespace Fps
{
    public class SessionInitState : SessionState
    {
        private readonly StartScreenManager startScreenManager;

        public SessionInitState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            startScreenManager = manager.ScreenManager.StartScreenManager;
        }

        public override void StartState()
        {
            startScreenManager.browseButton.enabled = false;
            startScreenManager.quickJoinButton.enabled = false;
            Manager.ShowFrontEnd();
            Manager.ScreenManager.SwitchToSessionScreen();
            var listDeploymentsState = new PrepareDeploymentsListState(Manager, Owner);
            var getPitState = new GetPlayerIdentityTokenState(listDeploymentsState, Manager, Owner);

            var textAsset = Resources.Load<TextAsset>("DevAuthToken");
            if (textAsset != null)
            {
                Owner.DevAuthToken = textAsset.text.Trim();
            }
            else
            {
                throw new MissingManifestResourceException(
                    "Unable to find DevAuthToken.txt inside the Resources folder. Ensure to generate one.");
            }

            Manager.ScreenManager.StartScreenManager.ShowGetDeploymentListText();
            Owner.SetState(getPitState);
        }

        public override void ExitState()
        {
        }

        public override void Tick()
        {
        }
    }
}
