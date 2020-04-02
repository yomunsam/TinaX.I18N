using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TinaX;
using TinaX.I18N;
using TinaX.I18N.Internal;
using TinaX.I18N.Const;
using UnityEditor;
using UnityEngine;

namespace TinaXEditor.I18N
{
    public static class I18NEditorManager
    {
        private static I18NEditorDataCache _cache;
        private static I18NEditorDataCache cache
        {
            get
            {
                if (_cache == null)
                    _cache = ScriptableSingleton<I18NEditorDataCache>.instance;
                return _cache;
            }
        }


        public static string EditorCurrentRegionName { get { return cache.Editor_CurRegion; } }

        public static void RefreshData()
        {
            cache.Config = XConfig.GetConfig<I18NConfig>(I18NConst.ConfigPath_Resources, AssetLoadType.Resources, false);
            if (cache.Config == null)
                return;

            cache.RegionNames.Clear();
            if(cache.Config.Regions != null && cache.Config.Regions.Count > 0)
            {
                foreach(var item in cache.Config.Regions)
                {
                    cache.RegionNames.Add(item.Name);
                }

                if (!cache.Editor_CurRegion.IsNullOrEmpty())
                {
                    if(!cache.Config.Regions.Any(r=> r.Name.ToLower() == cache.Editor_CurRegion.ToLower()))
                    {
                        cache.Editor_CurRegion = string.Empty;
                    }
                }

                if (cache.Editor_CurRegion.IsNullOrEmpty())
                {
                    //检查config里的default
                    if (!cache.Config.DefaultRegion.IsNullOrEmpty())
                    {
                        var regions = cache.Config.Regions.Where(r => r.Name.ToLower() == cache.Config.DefaultRegion.ToLower());
                        if(regions.Count() > 0)
                        {
                            cache.Editor_CurRegion = regions.First().Name;
                        }
                    }

                    if (cache.Editor_CurRegion.IsNullOrEmpty())
                    {
                        foreach(var item in cache.Config.Regions)
                        {
                            if (item.BindLanguage == null || item.BindLanguage.Count == 0)
                                continue;

                            bool flag = false;
                            foreach(var lang in item.BindLanguage)
                            {
                                if(lang == Application.systemLanguage)
                                {
                                    flag = true;
                                    cache.Editor_CurRegion = item.Name;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }

                    if (cache.Editor_CurRegion.IsNullOrEmpty())
                    {
                        cache.Editor_CurRegion = cache.Config.Regions.First().Name;
                    }
                }

                //--------------
                if (!cache.Editor_CurRegion.IsNullOrEmpty())
                    RefreshDataByRegionName(cache.Editor_CurRegion);
            }
        }

        public static void RefreshDataByRegionName(string regionName)
        {
            if (cache.Config == null)
                cache.Config = XConfig.GetConfig<I18NConfig>(I18NConst.ConfigPath_Resources, AssetLoadType.Resources, false);
            if (cache.Config == null)
                return;
            if (cache.Config.Regions == null || cache.Config.Regions.Count == 0) return;

            var regions = cache.Config.Regions.Where(r => r.Name.ToLower() == regionName.ToLower());
            if (regions.Count() == 0) return;

            cache.mDicts.Clear();
            var region = regions.First();
            if(region.JsonDicts != null && region.JsonDicts.Count > 0)
            {
                foreach (var item in region.JsonDicts)
                {
                    if (item.EditorLoadPath.IsNullOrEmpty() || item.LoadPath.IsNullOrEmpty())
                        continue;
                    var ta = AssetDatabase.LoadAssetAtPath<TextAsset>(item.EditorLoadPath);
                    if (ta == null) continue;
                    if (ta.text.IsNullOrEmpty()) continue;
                    try
                    {
                        var json_obj = JsonUtility.FromJson<I18NTable>(ta.text);
                        if (json_obj != null && json_obj.data != null && json_obj.data.Count > 0)
                        {
                            string groupName = item.GroupName;
                            if (groupName.IsNullOrEmpty())
                                groupName = I18NConst.DefaultGroupName;
                            foreach (var kv in json_obj.data)
                            {
                                if (!cache.mDicts.ContainsKey(groupName))
                                    cache.mDicts.Add(groupName, new Dictionary<string, string>());
                                if (!cache.mDicts[groupName].ContainsKey(kv.k))
                                {
                                    if (item.Base64Value)
                                    {
                                        try
                                        {
                                            string _value = kv.v.DecodeBase64();
                                            cache.mDicts[groupName].Add(kv.k, _value);
                                        }
                                        catch
                                        {
                                            Debug.LogError($"[TinaX.I18N] Decode base64 error: Json: {item.EditorLoadPath}   | Key: {kv.k}");
                                        }
                                    }
                                    else
                                    {
                                        cache.mDicts[groupName].Add(kv.k, kv.v);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if(region.AssetDicts != null && region.AssetDicts.Count > 0)
            {
                foreach (var item in region.AssetDicts)
                {
                    if (item.Asset == null || item.Asset.data == null || item.Asset.data.Count == 0)
                        continue;
                    string groupName = item.GroupName;
                    if (groupName.IsNullOrEmpty())
                        groupName = I18NConst.DefaultGroupName;
                    foreach(var kv in item.Asset.data)
                    {
                        if (!cache.mDicts.ContainsKey(groupName))
                            cache.mDicts.Add(groupName, new Dictionary<string, string>());
                        if (!cache.mDicts[groupName].ContainsKey(kv.k))
                            cache.mDicts[groupName].Add(kv.k, kv.v);
                    }
                }
            }

            cache.Editor_CurRegion = region.Name;

            // Debug used.
            //foreach(var item in cache.mDicts)
            //{
            //    item.Key.LogConsole();
            //    foreach(var kv in item.Value)
            //    {
            //        ($"    {kv.Key} | {kv.Value}").LogConsole();
            //    }
            //}
        }

        public static string[] GetRegionNames()
        {
            return cache.RegionNames.ToArray();
        }

        public static Dictionary<string,Dictionary<string,string>> SearchKeyOrValue(string content)
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            foreach(var item in cache.mDicts)
            {
                foreach(var kv in item.Value)
                {
                    if (kv.Key.IndexOf(content, StringComparison.OrdinalIgnoreCase) < 0 && kv.Value.IndexOf(content, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                    if (!result.ContainsKey(item.Key))
                        result.Add(item.Key, new Dictionary<string, string>());
                    if (!result[item.Key].ContainsKey(kv.Key))
                        result[item.Key].Add(kv.Key, kv.Value);
                }
            }
            return result;
        }

    }

    public class I18NEditorDataCache : ScriptableSingleton<I18NEditorDataCache>
    {
        /// <summary>
        /// 这个类型好像没法序列化，没法持久存储
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> mDicts = new Dictionary<string, Dictionary<string, string>>();
        public I18NConfig Config;
        public string Editor_CurRegion;
        public List<string> RegionNames = new List<string>();

    }

}
