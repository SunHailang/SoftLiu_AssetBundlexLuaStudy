using SoftLiu.State;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTracker
{

    private class LoadTracker
    {
        public float Progress;
        public bool IsDone;
    }

    private string m_name;
    private string m_previousStateName;

    // Level-loading trackers
    private LoadTracker m_loadTracker;
    public bool m_wasLevelLoaded = false;

    public GameObject m_screenRoot;

    public Collider[] m_initeractables = null;

    Stopwatch m_stopwatch = new Stopwatch();

    public bool IsAudioScene
    {
        get;
        private set;
    }

    public ScreenTracker(string name)
    {
        m_name = name;
        IsAudioScene = name.StartsWith("Audio_");
    }

    public bool Loaded
    {
        get { return m_wasLevelLoaded; }
    }

    public string previousStateName { get { return m_previousStateName; } }

    public string name { get { return m_name; } }

    public void OnUpdate()
    {
        if (!m_wasLevelLoaded)
        {
            bool wasLevelLoadedThisFrame = (m_loadTracker != null && m_loadTracker.IsDone);
            if (wasLevelLoadedThisFrame)
            {
                m_screenRoot = GameObject.Find(m_name);
                OnScreenLoaded();
            }
        }
    }

    public void OnScreenLoaded()
    {
        m_wasLevelLoaded = true;
        if (m_screenRoot != null)
        {

        }
    }

    public void OnEntry(ScreenState.EntryBehaviour entryBehaviour, string previousStateName)
    {
        m_previousStateName = previousStateName;
    }

    private void LoadScene(LoadSceneMode mode)
    {
        m_loadTracker = new LoadTracker();

        SceneManager.LoadScene(m_name, mode);

        //Util.StartCoroutineWithoutMonobehaviour("UpdateLoadTracker", UpdateLoadTracker(m_loadTracker));
    }

    IEnumerator UpdateLoadTracker(LoadTracker loadTracker)
    {
        Scene scene = SceneManager.GetSceneByName(m_name);

        // Even when using non async scene load, still takes one frame to actually load...
        while (!scene.isLoaded)
        {
            yield return null;
        }

        loadTracker.Progress = 1f;
        loadTracker.IsDone = true;
    }

    public void OnScreenDestroyed()
    {
        m_loadTracker = null;
    }

    public void OnExit(State nextState, State.ExitBehaviour exitBehaviour)
    {

    }
}
