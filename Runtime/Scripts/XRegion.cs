using System;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CA2235 // Mark all non-serializable fields

namespace TinaX.I18N.Internal
{
    [Serializable]
    public class XRegion
    {
        [Serializable]
        public struct I18NJsonDict
        {
            public string LoadPath;
            public string EditorLoadPath;
            public bool Base64Value;
            public string GroupName;
        }

        [Serializable]
        public struct I18NAssetDict
        {
            public I18NDict Asset;
            public string GroupName;
        }

        public string Name;
        public List<SystemLanguage> BindLanguage;

        public List<I18NJsonDict> JsonDicts;
        public List<I18NAssetDict> AssetDicts;

    }
}
#pragma warning restore CA2235 // Mark all non-serializable fields
