using UnityEngine;

namespace Fps
{
    [RequireComponent(typeof(LobbyScreenController))]
    public class LobbyScreenTester : MonoBehaviour
    {
        public bool Test;
        public int NumResultsToGenerate;

        private void OnValidate()
        {
            if (!Test)
            {
                return;
            }

            Test = false;
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Please use this in play mode");
                return;
            }

            Invoke(nameof(RunTest), 0);
        }

        private void RunTest()
        {
            var controller = GetComponent<LobbyScreenController>();

            var results = new LobbyScreenController.DeploymentData[NumResultsToGenerate];
            for (var i = 0; i < NumResultsToGenerate; i++)
            {
                results[i] = MakeDeployment();
            }

            controller.SetDeployments(results);
        }


        private LobbyScreenController.DeploymentData MakeDeployment()
        {
            var serverName = "Awesome super duper deployment #" + Random.Range(0, 30);
            var availability = Random.value > .33f;
            return new LobbyScreenController.DeploymentData(serverName, availability);
        }
    }
}
