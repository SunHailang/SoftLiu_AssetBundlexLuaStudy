using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SampleScene : MonoBehaviour
{
    private Button m_btnLoad = null;
    private void Start()
    {
        m_btnLoad = transform.Find("Button").GetComponent<Button>();

        m_btnLoad.onClick.AddListener(() =>
        {
            StartCoroutine(Load());
        });
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
