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

    public class Bundles : ScriptableObject
    {
        [SerializeField]
        private IBundle[] m_bundles;

    }
}
