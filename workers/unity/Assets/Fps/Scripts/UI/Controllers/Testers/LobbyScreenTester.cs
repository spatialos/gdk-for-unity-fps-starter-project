using UnityEngine;

namespace Fps
{
    [RequireComponent(typeof(LobbyScreenController))]
    public class LobbyScreenTester : MonoBehaviour
    {
        public bool Test;
        public int NumResultsToGenerate;

        private void Update()
        {
            if (!Test)
            {
                return;
            }

            Test = false;
            RunTest();
        }

        private void OnEnable()
        {
            Test = false;
            Invoke(nameof(RunTest), 1);
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
            var maxPlayers = (Random.Range(0, 4) + 1) * 64;
            var currentPlayers = availability ? Random.Range(0, maxPlayers) : maxPlayers;
            return new LobbyScreenController.DeploymentData(serverName, currentPlayers, maxPlayers, availability);
        }
    }
}
