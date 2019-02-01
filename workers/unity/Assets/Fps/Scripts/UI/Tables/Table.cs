using System;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public RectTransform EntriesParentRect;

    private GameObject EntryTemplate;

    public Color entryColorA;
    public Color entryColorB;

    [NonSerialized]
    public List<TableEntry> Entries = new List<TableEntry>();

    public int EntryCount => Entries.Count;

    private void OnValidate()
    {
        if (!gameObject.scene.isLoaded)
        {
            return;
        }

        if (Application.isPlaying)
        {
            return;
        }

        GatherEntryTemplate();
    }

    private void GatherEntryTemplate()
    {
        if (EntriesParentRect == null)
        {
            return;
        }

        if (EntriesParentRect.childCount == 0)
        {
            Debug.LogWarning($"The EntriesParentRect on '{gameObject}' must have exactly 1 child, which defines " +
                $"the template used to create entries.");
            return;
        }

        EntryTemplate = EntriesParentRect.GetChild(0).gameObject;

        if (EntryTemplate.GetComponent<TableFieldHelper>() == null)
        {
            Debug.LogWarning($"Entry template in object '{EntriesParentRect}' of '{gameObject}' must have a " +
                $"TableFieldHelper component on it. If this is an entry, please add one.");
            EntryTemplate = null;
        }
    }

    public TableFieldHelper AddEntry()
    {
        var newEntry = Instantiate(EntryTemplate, EntriesParentRect, false).GetComponent<TableFieldHelper>();
        Entries.Add(newEntry);
        return newEntry.GetComponent<TableFieldHelper>();
    }


}
