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

                // Patch the Perforce ridiculuseness
                //string sourcePath = Path.Combine(Application.dataPath, "../JenkinsScripts/AndroidGradleStuff");
                //string tragetPath = Path.Combine(path, "unityLibrary/");
                //copy gradle
                //FileUtils.CopyDirectoryFiles(new DirectoryInfo(sourcePath), new DirectoryInfo(path), true, true);

            }
        }
    }
}
