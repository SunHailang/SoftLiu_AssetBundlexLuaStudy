using Assets.Code.Utils.ZipData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Xml.Serialization;
using SoftLiu.Misc;

public class SplashLoader : MonoBehaviour
{
    [SerializeField]
    private Slider m_sliderProcess = null;
    [SerializeField]
    private TextMeshProUGUI m_textProcess = null;

    [SerializeField]
    private TextMeshProUGUI m_textDesc = null;
    [SerializeField]
    private Image m_imageIcon = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Application.persistentDataPath);


        m_sliderProcess.value = 0;
        m_textProcess.SetText(string.Format("{0}%", 0));
    }

    public void BtnGetServerTime()
    {
        UnityRequestManager.Instance.GetServerTime((datas, time) =>
        {
            Debug.Log("Time: " + time);

        });
    }

    public void BtnGetVersion_OnClick()
    {
        string m_serverURL = "http://localhost:8080/AssetBundles";
        Dictionary<string, object> headers = new Dictionary<string, object>();
        headers.Add("Content-Function", "AssetBundles");
        headers.Add("Content-GameID", "SoftLiu");
        headers.Add("Content-Platform", "Android");
        headers.Add("Content-Version", "0.0.9.zip");
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
                    switch (checkData.Type)
                    {
                        case VersionCheckType.UpdateType:
                            string versionName = checkData.m_version.Substring(0, checkData.m_version.LastIndexOf('.'));
#if UNITY_ERITOR
                            bool result = UnityEditor.EditorUtility.DisplayDialog("更新", "更新到版本: " + versionName, "确定", "取消");
                            if (result)
                            {
                                //UnityRequestManager.Instance.DownloadHandlerBufferGet
                                string path = Application.streamingAssetsPath + "/AssetBundles/Android";
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                FileUtils.DeleteDirectory(path);

                                StartCoroutine(DownAssetBundle(path, checkData.m_version));
                            }
#endif
                            break;
                        case VersionCheckType.LatestType:
#if UNITY_EDITOR
                            UnityEditor.EditorUtility.DisplayDialog("更新", "已经是最新版本了", "确定");
#endif
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

    IEnumerator DownAssetBundle(string filePath, string version)
    {
        yield return null;

        string m_serverURL = "http://localhost:8080/AssetBundles";
        Dictionary<string, object> headers = new Dictionary<string, object>();
        headers.Add("Content-Function", "AssetBundles");
        headers.Add("Content-GameID", "SoftLiu");
        headers.Add("Content-Platform", "Android");
        headers.Add("Content-Version", version);
        headers.Add("Content-VersionCheck", "False");

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
            string fileFullPath = filePath + "/" + version;
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

    public void BtnZip_OnClick()
    {
        StartCoroutine(ZipFile());
    }

    IEnumerator ZipFile()
    {
        string path = Application.dataPath + "/../Builds";
        ZipHandlerData zipData = new ZipHandlerData();

        SharpZipUtility.ZipFie(path + "/xLua", path + "/xLua.zip", zipData);

        while (!zipData.isDone && !zipData.isError)
        {
            m_sliderProcess.value = zipData.progress;
            m_textProcess.text = string.Format("{0}%", Mathf.FloorToInt(zipData.progress * 100));
            yield return null;
        }

        m_sliderProcess.value = 1;
        m_textProcess.text = string.Format("{0}%", 100);

        if (!zipData.isError)
        {
            Debug.Log(string.Format("Zip Complete: {0} , {1} , {2}", zipData.progress, zipData.fileCount, zipData.fileCurrentCount));
        }
        else
        {
            Debug.Log("Error: " + zipData.errorText);
        }
    }

    public void BtnUnZip_OnClick()
    {
        StartCoroutine(UnZipFile());
    }
    IEnumerator UnZipFile()
    {
        string fileDir = Application.dataPath + "/../Builds/";
        string target = Application.dataPath + "/../Builds/xLua.zip";
        ZipHandlerData zipData = new ZipHandlerData();

        SharpZipUtility.UnZipFile(target, fileDir, zipData);

        while (!zipData.isDone && !zipData.isError)
        {
            m_sliderProcess.value = zipData.progress;
            m_textProcess.text = string.Format("{0}%", Mathf.FloorToInt(zipData.progress * 100));
            yield return null;
        }

        if (!zipData.isError)
        {
            m_sliderProcess.value = 1;
            m_textProcess.text = string.Format("{0}%", 100);
            Debug.Log(string.Format("UnZip Complete: {0} , {1} , {2}", zipData.progress, zipData.fileCount, zipData.fileCurrentCount));
        }
        else
        {
            Debug.Log("Error: " + zipData.errorText);
        }
    }

    public void BtnPOST_OnClick()
    {
        string m_serverURL = "http://localhost:8080/";
        Dictionary<string, object> headers = new Dictionary<string, object>();
        Dictionary<string, object> formDic = new Dictionary<string, object>();
        formDic.Add("One", "Hello World.");
        formDic.Add("Two", "Hello Chine.");

        StringBuilder sb = new StringBuilder();
        int index = 0;
        foreach (KeyValuePair<string, object> form in formDic)
        {
            sb.Append(string.Format("{0}={1}", form.Key, form.Value));
            if (index < formDic.Count - 1)
            {
                sb.Append('&');
            }
            index++;
        }
        byte[] postData = Encoding.UTF8.GetBytes(sb.ToString());
        StartCoroutine(PostRequest(m_serverURL, postData));

        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(m_serverURL);
        request.Method = "POST";
        request.Timeout = 15;
        request.ContentType = "application/json;charset=UTF-8";
        using (StreamWriter st = new StreamWriter(request.GetRequestStream(), Encoding.GetEncoding("utf-8")))
        {
            st.WriteLine(sb.ToString());
            st.Flush();
        }
        using (var response = request.GetResponse())
        {
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string data = reader.ReadToEnd();
                Debug.Log("Post Callback: " + data);
            }
        }
    }

    IEnumerator PostRequest(string url, byte[] postData)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            request.timeout = 15;
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SendWebRequest();
            while (request.uploadProgress < 1)
            {
                Debug.Log("uploadProgress: " + request.uploadProgress);
                yield return null;
            }
            Debug.Log("upload finished");
            while (!request.isDone)
            {

            }
            string down = request.downloadHandler.text;
            Debug.Log(down);
        }
    }

    public void BtnDownloadImage_OnClick()
    {
        StartCoroutine(DownLoadImage());
    }
    IEnumerator DownLoadImage()
    {
        string url = "http://localhost:8080/favicon.ico";
        using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
        {
            request.downloadHandler = new DownloadHandlerTexture();
            request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                while (!request.isDone)
                {
                    m_sliderProcess.value = request.downloadProgress;
                    m_textProcess.text = string.Format("{0}%", Mathf.FloorToInt(request.downloadProgress * 100));
                    yield return null;
                }
                m_sliderProcess.value = 1;
                m_textProcess.text = string.Format("{0}%", 100);

                Dictionary<string, string> responseHanders = request.GetResponseHeaders();
                if (responseHanders != null)
                {
                    if (responseHanders.ContainsKey("Content-Length"))
                    {
                        long length = 0;
                        long.TryParse(responseHanders["Content-Length"], out length);
                        if (request.downloadHandler.data.Length != length)
                        {
                            Debug.Log("接收数据大小出错.");
                        }
                        else
                        {
                            Debug.Log("接收数据大小完整.");
                        }
                    }
                    if (responseHanders.ContainsKey("Content-disposition"))
                    {
                        string dis = responseHanders["Content-disposition"];
                        string value = FileUtils.GetContentDispositionByName(dis, "filename");
                        if (!string.IsNullOrEmpty(value))
                        {
                            File.WriteAllBytes(Application.dataPath + "/../Builds/" + value, request.downloadHandler.data);
                            Debug.Log("Texture Finish.");
                        }
                        else
                        {
                            Debug.Log("Download Error.");
                            Debug.Log(Encoding.UTF8.GetString(request.downloadHandler.data));
                        }
                    }
                    else
                    {
                        Debug.Log("Download Error.");
                        Debug.Log(Encoding.UTF8.GetString(request.downloadHandler.data));
                    }
                }
            }
        }
    }

    public void BtnGetRSAKey_OnClick()
    {
        string path = Application.dataPath + "/../Builds";
        bool result = EncryptUtils.CreateRSAKey(path, "softliu");
        Debug.Log("CreateRSAKey Result: " + result);
    }

    byte[] strData = null;

    public void BtnEncryptRSA_OnClick()
    {
        byte[] data = Encoding.UTF8.GetBytes("Hello World");

        strData = EncryptUtils.EncryptRSA(data, Application.dataPath + "/../Builds/softliu_rsa.pub");
        Debug.Log(Encoding.UTF8.GetString(strData));
    }

    public void BtnDecryptRSA_OnClick()
    {
        byte[] eData = EncryptUtils.DecryptRSA(strData, Application.dataPath + "/../Builds/softliu_rsa");
        string str = Encoding.UTF8.GetString(eData);
        Debug.Log("BtnDecryptRSA_OnClick : " + str);
    }

    public void BtnHandleXML()
    {
        //string path = Path.Combine(Application.dataPath, "../Tools/Android");
        //BuildUtils.HandleAndroidXml(path);
    }

    GameObject m_exitPopup = null;
    // Update is called once per frame
    void Update()
    {
        //if (time >= 5)
        //{
        //    time = 0;
        //    Debug.Log(Time.realtimeSinceStartup);
        //}
        //time += Time.deltaTime;
        //m_sliderProcess.value = m_process;
        m_textProcess.SetText(string.Format("{0}%", Mathf.FloorToInt(m_sliderProcess.value * 100)));
        //#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.GetKey(KeyCode.Escape))
        {
            // 返回键
            Debug.Log("KeyCode.Escape Down.");
            if (m_exitPopup == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Popup_ExitGame");
                m_exitPopup = Instantiate(prefab, transform);
            }
        }
        if (Input.GetKey(KeyCode.Home))
        {
            // Home键
            Debug.Log("KeyCode.Home Down.");
        }
        //#endif
        UnityRequestManager.Instance.OnUpdate();
    }

    public void GetIconBytes()
    {
        byte[] datas = SoftLiuNativeBinding.Instance.GetIconBytes();
        Texture2D texture = new Texture2D(100, 100);
        bool result = texture.LoadImage(datas);
        if (result)
        {
            m_imageIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
        }
        else
        {
            Debug.Log("GetIconBytes Error.");
        }
        //Application.productName
    }

    public void GetBuildleVersion()
    {
        string data = SoftLiuNativeBinding.Instance.GetBuildleVersion();
        m_textDesc.text += data + "\n";
    }

    public void GetCurrentAPKPath()
    {
        string data = SoftLiuNativeBinding.Instance.GetCurrentAPKPath();
        m_textDesc.text += data + "\n";
    }

    public void GetUniqueDeviceIdentifier()
    {
        string data = SoftLiuNativeBinding.Instance.GetUniqueDeviceIdentifier();
        m_textDesc.text += data + "\n";
    }

    public void GetMACAddress()
    {
        string data = SoftLiuNativeBinding.Instance.GetMACAddress();
        m_textDesc.text += data + "\n";
    }
    public void ToggleSpinnerEnable()
    {
        int width = Screen.width;
        int height = Screen.height;

        SoftLiuNativeBinding.Instance.ToggleSpinner(true, 0.995f, 0.995f);
    }
    public void ToggleSpinnerDisable()
    {
        SoftLiuNativeBinding.Instance.ToggleSpinner(false, 0, 0);
    }
}
