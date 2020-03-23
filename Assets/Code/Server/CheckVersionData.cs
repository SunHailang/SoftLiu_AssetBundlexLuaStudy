using System;
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
    public string m_type;
    public string m_version = string.Empty;

    private VersionCheckType m_checkType = VersionCheckType.None;
    public VersionCheckType Type
    {
        get
        {
            if (Enum.TryParse(m_type, out m_checkType))
            {
                return m_checkType;
            }
            return VersionCheckType.None;
        }
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}
