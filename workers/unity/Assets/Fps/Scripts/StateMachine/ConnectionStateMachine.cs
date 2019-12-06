using System;
using Fps.UI;
using UnityEngine;

namespace Fps.StateMachine
{
    public class ConnectionStateMachine : MonoBehaviour
    {
        public Blackboard Blackboard;
        public State StartState;

        public GameObject ClientWorkerConnectorPrefab;
        [SerializeField] private UIManager uiManager;

        private State currentState;
        private State nextState;
        private float waitTimeInSeconds;

        public void SetState(State state, float waitTimeInSeconds = 0f)
        {
            nextState = state;
            this.waitTimeInSeconds = waitTimeInSeconds;
        }

        private void Awake()
        {
            if (ClientWorkerConnectorPrefab == null)
            {
                throw new NullReferenceException("Missing reference to client worker prefab");
            }

            if (uiManager == null)
            {
                throw new NullReferenceException("Missing reference to UI manager");
            }
        }

        private void Start()
        {
            StartState = new DefaultInitState(uiManager, this);

            currentState = StartState;
            currentState.StartState();
        }

        private void Update()
        {
            if (nextState == null)
            {
                currentState?.Tick();
                return;
            }

            if (waitTimeInSeconds > 0f)
            {
                waitTimeInSeconds -= Time.deltaTime;
                return;
            }

            currentState?.ExitState();
            currentState = nextState;
            nextState = null;
            currentState?.StartState();
            currentState?.Tick();
        }
    }
}
