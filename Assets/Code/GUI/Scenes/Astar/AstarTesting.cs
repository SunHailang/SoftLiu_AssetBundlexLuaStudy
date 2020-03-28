using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AstarTesting : MonoBehaviour
{
    public Texture2D m_texture = null;
    [SerializeField]
    private SpriteRenderer m_spriteRander = null;
    [SerializeField]
    private Transform m_parent = null;

    SoftLiu_Grid grid;

    private void Start()
    {
        grid = new SoftLiu_Grid(10, 10, 10f);
    }


    public void CreateGrid_OnClick()
    {
        grid.SetSpriteRander(m_spriteRander, m_parent, m_texture);
    }


}

