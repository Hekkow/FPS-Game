using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGrid : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns,
        Upgrades
    }
    public enum RectOrObj
    {
        Rect,
        Obj
    }
    public FitType fitType;
    public RectOrObj rectOrObj;
    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;

    public bool fitX;
    public bool fitY;
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform) {
            fitX = true;
            fitY = true;
            float sqrt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrt);
            columns = Mathf.CeilToInt(sqrt);
        }
        
        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / columns);
        }
        if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / rows);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = (parentWidth / columns) - (spacing.x / ((float)columns / (columns - 1))) - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = (parentHeight / rows) - (spacing.y / ((float)rows / (rows - 1))) - (padding.top / (float)rows) - (padding.bottom / (float)rows);
        if (fitType == FitType.Upgrades)
        {
            cellHeight = cellWidth;
        }

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        int columnCount = 0;
        int rowCount = 0;
        if (rectOrObj == RectOrObj.Rect)
        {
            for (int i = 0; i < rectChildren.Count; i++)
            {
                rowCount = i / columns;
                columnCount = i % columns;

                RectTransform item = rectChildren[i];
                float xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                float yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                rowCount = i / columns;
                columnCount = i % columns;

                Transform item = transform.GetChild(i);
                float xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                float yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;
                item.position = new Vector3(xPos, yPos, 0);
            }
        }
    }
    public override void CalculateLayoutInputVertical()
    {
        
    }

    public override void SetLayoutHorizontal()
    {
        
    }

    public override void SetLayoutVertical()
    {
        
    }
}
