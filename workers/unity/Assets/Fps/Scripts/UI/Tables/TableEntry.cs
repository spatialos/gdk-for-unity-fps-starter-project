using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class TableEntry : MonoBehaviour
    {
        [SerializeField] private RectTransform[] Columns;

        public RectTransform GetColumnRect(int column)
        {
            return Columns[column];
        }

        public void SetTextOnColumn(int column, string text)
        {
            RectTransform columnRect;
            try
            {
                columnRect = GetColumnRect(column);
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException(
                    $"Column with index {column} does not exist. Does the TableEntry " +
                    $"object '{gameObject.name}' have its Column references filled in?");
            }

            var textComp = columnRect.GetComponentInChildren<Text>();

            if (textComp == null)
            {
                throw new NullReferenceException("Tried to set text on column with no text component.");
            }

            textComp.text = text;
        }

        public void SetAllTextVisuals(Color color, bool bold)
        {
            foreach (var text in GetComponentsInChildren<Text>())
            {
                text.color = color;
                text.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
            }
        }

        public void SetBackgroundColor(Color col)
        {
            GetComponent<Image>().color = col;
        }
    }
}
