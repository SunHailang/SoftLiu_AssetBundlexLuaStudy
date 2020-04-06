using SoftLiu.Build;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class BuildMenuEditor
{
    private static string GetLoadedScene()
    {
        Scene currentScent = EditorSceneManager.GetActiveScene();
        return currentScent.path;
    }
    private static void PerformBuild(BuildTarget target, BuildType type, bool fromJenkins = false, bool runAfterBuild = false, bool performUnityBuildSteps = true)
    {
        string openedScene = GetLoadedScene();
        BuildProcess.PerformUnityBuildSteps = performUnityBuildSteps;

        string buildPath = BuildProcess.GetBuildPath(target, type, fromJenkins);
        BuildProcess.Excute(target, type, buildPath, runAfterBuild);
        if (!string.IsNullOrEmpty(openedScene) && openedScene != GetLoadedScene())
        {
            EditorSceneManager.OpenScene(openedScene);
        }
    }

    #region Android
    [MenuItem("SoftLiu/Builds/Android/Build Development", false, 50)]
    public static void AndroidBuildEditor_Development()
    {
        Build_Android_Development();
    }

    public static void Build_Android_Development()
    {
        PerformBuild(BuildTarget.Android, BuildType.Development, false);
    }

    #endregion
}
