using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fps
{
    public class Table : MonoBehaviour
    {
        public RectTransform EntriesParentRect;

        private TableEntry EntryTemplate;

        public float EntryHeight
        {
            get
            {
                if (entryHeightCached >= 0)
                {
                    return entryHeightCached;
                }

                entryHeightCached = EntryTemplate.GetComponent<RectTransform>().rect.height;
                return entryHeightCached;
            }
        }

        private float entryHeightCached = -1f;

        [NonSerialized] private readonly List<TableEntry> Entries = new List<TableEntry>();

        private void Awake()
        {
            if (EntriesParentRect == null)
            {
                Debug.LogError($"EntriesParentRect is not set on table {gameObject.name}");
                return;
            }

            GatherEntryTemplate();

            if (EntryTemplate == null)
            {
                Debug.LogError($"Was unable to find an entry template for table {gameObject.name}");
                return;
            }

            ClearEntries();
        }

        public void ClearEntries()
        {
            // Don't destroy the first child, use it to make new children
            EntriesParentRect.GetChild(0).gameObject.SetActive(false);

            for (var i = EntriesParentRect.childCount - 1; i > 0; i--)
            {
                Destroy(EntriesParentRect.GetChild(i).gameObject);
            }

            Entries.Clear();
        }

        private void GatherEntryTemplate()
        {
            if (EntriesParentRect.childCount == 0)
            {
                Debug.LogWarning($"The EntriesParentRect on '{gameObject}' must have exactly 1 child, which defines " +
                    $"the template used to create entries.");
                return;
            }

            EntryTemplate = EntriesParentRect.GetChild(0).GetComponent<TableEntry>();

            if (EntryTemplate == null)
            {
                Debug.LogWarning($"Expected a TableEntry component on template object {EntriesParentRect.GetChild(0)}");
                return;
            }

            Entries.Add(EntryTemplate);
        }

        public TableEntry AddEntry()
        {
            TableEntry newEntry;

            if (Entries.Count == 0)
            {
                newEntry = EntryTemplate;
                newEntry.gameObject.SetActive(true);
            }
            else
            {
                newEntry = Instantiate(EntryTemplate.gameObject, EntriesParentRect, false).GetComponent<TableEntry>();
            }

            newEntry.transform.localPosition = Vector3.down * EntryHeight * Entries.Count;

            Entries.Add(newEntry);

            return newEntry;
        }

        public TableEntry GetEntry(int index)
        {
            return Entries[index];
        }
    }
}
