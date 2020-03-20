using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnityRequestManager
{
    private static UnityRequestManager m_instance = null;

    public static UnityRequestManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new UnityRequestManager();
            }
            return m_instance;
        }
    }

    List<RequestHandler> m_requestList = null;

    public UnityRequestManager()
    {
        m_requestList = new List<RequestHandler>();
    }

    ~UnityRequestManager()
    {
        m_requestList = null;
    }


    public void OnUpdate()
    {
        for (int i = m_requestList.Count - 1; i >= 0; i--)
        {
            if (m_requestList[i].IsProcessDone())
            {
                m_requestList.RemoveAt(i);
            }
        }
    }

    public void DownloadHandlerBufferGet(string url, Action<byte[], string> finished)
    {
        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        RequestHandler handler = new RequestHandler(request);
        handler.onFinished = finished;
        m_requestList.Add(handler);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SendWebRequest();
    }

}

public class RequestHandler
{
    public Action<byte[], string> onFinished = null;

    private UnityWebRequest m_request = null;

    public RequestHandler(UnityWebRequest request)
    {
        m_request = request;
    }

    public bool IsProcessDone()
    {
        if (m_request == null)
        {
            onFinished(null, "Request is null.");
            return true;
        }
        if (m_request.isHttpError || m_request.isNetworkError)
        {
            onFinished(null, m_request.error);
            return true;
        }
        if (m_request.isDone)
        {
            if (m_request.downloadHandler == null)
            {
                onFinished(null, "Request downloadHandler is null.");
                return true;
            }
            onFinished(m_request.downloadHandler.data, null);
            return true;
        }
        return false;
    }
}
