using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_parallaxEffectMultiplier = Vector2.one;

    private Transform m_cameraTrans = null;

    private Vector3 m_lastCameraPosition = Vector3.zero;

    private void Start()
    {
        m_cameraTrans = Camera.main.transform;
        m_lastCameraPosition = m_cameraTrans.position;
    }


    private void LateUpdate()
    {
        Vector3 deltaMovement = m_cameraTrans.position - m_lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * m_parallaxEffectMultiplier.x, deltaMovement.y * m_parallaxEffectMultiplier.y, deltaMovement.z);
        m_lastCameraPosition = m_cameraTrans.position;
    }
}
