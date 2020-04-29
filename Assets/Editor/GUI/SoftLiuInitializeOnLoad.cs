using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class SoftLiuInitializeOnLoad
{
    static SoftLiuInitializeOnLoad()
    {
        EditorApplication.update += OnUpdate;

        AssetDatabase.importPackageStarted += AssetDatabaseEditor.OnImportPackageStarted;
        AssetDatabase.importPackageCompleted += AssetDatabaseEditor.OnImportPackageCompleted;
        AssetDatabase.importPackageCancelled += AssetDatabaseEditor.OnImportPackageCancelled;
        AssetDatabase.importPackageFailed += AssetDatabaseEditor.OnImportPackageFailed;
    }

    private static void OnUpdate()
    {

    }
}
