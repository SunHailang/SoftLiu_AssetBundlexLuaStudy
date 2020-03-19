using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.AssetBundles
{

    public class Bundle : ScriptableObject, IBundle
    {
        [SerializeField]
        private string m_bundleName;
        public string bundleName { get { return m_bundleName; } set { m_bundleName = value; } }
        [SerializeField]
        private string m_bundlePath;
        public string bundlePath { get { return m_bundlePath; } set { m_bundlePath = value; } }
        [SerializeField]
        private BundleType m_bundleType;
        public BundleType bundleType { get { return m_bundleType; } set { m_bundleType = value; } }
    }
}
