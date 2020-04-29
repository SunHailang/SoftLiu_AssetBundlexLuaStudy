using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

/// <summary>
/// 切换Active平台的回调
/// </summary>
public class ActiveBuildTargetChanged : IActiveBuildTargetChanged
{
    /// <summary>
    /// 返回 回调顺序， 值越小越先调用
    /// </summary>
    public int callbackOrder
    {
        get
        {
            return -1;
        }
    }

    public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
    {
        Debug.Log(string.Format("OnActiveBuildTargetChanged previousTarget:{0} , newTarget:{1}", previousTarget, newTarget));
    }
}
