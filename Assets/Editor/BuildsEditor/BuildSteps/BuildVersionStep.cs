using SoftLiu.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    public class BuildVersionStep : IBuildStep
    {
        [MenuItem("SoftLiu/Misc/Build Version")]
        public static void BuildVersionModify()
        {
            BuildVersionWindow.Init((buildData) =>
            {
                ModifyBuildVersion(buildData);
            });
        }

        public void Execute(BuildTarget target, BuildType type, string path)
        {
            if (target == BuildTarget.Android)
            {
                if (type == BuildType.Development)
                {
                    ModifyBuildVersion();
                }
            }
            else if (target == BuildTarget.iOS)
            {

            }
        }

        private static void ModifyBuildVersion(BuildVersionData buildData = null)
        {
            try
            {
                string versionJson = Path.Combine(Application.dataPath, "Misc/buildVersion.json");
                if (buildData != null)
                {
                    if (File.Exists(versionJson)) File.Delete(versionJson);
                    File.WriteAllText(versionJson, JsonUtility.ToJson(buildData));
                    AssetDatabase.Refresh();
                }
                else
                {
                    BuildVersionData versionData;
                    using (StreamReader sr = new StreamReader(versionJson))
                    {
                        string datas = sr.ReadToEnd();
                        versionData = JsonUtility.FromJson<BuildVersionData>(datas);
                    }
                    string versionName = versionData.defVersionName;
                    string[] versions = versionName.Split('.');
                    if (versions.Length != 4)
                    {
                        Debug.LogError("ModifyBuildVersion versionName: " + versionName);
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(versions[0] + ".");
                        sb.Append(versions[1] + ".");
                        sb.Append(versions[2] + ".");
                        string versionIndex = "00";
                        if (!string.IsNullOrEmpty(versions[3]))
                        {
                            string indexStr = versions[3].Substring(versions[3].Length - 2);
                            int result = 0;
                            int.TryParse(indexStr, out result);
                            result++;
                            versionIndex = result.ToString("00");
                        }
                        sb.Append(string.Format("{0}{1}{2}", DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"), versionIndex));
                        versionData.defVersionName = sb.ToString();
                        if (File.Exists(versionJson)) File.Delete(versionJson);
                        File.WriteAllText(versionJson, JsonUtility.ToJson(versionData));
                        AssetDatabase.Refresh();
                    }
                }
            }
            catch (Exception error)
            {
                Debug.LogError("ModifyBuildVersion Error: " + error.Message);
            }
        }

        public BuildStepType GetBuildType()
        {
            return BuildStepType.Pre;
        }
    }

    public class BuildVersionWindow : EditorWindow
    {
        private static System.Action<BuildVersionData> m_callback = null;

        private string m_versionJson = string.Empty;

        private BuildVersionData m_buildVersionData = null;

        private string defVersionName = "";
        private string defVersionCode = "";
        private string defTargetSdkVersion = "";

        public static void Init(System.Action<BuildVersionData> action)
        {
            m_callback = action;
            BuildVersionWindow window = (BuildVersionWindow)EditorWindow.GetWindow(typeof(BuildVersionWindow), false, "Build Version Window", true);
            //window.autoRepaintOnSceneChange = true;
            window.Show();
        }

        private void OnEnable()
        {
            m_versionJson = Path.Combine(Application.dataPath, "Misc/buildVersion.json");
            if (!File.Exists(m_versionJson)) return;
            try
            {
                using (StreamReader sr = new StreamReader(m_versionJson))
                {
                    string datas = sr.ReadToEnd();
                    m_buildVersionData = JsonUtility.FromJson<BuildVersionData>(datas);
                }
                if (m_buildVersionData != null)
                {
                    defVersionName = m_buildVersionData.defVersionName;
                    defVersionCode = m_buildVersionData.defVersionCode.ToString();
                    defTargetSdkVersion = m_buildVersionData.defTargetSdkVersion.ToString();
                }
            }
            catch (Exception error)
            {
                Debug.LogError("BuildVersionWindow OnEnable Error: " + error.Message);
            }
        }

        private void OnGUI()
        {
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.Label("defVersionName: ");
            defVersionName = GUILayout.TextArea(defVersionName, GUILayout.Width(150), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("defVersionCode: ");
            defVersionCode = GUILayout.TextField(defVersionCode, GUILayout.Width(150), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("defTargetSdkVersion: ");
            defTargetSdkVersion = GUILayout.TextField(defTargetSdkVersion, GUILayout.Width(150), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical();
            if (GUILayout.Button("Sure"))
            {
                Debug.Log("Sure On Click");
                string versionName = defVersionName.Trim();
                string[] names = versionName.Split('.');
                if (names.Length != 4)
                {
                    Debug.LogError("defVersionName Error: " + defVersionName);
                    return;
                }
                int versionCode = 0;
                int.TryParse(defVersionCode, out versionCode);
                int targetSdkVersion = 0;
                int.TryParse(defTargetSdkVersion, out targetSdkVersion);
                m_buildVersionData.defVersionName = versionName;
                m_buildVersionData.defVersionCode = versionCode;
                m_buildVersionData.defTargetSdkVersion = targetSdkVersion;
                Debug.Log("Build Version Data ： " + JsonUtility.ToJson(m_buildVersionData));
                if (m_callback != null)
                    m_callback(m_buildVersionData);
            }
            GUILayout.EndVertical();
        }
    }
}
