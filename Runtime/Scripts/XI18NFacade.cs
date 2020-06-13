﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatLib;

namespace TinaX.I18N
{
    public class XI18N : Facade<II18N>
    {
        public static II18N Instance => XI18N.That;
        public static II18N I => XI18N.That;
    }
}
