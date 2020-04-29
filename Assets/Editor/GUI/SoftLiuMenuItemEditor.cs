using UnityEditor;
using UnityEngine;

public class SoftLiuMenuItemEditor
{
    [MenuItem("SoftLiu/SwitchPlatform/Android")]
    public static void SoftLiuSwitchPlatform_Android()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            Debug.Log("SwitchPlatform Error: Current Platform is Android.");
            return;
        }
        SwitchActivePlatform(BuildTargetGroup.Android, BuildTarget.Android);
        //EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.Android, BuildTarget.Android);
    }

    [MenuItem("SoftLiu/SwitchPlatform/iOS")]
    public static void SoftLiuSwitchPlatform_iOS()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            Debug.Log("SwitchPlatform Error: Current Platform is iOS.");
            return;
        }
        SwitchActivePlatform(BuildTargetGroup.iOS, BuildTarget.iOS);
        //EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.Android, BuildTarget.Android);
    }

    public static bool SwitchActivePlatform(BuildTargetGroup targetGroup, BuildTarget target)
    {
        return EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
    }
}
