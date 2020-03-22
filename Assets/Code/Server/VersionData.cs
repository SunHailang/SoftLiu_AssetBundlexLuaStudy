using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionData
{
    public string m_versionName = "0.1.0";
    public int m_versionCode = 1;

    public VersionData()
    {

    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}
