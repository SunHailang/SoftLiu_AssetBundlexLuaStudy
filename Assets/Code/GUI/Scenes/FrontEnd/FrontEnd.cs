using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class FrontEnd : MonoBehaviour
{
    string m_assetBundlesPath = null;

    public void BtnSetting_OnClick()
    {
        Debug.Log("FrontEnd Setting Button OnClick.");
        
        // load manifest
        string manifestFile = Path.GetFileNameWithoutExtension(m_assetBundlesPath) + ".manifest";
        StartCoroutine(GetAssetBundle());
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

    public void BtnPlay_OnClick()
    {
        Debug.Log("FrontEnd Play Button OnClick.");
    }
}
