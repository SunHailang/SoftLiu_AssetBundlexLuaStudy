using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class ButtonAreaUtils : Image, IPointerEnterHandler
{

    private BoxCollider2D m_btnAreaCoolider2D = null;
    protected override void Awake()
    {
        base.Awake();
        m_btnAreaCoolider2D = GetComponent<BoxCollider2D>();
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
    }

    /// <summary>
    /// 这样的方式 限制点击区域， 前提是要在 Image 的区域内
    /// </summary>
    /// <param name="screenPoint">屏幕坐标系</param>
    /// <param name="eventCamera">摄像机</param>
    /// <returns></returns>
    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        // 需要将屏幕坐标系转换成世界坐标系
        return m_btnAreaCoolider2D.OverlapPoint(eventCamera.ScreenToWorldPoint(screenPoint));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
}
