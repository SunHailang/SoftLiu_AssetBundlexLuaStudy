using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftLiuNativeReceiver : MonoBehaviour
{

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    public void MessageBoxClick(string msg)
    {
        Debug.Log("MessageBoxClick: " + msg);
    }

    public void PermissionReceivedSuccess(string msg)
    {
        Debug.Log("PermissionReceivedSuccess: " + msg);
    }
}
