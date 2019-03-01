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
                case ConnectionStateReporter.State.None:
                    break;
                case ConnectionStateReporter.State.GettingDeploymentList:
                    break;
                case ConnectionStateReporter.State.DeploymentListAvailable:
                    break;
                case ConnectionStateReporter.State.FailedToGetDeploymentList:
                    break;
                case ConnectionStateReporter.State.Connecting:
                    break;
                case ConnectionStateReporter.State.Connected:
                    animator.SetTrigger("Ready");
                    break;
                case ConnectionStateReporter.State.ConnectionFailed:
                    animator.SetTrigger("FailedToConnect");
                    break;
                case ConnectionStateReporter.State.WaitingForGameStart:
                    break;
                case ConnectionStateReporter.State.GameReady:
                    break;
                case ConnectionStateReporter.State.Spawning:
                    animator.SetTrigger("Connecting");
                    break;
                case ConnectionStateReporter.State.Spawned:
                    break;
                case ConnectionStateReporter.State.SpawningFailed:
                    animator.SetTrigger("FailedToSpawn");
                    break;
                case ConnectionStateReporter.State.WorkerDisconnected:
                    animator.SetTrigger("Disconnected");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
