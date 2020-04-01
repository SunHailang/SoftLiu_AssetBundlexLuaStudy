using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundForCamera : MonoBehaviour
{
    public bool m_PreserveAspect = false;

    private float m_width = 0;
    private float m_height = 0;

    private SpriteRenderer m_spriteRender;

    private Camera m_camera;

    private float m_disOld = 0;
    private void Start()
    {
        m_width = (float)Screen.width;
        m_height = (float)Screen.height;
        m_camera = Camera.main;
        m_spriteRender = GetComponent<SpriteRenderer>();

        //m_camera.fieldOfView/2

        m_camera.fieldOfView *= 1 + (m_height / m_width - (1080f / 1920));

        SetSprite();
    }

    private void Update()
    {
        SetSprite();
    }
    public void SetSprite()
    {
        
        float dis = Vector3.Distance(m_camera.transform.position, transform.position);

        if (m_disOld == dis && (float)Screen.height == m_height)
        {
            return;
        }
        m_width = (float)Screen.width;
        m_height = (float)Screen.height;
        m_camera.fieldOfView = 60 * (1 + (m_height / m_width - (1080f / 1920)));

        m_disOld = dis;
        float angle = m_camera.fieldOfView / 2 * Mathf.PI / 180;

        float height = Mathf.Tan(angle) * dis * m_spriteRender.sprite.pixelsPerUnit * 2;
        float width = height * (m_width / m_height);
        Debug.Log(string.Format("Height: {0}, Width: {1}", height, width));

        float textureW = m_spriteRender.sprite.texture.width;
        float textureH = m_spriteRender.sprite.texture.height;
        Debug.Log(string.Format("textureW: {0}, textureH: {1}", textureW, textureH));

        float scaleX = width / textureW;
        float scaleY = height / textureH;
        if (m_PreserveAspect)
        {
            float scaleMax = Mathf.Max(scaleX, scaleY);
            transform.localScale = new Vector3(scaleMax, scaleMax);
        }
        else
        {
            transform.localScale = new Vector3(scaleX, scaleY);
        }
    }
}
