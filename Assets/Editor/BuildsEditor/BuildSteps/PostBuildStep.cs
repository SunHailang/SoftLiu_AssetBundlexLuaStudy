using System.IO;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    public class PostBuildStep : IBuildStep
    {
        public void Execute(BuildTarget target, BuildType type, string path)
        {
            if (target == BuildTarget.iOS)
            {

            }
            else if (target == BuildTarget.Android)
            {
                PostBuildAndroid(type, path);
            }
        }

        public BuildStepType GetBuildType()
        {
            return BuildStepType.Post;
        }

        private void PostBuildAndroid(BuildType type, string path)
        {
            if (EditorUserBuildSettings.exportAsGoogleAndroidProject)
            {
                Debug.Log("ExportAsGooleAndroidProject is switched on. Running Android Greadle PostProcess.");
                // handle unityLibrary AndroidManifest.xml
                BuildUtils.HandleAndroidXml(path);

                // Patch the Perforce ridiculuseness
                string gradlePath = Path.Combine(Application.dataPath, "../JenkinsScripts/AndroidGradleStuff");
                FileUtils.CopyDirectoryFiles(new DirectoryInfo(gradlePath), new DirectoryInfo(path), true, true);
                string androidPath = Path.Combine(Application.dataPath, "../Tools/Android/Builds");
                FileUtils.CopyDirectoryFiles(new DirectoryInfo(androidPath), new DirectoryInfo(path), true, true);

                string gradleBuildType = "Debug";
                if (type == BuildType.Preproduction || type == BuildType.Production)
                {
                    gradleBuildType = "Release";
                }
                try
                {
                    // Run APK support
                    BuildUtils.RunGradleProcess(path, gradleBuildType);
                    // Bundle support
                    BuildUtils.RunGradleProcess(path, gradleBuildType, "bundle");
                }
                catch (System.Exception error)
                {
                    Debug.LogError("Android build python process failed. msg : " + error.Message);
                }
            }
        }
    }
}
