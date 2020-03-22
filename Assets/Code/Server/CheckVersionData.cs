using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum VersionCheckType
{
    None,
    UpdateType,
    LatestType,
}

public class CheckVersionData
{
    public VersionCheckType m_type;
    public string m_version = string.Empty;

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}
