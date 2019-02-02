using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ResultsScreenController))]
public class ResultsScreenTester : MonoBehaviour
{
    public bool Test;
    public int NumResultsToGenerate;

    private void OnValidate()
    {
        if (Test)
        {
            Test = false;
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Please use this in play mode");
                return;
            }

            Invoke(nameof(RunTest), 0);
        }
    }

    private void RunTest()
    {
        var controller = GetComponent<ResultsScreenController>();

        var results = new ResultsScreenController.ResultsData[NumResultsToGenerate];
        for (int i = 0; i < NumResultsToGenerate; i++)
        {
            results[i] = MakeResult(i);
        }

        controller.SetResults(results);
    }


    private ResultsScreenController.ResultsData MakeResult(int rank)
    {
        var playerName = RandomNameGenerator.GetName();
        var kills = Random.Range(0, 40);
        var deaths = Random.Range(0, 40);
        return new ResultsScreenController.ResultsData(rank, playerName, kills, deaths);
    }
}
