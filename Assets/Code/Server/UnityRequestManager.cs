using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    public void GetServerTime(Action<byte[], string> finished)
    {
        string serverTimeUrl = "https://baidu.com";
        UnityWebRequest request = UnityWebRequest.Get(serverTimeUrl);
        request.timeout = 15;
        Action<byte[], string, int> onGetServerTimeResponseInternal = (byte[] errorStr, string response, int errorcode) =>
        {
            if (string.IsNullOrEmpty(response))
            {
                Dictionary<string, string> headers = request.GetResponseHeaders();
                if (headers.ContainsKey("Date"))
                {
                    string time = headers["Date"];
                    finished(null, time);
                }
            }
            else
            {
                finished(null, null);
            }
        };
        RequestHandler handler = new RequestHandler(request);
        handler.onFinished = onGetServerTimeResponseInternal;
        m_requestList.Add(handler);
        request.SendWebRequest();
    }

    public void DownloadHandlerBufferGet(string url, Action<byte[], string, int> finished, Dictionary<string, object> headers = null)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        if (headers != null)
        {
            foreach (KeyValuePair<string, object> header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value.ToString());
            }
        }
        request.downloadHandler = new DownloadHandlerBuffer();
        RequestHandler handler = new RequestHandler(request);
        handler.onFinished = finished;
        m_requestList.Add(handler);
        request.SendWebRequest();
    }

    public void DownLoadAssetBundlesZip(string url, Action<byte[], string, int> finished, Dictionary<string, object> headers = null, Dictionary<string, object> cookies = null)
    {
        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        if (headers != null)
        {
            foreach (KeyValuePair<string, object> header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value.ToString());
            }
        }
        if (cookies != null)
        {
            request.SetRequestHeader("Cookie", string.Format("session={0}", JsonUtility.ToJson(cookies)));
        }

        RequestHandler handler = new RequestHandler(request);
        handler.onFinished = finished;
        m_requestList.Add(handler);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SendWebRequest();
    }

    public void RequestVersionCheck(string url, Action<byte[], string, int> finished, Dictionary<string, object> headers = null, Dictionary<string, object> cookies = null)
    {
        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        if (headers != null)
        {
            foreach (KeyValuePair<string, object> header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value.ToString());
            }
        }
        if (cookies != null)
        {
            request.SetRequestHeader("Cookie", string.Format("session={0}", JsonUtility.ToJson(cookies)));
        }

        RequestHandler handler = new RequestHandler(request);
        handler.onFinished = finished;
        m_requestList.Add(handler);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SendWebRequest();
    }

    public void RequestPost(string url, Action<byte[], string, int> finished, Dictionary<string, object> headers = null, Dictionary<string, object> cookies = null)
    {
        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        if (headers != null)
        {
            foreach (KeyValuePair<string, object> header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value.ToString());
            }
        }
        if (cookies != null)
        {
            request.SetRequestHeader("Cookie", string.Format("session={0}", JsonUtility.ToJson(cookies)));
        }

        //WWWForm form = new WWWForm();
        //byte[] buffer = Encoding.UTF8.GetBytes("Hello World!");
        //form.AddBinaryData("Hello", buffer);

        RequestHandler handler = new RequestHandler(request);
        handler.onFinished = finished;
        m_requestList.Add(handler);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SendWebRequest();
    }
}

public class RequestHandler
{
    public Action<byte[], string, int> onFinished = null;

    private UnityWebRequest m_request = null;

    public RequestHandler(UnityWebRequest request)
    {
        m_request = request;
    }

    public bool IsProcessDone()
    {
        try
        {
            if (m_request == null)
            {
                onFinished(null, "Request is null.", -1);
                return true;
            }
            if (m_request.isHttpError || m_request.isNetworkError)
            {
                onFinished(null, m_request.error, -1);
                return true;
            }
            if (m_request.isDone)
            {
                if (m_request.downloadHandler == null)
                {
                    onFinished(null, "Request downloadHandler is null.", -1);
                    return true;
                }
                string dis = m_request.GetResponseHeader("Content-disposition");
                string fileName = null;
                if (!string.IsNullOrEmpty(dis))
                {
                    fileName = FileUtils.GetContentDispositionByName(dis, "filename");
                }
                onFinished(m_request.downloadHandler.data, fileName, 0);
                return true;
            }
            return false;
        }
        catch (Exception error)
        {
            Debug.LogError(string.Format("URL: {0} , Error: ", m_request.url, error.Message));
            return true;
        }

    }
}
