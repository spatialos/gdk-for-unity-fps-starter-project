using System;
using System.Collections.Generic;
using System.Linq;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine;

namespace Fps
{
    public class ConnectionStateMachine : MonoBehaviour
    {
        // Blackboard
        public bool UseSessionBasedFlow;
        public string PlayerName = "Player";
        public string DevAuthToken;
        public string PlayerIdentityToken;
        public State StartState;
        public List<LoginTokenDetails> LoginTokens = new List<LoginTokenDetails>();
        public ClientWorkerConnector ClientConnector;

        private State currentState;
        private State nextState;
        private float transitionTime;

        [SerializeField] private GameObject clientWorkerConnectorPrefab;
        [SerializeField] private UIManager uiManager;

        public void SetState(State state)
        {
            SetState(state, 0f);
        }

        public void SetState(State state, float waitTimeInSeconds)
        {
            nextState = state;
            this.transitionTime = waitTimeInSeconds;
        }


        private void Start()
        {
            if (UseSessionBasedFlow)
            {
                StartState = new SessionInitState(uiManager, this);
            }
            else
            {
                StartState = new DefaultInitState(uiManager, this);
            }

            currentState = StartState;
            currentState.StartState();
        }

        private void Update()
        {
            if (nextState != null)
            {
                if (transitionTime > 0f)
                {
                    transitionTime -= Time.deltaTime;
                    return;
                }

                currentState.ExitState();
                currentState = nextState;
                nextState = null;
                currentState.StartState();
            }

            currentState?.Tick();
        }

        public void CreateClientWorker()
        {
            var clientWorker = Instantiate(clientWorkerConnectorPrefab, transform.position, Quaternion.identity);
            ClientConnector = clientWorker.GetComponent<ClientWorkerConnector>();
        }

        public void DestroyClientWorker()
        {
            UnityObjectDestroyer.Destroy(ClientConnector);
            ClientConnector = null;
        }
    }
}
