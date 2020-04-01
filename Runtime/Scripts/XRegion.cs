using System;
using System.Collections.Generic;
using UnityEngine;

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
