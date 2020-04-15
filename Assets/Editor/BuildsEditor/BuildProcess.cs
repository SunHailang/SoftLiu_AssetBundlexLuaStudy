using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SoftLiu.Build
{
    public class BuildProcess
    {
        // Flag to perform Direct build steps
        public static bool PerformUnityBuildSteps = true;

        public static BuildType BuildType = BuildType.Development;

        private static readonly List<IBuildStep> m_steps;

        static BuildProcess()
        {
            SetupBuildType();

            m_steps = new List<IBuildStep>();
            m_steps.Add(new EditorCleanStep());
            m_steps.Add(new SetupUnityBuildStep());
            m_steps.Add(new BuildVersionStep());
            m_steps.Add(new PlayerBuildStep());
            m_steps.Add(new PostBuildStep());
        }

        public static string GetBuildPath(BuildTarget target, BuildType type, bool fromJenkins = false)
        {
            string path = Application.dataPath + "/../Builds/";
            switch (target)
            {
                case BuildTarget.iOS:
                    path += "iOS/";
                    break;
                case BuildTarget.Android:
                    path += "Android/";
                    break;
                default:
                    path += "Unkown/";
                    break;
            }
            path += Application.productName + "_" + type.ToString() + "/";
            //path += GetBuildOutputName(target, type);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                FileUtil.DeleteFileOrDirectory(path);
            }
            // To prevent unity throwing exception on 5.6+
            string absolutePath = Path.GetFullPath(path);
            return absolutePath;
        }
        public static string GetBuildOutputName(BuildTarget target, BuildType type)
        {
            string toRet = PlayerSettings.productName.Replace(" ", "") + "_" + type;
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                toRet += ".exe";
            }
            return toRet;
        }

        public static List<IBuildStep> GetBuildSteps(BuildStepType type)
        {
            return m_steps.Where(s => s.GetBuildType() == type).ToList();
        }

        public static List<IBuildStep> GetStepSorted()
        {
            List<IBuildStep> steps = new List<IBuildStep>();
            steps.AddRange(GetBuildSteps(BuildStepType.Pre));
            steps.AddRange(GetBuildSteps(BuildStepType.Direct));
            steps.AddRange(GetBuildSteps(BuildStepType.Post));
            return steps;
        }

        public static void Excute(BuildTarget target, BuildType type, string path, bool runAfterBuild = false)
        {
            Debug.Log("Marking build " + target + " to " + path);
            BuildType = type;
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Compiling", "Please wait for the Editor to finish compiling.", "OK");
                return;
            }
            try
            {
                // Execute all steps in sequence
                BuildStepExecutor.Execute(GetStepSorted(), target, type, path);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        private static void SetupBuildType()
        {
            BuildType = BuildType.Development;
#if PREPRODUCTION
            BuildType = BuildType.Preproduction;
#elif PRODUCTION
            BuildType = BuildType.Production;
#endif
        }
    }
}
