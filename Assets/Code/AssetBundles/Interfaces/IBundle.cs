using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.AssetBundles
{
    public enum BundleType
    {
        LevelsType,
        PrefabType,
        ScriptsType,
    }

    public interface IBundle
    {
        string bundleName { get; }
        string bundlePath { get; }
        BundleType bundleType { get; }

    }
}
