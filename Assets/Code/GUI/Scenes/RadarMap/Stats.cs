using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stats
{
    public enum StatType
    {
        Attack,
        Defence,
        Speed,
        Mana,
        Health
    }

    Dictionary<string, SingleStat> m_SingleStatDic = null;

    public event EventHandler OnStatsAmountChanged;

    float m_chartSize;
    CanvasRenderer m_canvasRender;
    Material m_meshMaterial;
    Texture m_texture;

    public Stats(float chartSize, CanvasRenderer canvasRender, Material meshMaterial, Texture texture)
    {
        m_SingleStatDic = new Dictionary<string, SingleStat>();

        m_chartSize = chartSize;
        m_canvasRender = canvasRender;
        m_meshMaterial = meshMaterial;
        m_texture = texture;

        this.OnStatsAmountChanged += OnStatsAmount_Changed;
    }

    private void OnStatsAmount_Changed(object sender, EventArgs e)
    {
        UpdateStatsVisual();
    }

    public void AddSingleStat(string name, int amount)
    {
        if (!m_SingleStatDic.ContainsKey(name))
        {
            m_SingleStatDic.Add(name, new SingleStat(amount, OnStatsAmountChanged));
        }
    }

    public void SetAmount(string name, int amount)
    {
        if (!m_SingleStatDic.ContainsKey(name))
        {
            return;
        }
        m_SingleStatDic[name].SetAttackStatAmount(amount);
    }

    private void UpdateStatsVisual()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[m_SingleStatDic.Count + 1];
        Vector2[] uv = new Vector2[m_SingleStatDic.Count + 1];
        int[] triangles = new int[3 * m_SingleStatDic.Count];

        float angleIncrement = 360f / m_SingleStatDic.Count;
        float radarChartSize = m_chartSize;

        int[] index = new int[m_SingleStatDic.Count + 1];
        for (int i = 0; i < index.Length; i++)
        {
            index[i] = i;
        }
        uv[0] = Vector2.zero;
        vertices[0] = Vector3.zero;
        index[0] = 0;
        int indexStat = 0;
        foreach (KeyValuePair<string, SingleStat> item in m_SingleStatDic)
        {
            indexStat++;
            index[indexStat] = indexStat;
            uv[indexStat] = Vector2.one;
            vertices[indexStat] = Quaternion.Euler(0, 0, -angleIncrement * (indexStat - 1)) * Vector3.up * radarChartSize * item.Value.GetAttackStatAmountNormalized();
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
                triangles[i] = cur;
            else
            {
                if (i == triangles.Length - 1)
                    triangles[i] = index[1];
                else
                    triangles[i] = cur + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        m_canvasRender.SetMesh(mesh);
        m_canvasRender.SetMaterial(m_meshMaterial, m_texture);
    }


    private class SingleStat
    {
        private const int STAT_MIN = 0;
        private const int STAT_MAX = 20;

        private int m_stat = 0;

        private int m_statOld = 0;

        private EventHandler OnStatsAmountChanged = null;
        public SingleStat(int amount, EventHandler hander)
        {
            m_stat = amount;
            OnStatsAmountChanged = hander;
            if (OnStatsAmountChanged != null)
            {
                OnStatsAmountChanged(this, EventArgs.Empty);
            }
        }

        public void SetAttackStatAmount(int attackStatAmount)
        {
            m_stat = Mathf.Clamp(attackStatAmount, STAT_MIN, STAT_MAX);
            if (m_statOld != m_stat && OnStatsAmountChanged != null)
            {
                m_statOld = m_stat;
                OnStatsAmountChanged(this, EventArgs.Empty);
            }
        }

        public int GetAttackStatAmount()
        {
            return m_stat;
        }

        public float GetAttackStatAmountNormalized()
        {
            return (float)m_stat / STAT_MAX;
        }
    }
}

