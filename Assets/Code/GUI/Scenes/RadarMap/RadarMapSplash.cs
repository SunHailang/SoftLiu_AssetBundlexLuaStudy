using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarMapSplash : MonoBehaviour
{
    [SerializeField]
    private Material m_meshMaterial = null;
    [SerializeField]
    private CanvasRenderer m_canvasRender = null;

    [Range(0, 20)]
    public int m_attackAmount = 10;

    private Stats m_stat = null;

    private void Start()
    {
        // test
        //UpdateStatsVisualTest();

        m_stat = new Stats(147f, m_canvasRender, m_meshMaterial, null);
        SetStat(m_stat);
    }

    public void UpdateStatsVisualTest()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[6];
        Vector2[] uv = new Vector2[6];
        int[] triangles = new int[3 * 5];
        int[] index = new int[6];
        for (int i = 0; i < 6; i++)
        {
            index[i] = i;
        }
        int cur = 0;
        for (int i = 0; i < triangles.Length; i++)
        {
            if (i % 3 == 0)
            {
                triangles[i] = index[0];
                cur = index[cur] + 1;
            }
            else if (i % 3 == 1)
            {
                triangles[i] = cur;
            }
            else
            {
                if (i == triangles.Length - 1)
                {
                    triangles[i] = index[1];
                }
                else
                {
                    triangles[i] = cur + 1;
                }
            }
        }
        for (int i = 0; i < triangles.Length; i++)
        {
            Debug.Log(triangles[i]);
        }
    }

    public void SetStat(Stats stat)
    {
        this.m_stat = stat;

        this.m_stat.AddSingleStat("Attack", 20);
        this.m_stat.AddSingleStat("Defence", 20);
        this.m_stat.AddSingleStat("Speed", 20);
        this.m_stat.AddSingleStat("Mana", 20);
        this.m_stat.AddSingleStat("Health", 20);
        this.m_stat.AddSingleStat("Top", 20);
    }

    private void Update()
    {
        //m_stat.SetAmount("Attack", m_attackAmount);
    }
}
