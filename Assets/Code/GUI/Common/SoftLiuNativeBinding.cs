using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftLiuNativeBinding
{
    private static SoftLiuNativeBinding m_instance = null;
    public static SoftLiuNativeBinding Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SoftLiuNativeBinding();
            }
            return m_instance;
        }
    }

    private AndroidJavaObject m_javaObject;

    public SoftLiuNativeBinding()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        if (jc == null)
        {
            Debug.LogError("Could not find class com.unity3d.player.UnityPlayer!");
        }

        // find the plugin instance
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        if (jo == null)
        {
            Debug.LogError("Could not find currentActivity!");
        }

        using (var nativeClass = new AndroidJavaClass("com.softliu.hlsun.SoftLiuNative"))
        {
            m_javaObject = nativeClass.CallStatic<AndroidJavaObject>("Init", jo);
        }
#endif
    }

    public string GetBuildleVersion()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return m_javaObject.Call<string>("GetBuildleVersion");
#endif
        return "GetBuildleVersion Editor";
    }

    public string GetCurrentAPKPath()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return m_javaObject.Call<string>("GetCurrentAPKPath");
#endif
        return "GetCurrentAPKPath Editor";
    }

    public string GetUniqueDeviceIdentifier()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return m_javaObject.Call<string>("GetUniqueDeviceIdentifier");
#endif
        return "GetUniqueDeviceIdentifier Editor";
    }

    public string GetMACAddress()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return m_javaObject.Call<string>("GetMACAddress");
#endif
        return "GetMACAddress Editor";
    }

    public void ToggleSpinner(bool enable, float x, float y)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        m_javaObject.Call("ToggleSpinner",enable, x, y);
#endif
        Debug.Log("ToggleSpinner Editor");
    }
}
