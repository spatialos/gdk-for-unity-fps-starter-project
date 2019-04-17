using System.Collections.Generic;
using UnityEngine;

namespace Fps
{
    public class Table : MonoBehaviour
    {
        [SerializeField] private RectTransform entriesParentRect;
        [SerializeField] private TableEntry entryTemplate;

        private RectTransform entryTemplateRect;

        public float EntryHeight;

        private readonly List<TableEntry> entries = new List<TableEntry>();
        private float entryHeight;

        private void OnValidate()
        {
            if (entriesParentRect == null)
            {
                throw new MissingReferenceException("Missing reference to parent object for table entries.");
            }

            if (entryTemplate == null)
            {
                throw new MissingReferenceException("Missing reference to table entry prefab.");
            }

            entryTemplateRect = entryTemplate.GetComponent<RectTransform>();
            EntryHeight = entryTemplateRect.rect.height;
        }

        public TableEntry AddEntry()
        {
            TableEntry newEntry = Instantiate(entryTemplate.gameObject, entriesParentRect, false).GetComponent<TableEntry>();
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
            for (var i = entriesParentRect.childCount - 1; i >= 0; i--)
            {
                Destroy(entriesParentRect.GetChild(i).gameObject);
            }

            entries.Clear();
        }
    }
}
