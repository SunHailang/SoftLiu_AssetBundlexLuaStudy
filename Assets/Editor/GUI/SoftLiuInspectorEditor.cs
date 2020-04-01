using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CanEditMultipleObjects, CustomEditor(typeof(BackgroundForCamera))]
public class SoftLiuInspectorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BackgroundForCamera background = target as BackgroundForCamera;
        if (background != null)
        {
            
        }
    }

}
