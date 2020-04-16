using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        return m_javaObject.Call<string>("GetCurrentAppVersion");
#endif
        return "GetBuildleVersion Editor";
    }

    public string GetAppName()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return m_javaObject.Call<string>("GetCurrentAppName");
#endif
        return Application.productName;
    }

    public byte[] GetIconBytes()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return m_javaObject.Call<byte[]>("GetCurrentIconBytes");
#endif
        Texture2D datas = Resources.Load<Texture2D>("app_icon");
        return datas.EncodeToPNG();
    }

    public string GetCurrentAPKPath()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return m_javaObject.Call<string>("GetCurrentAPKPath");
#endif
        return "GetCurrentAPKPath Editor";
    }
    /// <summary>
    /// 默认使用 Android ID
    /// </summary>
    /// <returns></returns>
    public string GetUniqueDeviceIdentifier()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return m_javaObject.Call<string>("GetAndroidID");
#endif
        return "GetUniqueDeviceIdentifier Editor";
    }


    public void ToggleSpinner(bool enable, float x = 0f, float y = 0f)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        m_javaObject.Call("ToggleSpinner",enable, x, y);
#endif
        Debug.Log("ToggleSpinner Editor");
    }
}
