using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPingFPS : MonoBehaviour
{
    private const string m_IP = "baidu.com";

    private Queue<Ping> m_pingQueue = new Queue<Ping>();

    private float deltaTime = 0;
    private Queue<float> m_delteTimeQueue = new Queue<float>();

    private void Awake()
    {
        
    }

    private void Start()
    {
        SendPing();
        StartCoroutine(SendPing());
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 20;
        GUI.skin.label.normal.textColor = Color.green;

        float ping = 0;
        int pingFinishCount = 0;
        foreach (var item in m_pingQueue)
        {
            if (item != null && item.isDone)
            {
                pingFinishCount++;
                ping += item.time;
            }
        }
        if (pingFinishCount > 0)
        {
            ping = ping / pingFinishCount;
            GUI.Label(new Rect(50, 20, 500, 30), "Ping: " + Mathf.CeilToInt(ping) + "ms");
        }
        else
        {
            GUI.Label(new Rect(50, 20, 500, 30), "Ping: " + -1 + "ms");
        }

        float fps = 0;//1.0f / deltaTime;
        int count = 0;
        foreach (var item in m_delteTimeQueue)
        {
            if (item <= 0) continue;
            count++;
            fps += item;
        }
        fps = fps / count;
        GUI.Label(new Rect(50, 50, 500, 30), "FPS: " + Mathf.CeilToInt(1.0f / fps));
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        m_delteTimeQueue.Enqueue(deltaTime);
        while (m_delteTimeQueue.Count > 20)
        {
            m_delteTimeQueue.Dequeue();
        }
    }

    private IEnumerator SendPing()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Ping ping = new Ping(m_IP);
            m_pingQueue.Enqueue(ping);
            while (m_pingQueue.Count > 10)
            {
                Ping pingD = m_pingQueue.Dequeue();
                pingD.DestroyPing();
            }
        }
    }
}
