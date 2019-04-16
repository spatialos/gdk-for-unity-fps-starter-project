using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class TableEntry : MonoBehaviour
    {
        public Image Background;
        public Text[] TextVisuals;

        public void SetAllTextVisuals(Color color, bool bold)
        {
            foreach (var text in TextVisuals)
            {
                text.color = color;
                text.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
            }
        }
    }
}
