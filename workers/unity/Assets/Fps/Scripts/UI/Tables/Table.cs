using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fps
{
    public class Table : MonoBehaviour
    {
        public RectTransform EntriesParentRect;
        public TableEntry EntryTemplate;

        public float EntryHeight
        {
            get
            {
                if (entryHeight <= 0f)
                {
                    entryHeight = EntryTemplate.GetComponent<RectTransform>().rect.height;
                }

                return entryHeight;
            }
        }

        private readonly List<TableEntry> entries = new List<TableEntry>();
        private float entryHeight;


        public TableEntry AddEntry()
        {
            TableEntry newEntry = Instantiate(EntryTemplate.gameObject, EntriesParentRect, false).GetComponent<TableEntry>();
            newEntry.transform.localPosition = Vector3.down * EntryHeight * entries.Count;
            entries.Add(newEntry);
            return newEntry;
        }

        public TableEntry GetEntry(int index)
        {
            return entries[index];
        }

        public void ClearEntries()
        {
            for (var i = EntriesParentRect.childCount - 1; i >= 0; i--)
            {
                Destroy(EntriesParentRect.GetChild(i).gameObject);
            }

            entries.Clear();
        }
    }
}
