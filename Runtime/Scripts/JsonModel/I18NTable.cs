using System.Collections.Generic;
using TinaX.I18N.Internal;

namespace TinaX.I18N
{
#pragma warning disable CA2235 // Mark all non-serializable fields

    [System.Serializable]
    public class I18NTable
    {
        public List<I18NKV> data;
    }
#pragma warning restore CA2235 // Mark all non-serializable fields

}

