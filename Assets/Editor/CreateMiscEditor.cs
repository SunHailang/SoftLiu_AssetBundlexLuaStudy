using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using SoftLiu.AssetBundles;

public class CreateMiscEditor
{
    [MenuItem("SoftLiu/Misc/Editor/Create AssetBundleData")]
    public static void CreateAssetBundleDataEditor()
    {
        CreateAsset<AssetBundleData>();
    }

    [MenuItem("SoftLiu/Misc/Editor/Create Bundles")]
    public static void CreateBundlesEditor()
    {
        CreateAsset<Bundle>();
    }


    private static T CreateAsset<T>(string assetPath = null) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        if (string.IsNullOrEmpty(assetPath))
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            assetPath = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
        }

        AssetDatabase.CreateAsset(asset, assetPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }

}
