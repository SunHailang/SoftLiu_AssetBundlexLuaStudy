using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class FrontEnd : MonoBehaviour
{
    string m_assetBundlesPath = null;

    public void BtnSetting_OnClick()
    {
        Debug.Log("FrontEnd Setting Button OnClick.");
        m_assetBundlesPath = Application.dataPath + "/../Builds/AssetBundles/Android/prefab";
        // load manifest
        string manifestFile = Path.GetFileNameWithoutExtension(m_assetBundlesPath) + ".manifest";
        StartCoroutine(GetAssetBundle(manifestFile, (request) =>
        {
        AssetBundle assetBundle = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
        UnityEngine.Object obj = assetBundle.LoadAsset("AssetBundleManifest");
        assetBundle.Unload(false);
        AssetBundleManifest manifest = obj as AssetBundleManifest;
        List<AssetBundle> depenceAssetBundles = new List<AssetBundle>(); //用来存放加载出来的依赖资源的AssetBundle
        string[] dependences = manifest.GetAllDependencies("prefab");
        Debug.Log("依赖文件个数：" + dependences.Length);
        int length = dependences.Length;
        int finishedCount = 0;
        if (length == 0)
        {
            //没有依赖

        }
        else
        {
            //有依赖，加载所有依赖资源
            for (int i = 0; i < length; i++)
            {
                string dependenceAssetName = dependences[i];
                dependenceAssetName = Path.Combine(Application.dataPath, "/../Builds/AssetBundles/Android/" + dependenceAssetName);
                GetAssetBundle(dependenceAssetName, (request1) =>
            {
                int index = dependenceAssetName.LastIndexOf("/");
                string assetName = dependenceAssetName.Substring(index + 1);
                assetName = assetName.Replace(assetTail, "");
                AssetBundle assetBundle = www.assetBundle;
                UnityEngine.Object obj = assetBundle.LoadAsset(assetName);
                //assetBundle.Unload(false);
                depenceAssetBundles.Add(assetBundle);
                finishedCount++;
                if (finishedCount == length)
                {
                    //依赖都加载完了
                    action(depenceAssetBundles);
                }
            });
            }
        }));
    }

    IEnumerator GetAssetBundle(string path, System.Action<UnityWebRequest> callback)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(m_assetBundlesPath);
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            callback(request);
        }
    }

    public void BtnPlay_OnClick()
    {
        Debug.Log("FrontEnd Play Button OnClick.");
    }
}
