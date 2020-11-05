﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinaX.I18N.Const;
using UnityEngine;

namespace TinaX.I18N
{
    public interface II18N
    {
        string GetText(string key, string groupName = I18NConst.DefaultGroupName, string defaultText = null);
        Task UseRegionAsync(string regionName);
        void UseRegionAsync(string regionName, Action<XException> callback);
    }
}

