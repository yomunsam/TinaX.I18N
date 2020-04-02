using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.I18N;
using TinaX.I18N.Const;
using TinaX.Services;
using UnityEngine;
using UniRx;

namespace TinaX.I18N.Internal
{
    public class I18NManager : II18N, II18NInternal
    {
        [CatLib.Container.Inject]
        public IAssetService mAssets { get; set; }

        private I18NConfig mConfig;
        private bool mInited = false;

        private string mRegionName = string.Empty;

        /// <summary>
        /// Global dict 
        /// [Group Name] -> [Key] -> Value;
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> mDicts = new Dictionary<string, Dictionary<string, string>>();

        private XException mStartException;
        public XException GetStartException() => mStartException;

        public async Task<bool> Start()
        {
            if (mInited) return true;
            mConfig = XConfig.GetConfig<I18NConfig>(I18NConst.ConfigPath_Resources, AssetLoadType.Resources);
            if (mConfig == null) 
            {
                mStartException = new XException("[TinaX.I18N]Load I18N config file failed. please cheak in ProjectSettings window.");
                return false;
            }
            if (!mConfig.EnableI18N) return true;
            if(mAssets == null)
            {
                mStartException = new XException("[TinaX.I18N]No service implements the built-in asset loading interface in Framework");
                return false;
            }

            if(mConfig.Regions != null && mConfig.Regions.Count > 0)
            {
                bool check_default = !mConfig.DefaultRegion.IsNullOrEmpty();
                string default_region_name = check_default ? mConfig.DefaultRegion.ToLower() : string.Empty;
                XRegion _default_region = null;
                foreach(var item in mConfig.Regions)
                {
                    if (item.Name.IsNullOrEmpty()) continue;

                    bool _flag_1 = false;
                    if(check_default && _default_region == null)
                    {
                        if (item.Name.ToLower().Equals(default_region_name))
                        {
                            _default_region = item;
                            _flag_1 = true;
                        }
                    }

                    if (mConfig.AutomaticMatchingBySystemLanaguage)
                    {
                        bool flag_2 = false;
                        if(item.BindLanguage != null && item.BindLanguage.Count > 0)
                        {
                            foreach(var lang in item.BindLanguage)
                            {
                                if(lang == Application.systemLanguage)
                                {
                                    flag_2 = true;
                                    _default_region = item;
                                    break;
                                }
                            }
                        }
                        if (flag_2)
                            break;
                    }
                    else
                    {
                        if (_flag_1)
                            break;
                    }
                }
                if (_default_region != null)
                {
                    try
                    {
                        await this.useRegion(_default_region);
                    }
                    catch(XException e)
                    {
                        mStartException = e;
                        return false;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }

            //Debug code
            //foreach(var item in mDicts)
            //{
            //    item.Key.LogConsole();
            //    foreach(var kv in item.Value)
            //    {
            //        ("    " + kv.Key + "  |  " + kv.Value).LogConsole();
            //    }
            //}
            
            mInited = true;
            return true;
        }


        public Task UseRegionAsync(string regionName)
        {
            if (!mInited)
                throw new XException("[TinaX.I18N] Cannot \"UseRegion\" before framework started.");
            if(mConfig == null)
                throw new Exception("[TinaX.I18N] I18n config not found. cannot set region");
            if (regionName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(regionName));

            if (mConfig.Regions == null || mConfig.Regions.Count == 0)
            {
                Debug.LogWarning("[TinaX.I18N] None valid region in i18n config.");
                return Task.CompletedTask;
            }
            foreach(var item in mConfig.Regions)
            {
                if (item.Name.ToLower().Equals(regionName.ToLower()))
                {
                    return useRegion(item);
                }
            }
            Debug.LogWarning("[TinaX.I18N] Not found region \"" + regionName + "\" in config.");
            return Task.CompletedTask;
        }
        public void UseRegionAsync(string regionName, Action<XException> callback)
        {
            this.UseRegionAsync(regionName)
                .ToObservable()
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    callback?.Invoke(null);
                }, 
                e =>
                {
                    if (e is XException)
                        callback(e as XException);
                    else
                        Debug.LogException(e);
                });
        }

        public string GetText(string key, string groupName = I18NConst.DefaultGroupName, string defaultText = null)
        {
            if (mDicts.TryGetValue(groupName, out var kv))
            {
                if (kv.TryGetValue(key, out var value))
                    return value;
            }
            return defaultText == null ? key : defaultText;
        }

        private async Task useRegion(XRegion region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            mDicts.Clear();
            if(region.JsonDicts != null && region.JsonDicts.Count > 0)
            {
                List<Task<I18NTable>> load_task = new List<Task<I18NTable>>();
                List<int> load_task_index = new List<int>();
                for(var i = 0; i < region.JsonDicts.Count; i++)
                {
                    if (!region.JsonDicts[i].LoadPath.IsNullOrEmpty())
                    {
                        load_task.Add(this.getJsonDict(region.JsonDicts[i].LoadPath));
                        load_task_index.Add(i);
                    }
                }

                if(load_task.Count > 0)
                {
                    await Task.WhenAll(load_task);
                    for(var i = 0; i < load_task.Count; i++)
                    {
                        foreach(var item in load_task)
                        {
                            var json_obj = item.Result;
                            bool base64_value = region.JsonDicts[load_task_index[i]].Base64Value;
                            string groupName = region.JsonDicts[load_task_index[i]].GroupName;
                            if (groupName.IsNullOrEmpty()) groupName = I18NConst.DefaultGroupName;

                            foreach(var kv in json_obj.data)
                            {
                                if (!mDicts.ContainsKey(groupName))
                                    mDicts.Add(groupName, new Dictionary<string, string>());
                                if (!mDicts[groupName].ContainsKey(kv.k))
                                {
                                    if (base64_value)
                                    {
                                        try
                                        {
                                            string _value = kv.v.DecodeBase64();
                                            mDicts[groupName].Add(kv.k, _value);
                                        }
                                        catch
                                        {
                                            Debug.LogError($"[TinaX.I18N] Decode base64 error: Json: {region.JsonDicts[load_task_index[i]].LoadPath}   | Key: {kv.k}");
                                        }
                                    }
                                    else
                                        mDicts[groupName].Add(kv.k, kv.v);
                                }
                            }
                        }
                    }
                }
            }

            if(region.AssetDicts !=  null && region.AssetDicts.Count > 0)
            {
                foreach(var item in region.AssetDicts)
                {
                    if (item.Asset == null) continue;
                    string groupName = item.GroupName.IsNullOrEmpty() ? I18NConst.DefaultGroupName : item.GroupName;
                    foreach(var kv in item.Asset.data)
                    {
                        if (!mDicts.ContainsKey(groupName))
                            mDicts.Add(groupName, new Dictionary<string, string>());
                        if (!mDicts[groupName].ContainsKey(kv.k))
                            mDicts[groupName].Add(kv.k, kv.v);
                    }
                }
            }

            mRegionName = region.Name;
            XEvent.Call(I18NEventConst.OnUseRegion, region.Name, TinaX.Const.FrameworkConst.FrameworkEventGroupName);
        }

        private async Task<I18NTable> getJsonDict(string loadPath)
        {
            var text_asset = await mAssets.LoadAsync<TextAsset>(loadPath);
            var json_obj = JsonUtility.FromJson<I18NTable>(text_asset.text);
            mAssets.Release(text_asset);
            return json_obj;
        }

    }
}
