using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftLiu_Grid
{

    private int m_width = 0;
    private int m_height = 0;
    private float m_cellSize = 0;
    private int[,] gridArray;

    public SoftLiu_Grid(int width, int height, float cellSize)
    {
        this.m_width = width;
        this.m_height = height;
        this.m_cellSize = cellSize;
        gridArray = new int[width, height];

        Debug.Log(string.Format("width:{0} , hight:{1}", width, height));
    }

    public void SetSpriteRander(SpriteRenderer render, Transform parent, Texture2D texture)
    {
        SpriteRenderer[,] renderGrid = new SpriteRenderer[m_width, m_height];

        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                gridArray[i, j] = i + j;
                SpriteRenderer r = GameObject.Instantiate(render, parent);
                r.transform.position = GetWorldPosition(i, j);
                r.transform.localScale = Vector3.one * m_cellSize;
                Material m = r.material;
                Color c = texture.GetPixel((i + j), 2);
                m.SetColor("_Color", c);
                renderGrid[i, j] = r;
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * m_cellSize;
    }
}
