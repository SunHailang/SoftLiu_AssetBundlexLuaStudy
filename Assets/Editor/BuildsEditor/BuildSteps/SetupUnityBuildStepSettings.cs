using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace SoftLiu.Build
{
    public class SetupUnityBuildStepSettings
    {
        public static readonly Dictionary<BuildTarget, Dictionary<BuildType, string>> PreprocessorDefines = new Dictionary<BuildTarget, Dictionary<BuildType, string>>()
        {
            {
                BuildTarget.iOS, new Dictionary<BuildType, string>
                {
                    {BuildType.Development, "ENABLE_LOG;"},
                    {BuildType.Preproduction, "PREPRODUCTION;ENABLE_LOG;"},
                    {BuildType.Production, "PRODUCTION;"},
                    {BuildType.Marketing, ";"}
                }
            },
            {
                BuildTarget.Android, new Dictionary<BuildType, string>
                {
                    {BuildType.Development, "ENABLE_LOG;"},
                    {BuildType.Preproduction, "PREPRODUCTION;ENABLE_LOG;"},
                    {BuildType.Production, "PRODUCTION;"},
                    {BuildType.Marketing, ";"}
                }
            }
        };

    }
}
