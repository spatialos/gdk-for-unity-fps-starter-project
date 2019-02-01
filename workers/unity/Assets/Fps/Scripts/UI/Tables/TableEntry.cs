using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TableEntry : MonoBehaviour
{
    [SerializeField] private RectTransform[] Columns;
    [SerializeField] private Selectable Selectable;

    public int ColumnCount => Columns.Length;


    public RectTransform GetColumnRect(int column)
    {
        return Columns[column];
    }

    public Selectable GetSelectable()
    {
        return Selectable;
    }

    public void SetColor(Color col)
    {
        GetComponent<Image>().color = col;
    }

}
