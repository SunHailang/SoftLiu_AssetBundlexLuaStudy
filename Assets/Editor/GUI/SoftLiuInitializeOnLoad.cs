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
    }

    private static void OnUpdate()
    {

    }
}
