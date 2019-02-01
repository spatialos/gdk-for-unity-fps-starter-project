using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Table))]
public class Table_ResultsTester : MonoBehaviour
{
    public bool AddEntry;
    private Table table;

    private Table Table
    {
        get
        {
            if (table != null)
            {
                return table;
            }

            table = GetComponent<Table>();
            return table;
        }
    }

    private void OnValidate()
    {
        if (AddEntry)
        {
            UnityEditor.EditorApplication.update += AddEntryOnTable;
        }
    }

    private void AddEntryOnTable()
    {
        UnityEditor.EditorApplication.update -= AddEntryOnTable;
        if (!AddEntry)
        {
            return;
        }

        AddEntry = false;


        // 0 rank
        // 1 name
        // 2 kills
        // 3 deaths
        // 4 ratio

        var kills = Random.Range(0, 40);
        var deaths = Random.Range(0, 40);
        var kdRatio = kills / deaths;

        var entry = Table.AddEntry();
        SetTextOnColumn(entry, 0, Table.EntryCount.ToString());
        SetTextOnColumn(entry, 1, RandomNameGenerator.GetName());
        SetTextOnColumn(entry, 2, kills.ToString());
        SetTextOnColumn(entry, 3, deaths.ToString());
        SetTextOnColumn(entry, 4, kdRatio.ToString());
    }

    private void SetTextOnColumn(TableEntry entry, int column, string text)
    {
        entry.GetColumnRect(column).GetComponent<Text>().text = text;
    }
}
