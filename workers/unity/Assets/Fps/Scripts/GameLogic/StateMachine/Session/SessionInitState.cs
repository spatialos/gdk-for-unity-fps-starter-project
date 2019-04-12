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
        private readonly SessionScreenController sessionScreenController;

        public SessionInitState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
            sessionScreenController = controller.FrontEndController.SessionScreenController;
        }

        public override void StartState()
        {
            sessionScreenController.browseButton.enabled = false;
            sessionScreenController.quickJoinButton.enabled = false;
            Controller.FrontEndController.SwitchToSessionScreen();
            var listDeploymentsState = new PrepareDeploymentsListState(Controller, Owner);
            var getPitState = new GetPlayerIdentityTokenState(listDeploymentsState, Controller, Owner);

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
