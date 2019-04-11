using System;
using System.Collections.Generic;
using System.Linq;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
using Unity.Entities;
using UnityEngine;

namespace Fps
{
    public class ConnectionStateMachine : MonoBehaviour
    {
        public bool UseSessionBasedFlow;

        private State state;
        private State nextState;

        [SerializeField] private GameObject clientWorkerConnectorPrefab;
        [SerializeField] private ScreenUIController screenUIController;

        public void SetState(State state)
        {
            nextState = state;
        }

        private void Start()
        {
            state = new InitState(screenUIController, this);
            state.StartState();
        }

        private void Update()
        {
            if (nextState != null)
            {
                state.ExitState();
                state = nextState;
                nextState = null;
                state.StartState();
            }

            state?.Tick();
        }

        public ClientWorkerConnector CreateClientWorker()
        {
            var clientWorker = Instantiate(clientWorkerConnectorPrefab, transform.position, Quaternion.identity);
            return clientWorker.GetComponent<ClientWorkerConnector>();
        }
    }
}
