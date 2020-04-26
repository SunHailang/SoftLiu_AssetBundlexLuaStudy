using SoftLiu.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class BootLoader : MonoBehaviour
{
    string m_assetBundlesPath = null;

    private string m_persistentDataPath = null;
    private string m_assetBundlePath = null;

    string m_serverURL = "http://localhost:8080/AssetBundles";
    private Dictionary<string, uint> m_downloadFileList = null;
    private Dictionary<string, uint> m_locationFileList = null;
    private void Awake()
    {
        m_persistentDataPath = SoftLiuNativeBinding.Instance.GetPersistentDataPath();
        m_assetBundlePath = Path.Combine(m_persistentDataPath, "ABFile/");
    }

    private void Start()
    {
        // CRC 校验
        string localCRC = Path.Combine(m_assetBundlePath, "assetbundles.crc");
        if (File.Exists(localCRC))
        {
            m_locationFileList = GetCRCFileList(localCRC);
            Debug.Log("m_locationFileList Length: " + m_locationFileList.Count);
        }
    }
    private Dictionary<string, uint> GetCRCFileList(string path)
    {
        if (!File.Exists(path)) return null;
        Dictionary<string, uint> list = new Dictionary<string, uint>();
        using (StreamReader sr = new StreamReader(path))
        {
            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                string[] crc = line.Split('|');
                if (crc.Length == 3)
                {
                    list.Add(crc[0], Convert.ToUInt32(crc[1]));
                }
            }
        }
        return list;
    }

    private Dictionary<string, object> GetAssetBundleHeader(string name)
    {
        Dictionary<string, object> headers = new Dictionary<string, object>();
        headers.Add("Content-Function", "AssetBundles");
        headers.Add("Content-GameID", "SoftLiu");
        headers.Add("Content-Platform", "Android");
        headers.Add("Content-FileName", name);
        return headers;
    }
    int m_downloadTotalCount = 0;
    int m_downloadCount = 0;
    private bool DownloadFile(Dictionary<string, uint> local, Dictionary<string, uint> server)
    {
        Dictionary<string, uint> deletelist = GetDeleteFileList(local, server);
        if (deletelist != null)
        {
            foreach (var item in deletelist)
            {
                string path = Path.Combine(m_assetBundlePath, item.Key);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
        Dictionary<string, uint> downlist = GetDownloadFileList(local, server);
        if (downlist != null)
        {
            m_downloadTotalCount = downlist.Count;
            m_downloadCount = 0;
            foreach (var item in downlist)
            {
                Dictionary<string, object> headers = GetAssetBundleHeader(item.Key);
                UnityRequestManager.Instance.DownloadHandlerBufferGet(m_serverURL, (data, fileName, errorCode) =>
                {
                    if (errorCode == 0)
                    {
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            m_downloadCount++;
                            Debug.Log(string.Format("Download Count: {0}/{1}", m_downloadCount, m_downloadTotalCount));
                            string path = Path.Combine(m_assetBundlePath, fileName);
                            using (StreamWriter sw = new StreamWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write)))
                            {
                                sw.Write(data);
                            }
                        }
                        else
                        {
                            Debug.Log("DownloadHandlerBufferGet File name error: " + Encoding.UTF8.GetString(data));
                        }
                    }
                    else
                    {
                        Debug.Log("Error: " + fileName);
                    }
                }, headers);
            }

        }
        return true;
    }
    private Dictionary<string, uint> GetDeleteFileList(Dictionary<string, uint> local, Dictionary<string, uint> server)
    {
        if (local == null || server == null) return null;
        Dictionary<string, uint> deletelist = new Dictionary<string, uint>();
        IEnumerable<KeyValuePair<string, uint>> allDelete = local.Where(loc =>
        {
            return (!server.ContainsKey(loc.Key));
        });
        foreach (var item in allDelete)
        {
            deletelist.Add(item.Key, item.Value);
        }
        return deletelist;
    }
    private Dictionary<string, uint> GetDownloadFileList(Dictionary<string, uint> local, Dictionary<string, uint> server)
    {
        if (local == null) return server;
        Dictionary<string, uint> downlist = new Dictionary<string, uint>();
        IEnumerable<KeyValuePair<string, uint>> allDif = server.Where(ser =>
        {
            return (!local.ContainsKey(ser.Key) || local[ser.Key] != ser.Value);
        });
        foreach (var item in allDif)
        {
            downlist.Add(item.Key, item.Value);
        }
        return downlist;
    }


    public void BtnLoadServerCRC_OnClick()
    {
        Debug.Log("Button Load Server CRC Click.");
        // Server CRC File
        Dictionary<string, object> headers = GetAssetBundleHeader("assetbundles.crc");
        UnityRequestManager.Instance.DownloadHandlerBufferGet(m_serverURL, (data, fileName, errorCode) =>
        {
            if (errorCode == 0)
            {
                string file = m_assetBundlePath + "/assetbundles_temp.crc";
                if (File.Exists(file))
                {
                    Debug.Log("File exisit: " + file);
                    File.Delete(file);
                }
                using (FileStream fs = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Write(data, 0, data.Length);
                }
                m_downloadFileList = GetCRCFileList(file);
                FileUtils.FileMoveTo(file, m_assetBundlePath + "/assetbundles.crc", true);
                DownloadFile(m_locationFileList, m_downloadFileList);
            }
            else
            {
                Debug.Log("Download CRC File Error: " + fileName);
            }
        }, headers);
        //string m_serverURL = "http://localhost:8080/AssetBundles";
        //Dictionary<string, object> headers = GetAssetBundleHeader("assetbundles.crc");
        //StartCoroutine(GetServerCRCFile(m_serverURL, headers, (response, errorCode) =>
        //{
        //    if (errorCode == 0)
        //    {
        //        string filePath = response;
        //        using (StreamReader sr = new StreamReader(filePath))
        //        {
        //            string line = null;
        //            while ((line = sr.ReadLine()) != null)
        //            {

        //            }
        //        }
        //        string localCRC = Path.Combine(m_assetBundlePath, "assetbundles.crc");
        //        if (!File.Exists(localCRC))
        //        {
        //            // 直接下载

        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("GetServerCRCFile Error: " + response);
        //    }
        //}));
    }

    private IEnumerator GetServerCRCFile(string url, Dictionary<string, object> headers = null, System.Action<string, int> callback = null)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value.ToString());
            }
        }
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError("GetServerCRCFile Error: " + request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            Debug.Log("Download Server CRC File: \n" + response);
            if (response.Contains("errorCode"))
            {
                Dictionary<string, object> errorDic = MiniJSON.Deserialize(response) as Dictionary<string, object>;
                int errorCode = Convert.ToInt32(errorDic["errorCode"]);
                string error = errorDic["error"].ToString();
                callback(error, errorCode);
            }
            else
            {
                if (!Directory.Exists(m_assetBundlePath))
                {
                    Directory.CreateDirectory(m_assetBundlePath);
                }
                string file = m_assetBundlePath + "/assetbundles_temp.crc";
                if (!File.Exists(file))
                {
                    Debug.Log("File not exisit: " + file);
                }
                using (FileStream fs = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Write(request.downloadHandler.data, 0, request.downloadHandler.data.Length);
                }
                callback(file, 0);
            }
        }
    }

    private void Update()
    {
        UnityRequestManager.Instance.OnUpdate();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 70), "Move File"))
        {
            Debug.Log("GUI Button");

            string localCRC = Path.Combine(m_assetBundlePath, "assetbundles_1.crc");
            string target = Path.Combine(m_assetBundlePath, "../ABFile_1/assetbundles_1.crc");

            FileUtils.FileMoveTo(localCRC, target, true);
        }
    }

    IEnumerator GetAssetBundle()
    {
        yield return null;
        //UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(m_assetBundlesPath);
        //yield return request.SendWebRequest();
        //if (request.isHttpError || request.isNetworkError)
        //{
        //    Debug.Log("Error: " + request.error);
        //}
        //else
        {
            //DownloadHandlerAssetBundle bundle = request.downloadHandler as DownloadHandlerAssetBundle;
            //byte[] data = bundle.data;
            //m_assetBundlesPath = Application.dataPath + "/../Builds/AssetBundles/Android/Development/" + Application.version + "/sprite";
            //AssetBundle ab1 = AssetBundle.LoadFromFile(m_assetBundlesPath);
            //Texture2D sprite1 = ab1.LoadAsset<Texture2D>("app_icon");
            m_assetBundlesPath = Application.dataPath + "/../Builds/AssetBundles/Android/Development/" + Application.version + "/prefab";
            AssetBundle ab = AssetBundle.LoadFromFile(m_assetBundlesPath);
            GameObject[] sprite = ab.LoadAllAssets<GameObject>();
            if (sprite == null)
            {
                Debug.Log("AssetBundle  SpriteRenderer is null.");
            }
            else
            {
                //Instantiate(sprite, transform);
                Debug.Log("Length: " + sprite.Length);
                GameObject exit = sprite.Where(item => { return item.name == "FE_Popup_ExitGame"; }).FirstOrDefault();
                if (exit == null)
                {
                    Debug.Log("FE_Popup_ExitGame is null");
                }
                else
                {
                    Instantiate(exit, transform);
                }
            }
            ab.Unload(true);
        }
    }
}
