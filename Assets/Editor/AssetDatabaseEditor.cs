using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class AssetDatabaseEditor
{

    public static void OnImportPackageStarted(string packageName)
    {
        Debug.Log(string.Format("Import {0} Started."));
    }

    public static void OnImportPackageCompleted(string packageName)
    {
        Debug.Log(string.Format("Import {0} Completed."));
    }

    public static void OnImportPackageCancelled(string packageName)
    {
        Debug.Log(string.Format("Import {0} Cancelled."));
    }

    public static void OnImportPackageFailed(string packageName, string errorMessage)
    {
        Debug.Log(string.Format("Import {0} Failed."));
    }

}
