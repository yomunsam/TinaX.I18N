using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TinaX.Systems;
using TinaX;
using TinaX.I18N.Const;
using TinaX.Const;

namespace TinaX.I18N.Components
{
    [RequireComponent(typeof(Text))]
    [DisallowMultipleComponent]
    [AddComponentMenu("TinaX/I18N/Text Localized")]
    public class TextLocalized : MonoBehaviour
    {
        public string I18NKey;
        public string I18NGroup = I18NConst.DefaultGroupName;

        private IEventTicket mRegionTick;

        private void Awake()
        {
            mRegionTick = XEvent.Register(I18NEventConst.OnUseRegion, OnSetRegion, FrameworkConst.FrameworkEventGroupName);
            RefreshTextValue();
        }

        private void OnDestroy()
        {
            if (mRegionTick != null)
                mRegionTick.Unregister();
            mRegionTick = null;
        }

        private void OnSetRegion(object obj)
        {
            RefreshTextValue();
        }

        private void RefreshTextValue()
        {
            if (I18NGroup.IsNullOrEmpty())
                I18NGroup = I18NConst.DefaultGroupName;
            if (I18NKey.IsNullOrEmpty())
                return;
            if (XCore.MainInstance == null) return;
            if (!XCore.MainInstance.IsRunning) return;
            var result = XCore.MainInstance.GetService<II18N>().GetText(this.I18NKey, this.I18NGroup, string.Empty);
            if (!result.IsNullOrEmpty())
            {
                this.GetComponent<Text>().text = result;
            }
        }

        
    }
}

