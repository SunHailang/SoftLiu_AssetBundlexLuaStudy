using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SampleScene : MonoBehaviour
{
    private Button m_btnLoad = null;

    private string m_url = "../Builds/AssetBundles/Android";

    private void Start()
    {
        m_url = Application.dataPath + "/" + m_url;

        m_btnLoad = transform.Find("Button").GetComponent<Button>();

        m_btnLoad.onClick.AddListener(() =>
        {
            //StartCoroutine(Load());
            StartCoroutine(Download());
        });
    }

    IEnumerator Download()
    {
        yield return null;
        string fileName = "version.txt";
        string versionPath = m_url + "/" + fileName;

        UnityWebRequest request = UnityWebRequest.Get(versionPath);
        yield return request.SendWebRequest();
        //request.downloadProgress
        Debug.Log("Download Complete");

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("Request Error: " + request.error);
        }
        else
        {
            byte[] fileBs = request.downloadHandler.data;

            string dicPath = Application.streamingAssetsPath + "/AssetBundles/Android";
            if (!Directory.Exists(dicPath))
            {
                Directory.CreateDirectory(dicPath);
            }
            string savePath = dicPath + "/" + fileName;

            if (File.Exists(savePath))
            {
                byte[] hasFile = File.ReadAllBytes(savePath);
                if (hasFile.Equals(fileBs))
                {
                    yield break;
                }
            }

            using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(fileBs, 0, fileBs.Length);
            }
            Debug.Log("Write Success");
        }
    }

    IEnumerator Load()
    {
        string url = Application.streamingAssetsPath + "/AssetBundles/testbundle.unity3d";
        CachedAssetBundle cached = new CachedAssetBundle();
        cached.name = url;

        UnityWebRequest download = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return download.SendWebRequest();
        yield return null;

        Debug.Log("Complete...");

        AssetBundle ab = (download.downloadHandler as DownloadHandlerAssetBundle).assetBundle;

        GameObject text = ab.LoadAsset<GameObject>("Text (TMP)");
        GameObject obj = Instantiate(text, transform);

        obj.transform.localPosition = new Vector3(550, 0, 0);
        AssetBundle.UnloadAllAssetBundles(false);
    }
}
