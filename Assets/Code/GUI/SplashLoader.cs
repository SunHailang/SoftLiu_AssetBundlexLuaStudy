using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SplashLoader : MonoBehaviour
{
    [SerializeField]
    private Slider m_sliderProcess = null;
    [SerializeField]
    private TextMeshProUGUI m_textProcess = null;

    // Start is called before the first frame update
    void Start()
    {
        //string dir = Application.dataPath + "/../xLua";
        //FileUtils.DeleteDirectory(dir);
        //Debug.Log("Delete Complete.");

        m_sliderProcess.value = 0;
        m_textProcess.SetText(string.Format("{0}%", 0));
    }

    public void BtnGetVersion_OnClick()
    {
        string m_serverURL = "http://localhost:8080/AssetBundles/";
        Dictionary<string, object> headers = new Dictionary<string, object>();
        headers.Add("Content-Function", "AssetBundles");
        headers.Add("Content-GameID", "SoftLiu");
        headers.Add("Content-Platform", "Android");
        headers.Add("Content-Version", "0.0.9");
        headers.Add("Content-VersionCheck", "True");
        Dictionary<string, object> cookies = new Dictionary<string, object>();
        cookies.Add("TestGroup", "test");
        UnityRequestManager.Instance.RequestVersionCheck(m_serverURL, (data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Debug.Log(data.Length);
                string info = Encoding.UTF8.GetString(data);
                Debug.Log("Callback Info : " + info);
                CheckVersionData checkData = JsonUtility.FromJson<CheckVersionData>(info);
                if (checkData == null)
                {
                    Debug.Log("RequestVersionCheck Response Data is null.");
                }
                else
                {
                    switch (checkData.m_type)
                    {
                        case VersionCheckType.UpdateType:
                            bool result = UnityEditor.EditorUtility.DisplayDialog("更新", "更新到版本: " + checkData.m_version, "确定", "取消");
                            if (result)
                            {
                                //UnityRequestManager.Instance.DownloadHandlerBufferGet
                                string path = Application.streamingAssetsPath + "/AssetBundles/Android";
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                FileUtils.DeleteDirectory(path);

                                StartCoroutine(DownAssetBundle(path, checkData.m_version, ".zip"));
                            }
                            break;
                        case VersionCheckType.LatestType:
                            UnityEditor.EditorUtility.DisplayDialog("更新", "已经是最新版本了", "确定");
                            break;
                    }
                }
            }
            else
            {
                Debug.Log(error);
            }
        }, headers, cookies);
    }

    IEnumerator DownAssetBundle(string filePath, string fileName, string ext)
    {
        yield return null;

        string m_serverURL = "http://localhost:8080/AssetBundles/";
        Dictionary<string, object> headers = new Dictionary<string, object>();
        headers.Add("Content-Function", "AssetBundles");
        headers.Add("Content-GameID", "SoftLiu");
        headers.Add("Content-Platform", "Android");
        headers.Add("Content-Version", fileName);
        headers.Add("Content-VersionCheck", "false");

        using (UnityWebRequest request = new UnityWebRequest(m_serverURL, UnityWebRequest.kHttpVerbGET))
        {
            if (headers != null)
            {
                foreach (KeyValuePair<string, object> header in headers)
                {
                    request.SetRequestHeader(header.Key, header.Value.ToString());
                }
            }
            request.timeout = 30;
            string fileFullPath = filePath + "/" + fileName + ext;
            request.downloadHandler = new DownloadHandlerFile(fileFullPath);
            request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log("Download Error : " + request.error);
            }
            else
            {
                while (!request.isDone)
                {
                    float process = request.downloadProgress;
                    m_sliderProcess.value = process;

                    m_textProcess.SetText(string.Format("{0}%", Mathf.FloorToInt(process * 100)));
                    yield return null;
                }

                m_sliderProcess.value = 1;
                m_textProcess.SetText(string.Format("{0}%", 100));

                if (File.Exists(fileFullPath))
                {
                    Dictionary<string, string> responseHanders = request.GetResponseHeaders();
                    if (responseHanders != null)
                    {
                        if (responseHanders.ContainsKey("Content-Length"))
                        {
                            long length = 0;
                            using (FileStream fs = File.OpenRead(fileFullPath))
                            {
                                long.TryParse(responseHanders["Content-Length"], out length);
                                if (fs.Length != length)
                                {
                                    Debug.Log("接收数据大小出错.");
                                }
                                else
                                {
                                    Debug.Log("接收数据大小完整.");
                                }
                            }
                        }
                    }
                    foreach (KeyValuePair<string, string> hander in responseHanders)
                    {
                        Debug.Log(string.Format("{0} -> {1}", hander.Key, hander.Value));
                    }
                }
                else
                {
                    Debug.Log("Download Error.");
                }
            }
        }
    }

    public void BtnUnZip_OnClick()
    {
        m_process = 0;
        //StartCoroutine(UnZipFile());
        UnZipFileThrad();
        try
        {

            Debug.Log("process : " + m_process);
        }
        catch (System.Exception e)
        {
            Debug.Log("BtnUnZip_OnClick : " + e.Message);
        }

    }

    private float m_process = 0;

    private void UnZipFileThrad()
    {
        string fileDir = Application.dataPath + "/../TestFolder";
        string target = fileDir + "/TestFolder.zip";

        Thread th = new Thread(() =>
        {
            bool result = SharpZipUtility.UnZipFile(target, fileDir);
            Debug.Log("Result : " + result);
        });
        th.Start();
    }

    IEnumerator UnZipFile()
    {
        

        yield return null;

        m_sliderProcess.value = 1;
        m_textProcess.SetText(string.Format("{0}%", 100));
    }

    // Update is called once per frame
    void Update()
    {

        m_sliderProcess.value = m_process;
        m_textProcess.SetText(string.Format("{0}%", Mathf.FloorToInt(m_process * 100)));


        UnityRequestManager.Instance.OnUpdate();
    }
}
