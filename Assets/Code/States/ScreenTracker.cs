using SoftLiu.State;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTracker
{
    private string m_name = string.Empty;
    public string Name { get { return m_name; } }

    private AsyncOperation m_screenAsyncOperation = null;
    public AsyncOperation ScreenAsyncOperation { get { return m_screenAsyncOperation; } }

    public ScreenTracker(string name, LoadSceneMode loadSceneMode)
    {
        m_name = name;
        m_screenAsyncOperation = SceneManager.LoadSceneAsync(name, loadSceneMode);
    }
}
