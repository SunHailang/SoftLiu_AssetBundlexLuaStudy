using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using XLua;

public class AssetBundleDownloader : MonoBehaviour
{
    public Slider m_slider = null;
    public TextMeshProUGUI m_progressText = null;

    private string m_downloadURL = "";

    public AssetBundle m_assetBundle = null;

    public void StartDownload()
    {
        Debug.Log("开始下载更新 ...");
        StartCoroutine(GetAssetBundle(ExcuteHotFix));

    }

    private IEnumerator GetAssetBundle(System.Action callBack)
    {
        yield return null;

        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(m_downloadURL);
        request.SendWebRequest();

        while (!request.isDone)
        {
            //下载进度
            m_slider.value = request.downloadProgress;
            m_progressText.SetText(string.Format("下载进度: {0}%", (request.downloadProgress * 100).ToString("F2")));
            yield return null;
        }
        // 下载完成
        if (request.isDone)
        {
            m_progressText.SetText(string.Format("下载进度: {0}%", 100));
            m_slider.value = 1;

            yield return new WaitForSeconds(1);

        }

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Download Error: " + request.error);
        }
        else
        {
            m_assetBundle = DownloadHandlerAssetBundle.GetContent(request);
            TextAsset hot = m_assetBundle.LoadAsset<TextAsset>("luaScript.lua.txt");
            string newPath = Application.persistentDataPath + @"/luaScript,lua.txt";
            if (!File.Exists(newPath))
            {
                File.Create(newPath).Dispose();
            }
            // 写入文件
            File.WriteAllText(newPath, hot.text);

            callBack();
        }
    }

    /// <summary>
    /// 执行热更脚本
    /// </summary>
    private void ExcuteHotFix()
    {
        LuaEnv luaEnv = new LuaEnv();
        luaEnv.AddLoader(MyLoader);
        luaEnv.DoString("require 'luaScript'");
    }

    public byte[] MyLoader(ref string filePath)
    {
        string newPath = Application.persistentDataPath + @"/" + filePath + ".lua.txt";
        string txtString = File.ReadAllText(newPath);
        return System.Text.Encoding.UTF8.GetBytes(txtString);
    }
}
