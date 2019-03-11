using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class DefaultConnectionAnimController : MonoBehaviour
    {
        public Text informationText;
        private Animator animator;

        public void Awake()
        {
            ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChange;
            animator = GetComponentInChildren<Animator>();
        }

        private void OnConnectionStateChange(ConnectionStateReporter.State state, string information)
        {
            informationText.text = information;

            switch (state)
            {
                case ConnectionStateReporter.State.Connected:
                    animator.SetTrigger("Ready");
                    break;
                case ConnectionStateReporter.State.ConnectionFailed:
                    animator.SetTrigger("FailedToConnect");
                    break;
                case ConnectionStateReporter.State.Spawning:
                    animator.SetTrigger("Connecting");
                    break;
                case ConnectionStateReporter.State.SpawningFailed:
                    animator.SetTrigger("FailedToSpawn");
                    break;
                case ConnectionStateReporter.State.WorkerDisconnected:
                    animator.SetTrigger("Disconnected");
                    break;
                case ConnectionStateReporter.State.None:
                case ConnectionStateReporter.State.GettingDeploymentList:
                case ConnectionStateReporter.State.DeploymentListAvailable:
                case ConnectionStateReporter.State.FailedToGetDeploymentList:
                case ConnectionStateReporter.State.Connecting:
                case ConnectionStateReporter.State.WaitingForGameStart:
                case ConnectionStateReporter.State.GameReady:
                case ConnectionStateReporter.State.Spawned:
                case ConnectionStateReporter.State.GatherResults:
                case ConnectionStateReporter.State.ShowResults:
                case ConnectionStateReporter.State.EndSession:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
