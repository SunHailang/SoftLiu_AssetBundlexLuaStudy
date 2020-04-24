using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Force.Crc32;
using System;
using System.Diagnostics;
using System.Text;
using SoftLiu.Build;

public struct AssetBundleCRCFileInfo
{
    public string m_Name;
    public uint m_CRC;
    public long m_FileSizeBytes;
}

public class AssetBundlesBuilder
{
    public static readonly string AssetBundlesPath = Application.dataPath + "/../Builds/AssetBundles";

    [MenuItem("SoftLiu/AssetBundles/Android/Build Production", false, 0)]
    public static void AssetBundles_BuildAndroidProd()
    {
        // 创建文件夹
        string dir = AssetBundlesPath + "/Android";
        if (Directory.Exists(dir))
        {
            FileUtils.DeleteDirectory(dir);
        }
        else
        {
            Directory.CreateDirectory(dir);
        }
        CreateVersion(dir);
        // 构建
        // 参数1：路径
        // 参数2：压缩算法，none 默认
        // 参数3：设备参数，iOS，Android，windows 等
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.Android);
        UnityEngine.Debug.Log("资源打包完成");
    }

    private static void CreateVersion(string path)
    {
        string verPath = path + "/version.txt";

        using (FileStream sw = new FileStream(verPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            string text = DateTime.Now.Ticks.ToString();
            byte[] bs = Encoding.UTF8.GetBytes(text);
            sw.Write(bs, 0, bs.Length);
        }
    }

    [MenuItem("SoftLiu/AssetBundles/Android/Build Development", false, 0)]
    public static void AssetBundles_BuildAndroidDev()
    {
        BuildTarget platform = BuildTarget.Android;
        BuildType buildType = BuildType.Development;
        string buildDir = Application.dataPath + "/../Builds/AssetBundles/" + platform.ToString() + "/" + buildType.ToString() + "/" + Application.version;
        if (Directory.Exists(buildDir))
        {
            Directory.Delete(buildDir, true);
        }
        Directory.CreateDirectory(buildDir);
        BuildPipeline.BuildAssetBundles(buildDir, BuildAssetBundleOptions.UncompressedAssetBundle, platform);
        GenerateCRCFileInfoPlatform(platform, buildType);
    }

    [MenuItem("SoftLiu/AssetBundles/ENABLE BUNDLES/Levels", priority = 0)]
    public static void EnableLevelsAssetBundles()
    {
        DisableSourceScenes();
        AddAssetBundleDefines(true);
        //AssetBundles.
    }

    public static void DisableSourceScenes()
    {
        string[] sourceScenesAssets = AssetDatabase.FindAssets("l:SourceScene");
        foreach (string sourceSceneGUID in sourceScenesAssets)
        {
            string path = AssetDatabase.GUIDToAssetPath(sourceSceneGUID);

            List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>();
            foreach (EditorBuildSettingsScene scene in list)
            {
                if (path == scene.path)
                {
                    scene.enabled = false;
                }
            }
            EditorBuildSettings.scenes = list.ToArray();
        }
    }
    public static void AddAssetBundleDefines(bool levels)
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
        if (levels && !defines.Contains("ASSETABUNDLES_LEVELS"))
        {
            defines += defines + ";ASSETABUNDLES_LEVELS";
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), defines);
    }
    public static void GenerateCRCFileInfoPlatform(BuildTarget platform, BuildType buildType)
    {
        UnityEngine.Debug.Log("GenerateCRCFileInfo " + platform.ToString());
        try
        {
            string buildDir = Application.dataPath + "/../Builds/AssetBundles/" + platform.ToString() + "/" + buildType.ToString() + "/" + Application.version;
            if (Directory.Exists(buildDir))
            {
                if (File.Exists(buildDir + "/assetbundles.crc"))
                {
                    File.Delete(buildDir + "/assetbundles.crc");
                }
                List<AssetBundleCRCFileInfo> m_crcInfoList = new List<AssetBundleCRCFileInfo>();
                string[] assetBundles = Directory.GetFiles(buildDir);
                foreach (string abFileName in assetBundles)
                {
                    FileInfo abFile = new FileInfo(abFileName);
                    if (abFile.FullName.Contains(".manifest"))
                    {
                        File.Delete(abFile.FullName);
                    }
                    else if (abFile.Name != platform.ToString() && abFile.Name != Application.version)
                    {
                        AssetBundleCRCFileInfo abcrci = new AssetBundleCRCFileInfo();
                        abcrci.m_Name = abFile.Name;
                        abcrci.m_FileSizeBytes = abFile.Length;
                        abcrci.m_CRC = GenerateCRC32FromFile(abFile.FullName);
                        m_crcInfoList.Add(abcrci);
                    }
                }
                string fileData = "";
                foreach (AssetBundleCRCFileInfo abcrci in m_crcInfoList)
                {
                    if (fileData != "")
                    {
                        fileData = fileData + "\n";
                    }
                    fileData = fileData + abcrci.m_Name + "|" + abcrci.m_CRC.ToString() + "|" + abcrci.m_FileSizeBytes;
                }
                File.WriteAllText(buildDir + "/assetbundles.crc", fileData);
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log("GenerateCRCFileInfoPlatform Failed " + e.Message);
        }
    }

    public static uint GenerateCRC32FromFile(string fileName)
    {
#if UNITY_EDITOR
        Stopwatch x = new Stopwatch();
        x.Start();
#endif
        uint crc = 0;
        FileStream fs = null;
        try
        {
            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            int i = 0;
            int block = 4096;
            byte[] buffer = new byte[block];
            int l = (int)fs.Length;
            while (i < l)
            {
                fs.Read(buffer, 0, block);
                crc = Crc32Algorithm.Append(crc, buffer);
                i += block;
            }
            fs.Close();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("AssetBundleUtils | GenerateCRC32FromFile failed to generate CRC: " + fileName + ". Exception: " + e.ToString());
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
            }
        }
#if UNITY_EDITOR
        x.Stop();
        UnityEngine.Debug.Log("CRC32 " + fileName + ":" + x.ElapsedMilliseconds + "ms <> " + crc.ToString());
#endif
        return crc;
    }
}
