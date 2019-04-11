using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fps
{
    public class Table : MonoBehaviour
    {
        public RectTransform EntriesParentRect;
        public TableEntry EntryTemplate;

        private float entryHeightCached = -1f;

        [NonSerialized] private readonly List<TableEntry> Entries = new List<TableEntry>();

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

        public TableEntry AddEntry()
        {
            TableEntry newEntry = Instantiate(EntryTemplate.gameObject, EntriesParentRect, false).GetComponent<TableEntry>();
            newEntry.transform.localPosition = Vector3.down * EntryHeight * Entries.Count;
            Entries.Add(newEntry);
            return newEntry;
        }

        public TableEntry GetEntry(int index)
        {
            return Entries[index];
        }

        public void ClearEntries()
        {
            // Don't destroy the first child, use it to make new children
            for (var i = EntriesParentRect.childCount - 1; i >= 0; i--)
            {
                Destroy(EntriesParentRect.GetChild(i).gameObject);
            }

            Entries.Clear();
        }
    }
}
