using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.AssetBundles
{
    [System.Serializable]
    public class LevesBundle : IBundle
    {
        [SerializeField]
        public string bundlePath { get; set; }

    }
}
