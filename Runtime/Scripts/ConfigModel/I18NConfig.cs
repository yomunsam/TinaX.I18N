using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TinaX.I18N.Internal
{
    public class I18NConfig : ScriptableObject
    {
        public bool EnableI18N = true;
        public List<XRegion> Regions;
    }
}
