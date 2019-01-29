using System;
using System.Collections.Generic;
using UnityEngine;

public class UITable : MonoBehaviour
{
    public string fill = "filler 1";
    public ColumnList columnList;
    public int someInt = 666;

    [Serializable]
    public class Column
    {
        public string Title;
        [Range(0f, 1f)] public float Percentage;
        public float DividerWidth;

        public Column(string title, float percentage, float dividerWidth)
        {
            Title = title;
            Percentage = percentage;
            DividerWidth = dividerWidth;
        }
    }

    [Serializable]
    public class ColumnList
    {
        public List<Column> Columns;

        public ColumnList()
        {
            Columns = new List<Column>();
            Columns.Add(new Column("Col1", .2f, 4));
            Columns.Add(new Column("Col2", .4f, 4));
            Columns.Add(new Column("Col3", .7f, 4));
        }
    }

    public class Row
    {
        public string[] Cells;
    }
}
