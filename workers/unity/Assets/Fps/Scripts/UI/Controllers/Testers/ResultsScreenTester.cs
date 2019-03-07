using UnityEngine;

namespace Fps
{
    //TODO Remove class + usages on prefabs before release
    [RequireComponent(typeof(ResultsScreenController))]
    public class ResultsScreenTester : MonoBehaviour
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
            var controller = GetComponent<ResultsScreenController>();

            var results = new ResultsData[NumResultsToGenerate];
            for (var i = 0; i < NumResultsToGenerate; i++)
            {
                results[i] = MakeResult(i);
            }

            controller.SetResults(results, Random.Range(0, NumResultsToGenerate));
        }


        private ResultsData MakeResult(int rank)
        {
            var playerName = RandomNameGenerator.GetName();
            var kills = Random.Range(0, 40);
            var deaths = Random.Range(0, 40);
            return new ResultsData(rank, playerName, kills, deaths);
        }
    }
}
