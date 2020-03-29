using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRenderHit : MonoBehaviour
{
    [SerializeField]
    private Transform m_spriteRender = null;
    [SerializeField]
    private PingPoint m_pingPoint = null;

    private List<Collider2D> m_alreadyPingList = new List<Collider2D>();

    private float m_rotationSpeed = 45f;

    private void Update()
    {
        float preR = (m_spriteRender.eulerAngles.z % 360) - 180;
        m_spriteRender.eulerAngles -= new Vector3(0, 0, m_rotationSpeed * Time.deltaTime);
        float curR = (m_spriteRender.eulerAngles.z % 360) - 180;

        if (preR < 0 && curR >= 0)
        {
            m_alreadyPingList.Clear();
        }
        RaycastHit2D[] raycastHit2DArray = Physics2D.RaycastAll(m_spriteRender.position, GetVectorFromAngle(m_spriteRender.eulerAngles.z), 95f);
        foreach (RaycastHit2D raycastHit2D in raycastHit2DArray)
        {
            if (raycastHit2D.collider != null)
            {
                // Hit something
                if (!m_alreadyPingList.Contains(raycastHit2D.collider))
                {
                    m_alreadyPingList.Add(raycastHit2D.collider);
                    // TODO
                    Debug.Log("Hit Object: " + raycastHit2D.collider.name);
                    PingPoint ping = Instantiate(m_pingPoint, raycastHit2D.transform);
                    ping.SetColor(Color.red);
                }
            }
        }
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

}
