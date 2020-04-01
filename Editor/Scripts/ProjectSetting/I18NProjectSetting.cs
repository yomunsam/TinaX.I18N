using System.Collections;
using System.Collections.Generic;
using TinaX;
using TinaX.I18N.Const;
using TinaX.I18N.Internal;
using TinaXEditor.I18N.Const;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.IO;

namespace TinaXEditor.I18N.Internal
{
    public class I18NProjectSetting
    {
        private static bool mDataRefreshed = false;
        private static I18NConfig mConfig;
        private static SerializedObject mConfig_SerObj;

        private static ReorderableList mList_Regions;

        [SettingsProvider]
        public static SettingsProvider XI18NSetting()
        {
            return new SettingsProvider(I18NEditorConst.ProjectSetting_Node, SettingsScope.Project, new string[] { "Nekonya", "TinaX", "I18N", "TinaX.I18N", "Localization" })
            {
                label = "X I18N",
                guiHandler = (searchContent) => 
                {
                    if (!mDataRefreshed) refreshData();
                    EditorGUILayout.BeginVertical(Styles.body);
                    if (mConfig == null)
                    {
                        GUILayout.Space(20);
                        GUILayout.Label(I18Ns.NoConfig);
                        if (GUILayout.Button(I18Ns.BtnCreateConfigFile, Styles.style_btn_normal, GUILayout.MaxWidth(120)))
                        {
                            mConfig = XConfig.CreateConfigIfNotExists<I18NConfig>(I18NConst.ConfigPath_Resources, AssetLoadType.Resources);
                            refreshData();
                        }
                    }
                    else
                    {
                        if(mList_Regions == null)
                        {
                            mList_Regions = new ReorderableList(mConfig_SerObj,
                                mConfig_SerObj.FindProperty("Regions"),
                                true,   //draggable
                                true,   //display head
                                true,   //add button
                                true);  //remove button
                            mList_Regions.drawElementCallback = (rect, index, isActive, isFocused) =>
                            {
                                rect.y += 2;
                                rect.height = EditorGUIUtility.singleLineHeight;
                                var singleLine = EditorGUIUtility.singleLineHeight + 2;
                                float base_line = 0;

                                SerializedProperty itemData = mList_Regions.serializedProperty.GetArrayElementAtIndex(index);
                                SerializedProperty item_name = itemData.FindPropertyRelative("Name");
                                SerializedProperty item_bind_language = itemData.FindPropertyRelative("BindLanguage");
                                SerializedProperty item_json_dict = itemData.FindPropertyRelative("JsonDicts");
                                SerializedProperty item_asset_dict = itemData.FindPropertyRelative("AssetDicts");

                                //line 1  region name;
                                var rect_regionName = rect;
                                rect_regionName.width = 320;
                                EditorGUI.PropertyField(rect_regionName, item_name, new GUIContent(I18Ns.RegionName));
                                base_line += 1;

                                //line 2 Bind Language
                                var rect_language_title = rect;
                                rect_language_title.y += singleLine;
                                rect_language_title.width = 125;
                                EditorGUI.LabelField(rect_language_title, new GUIContent(I18Ns.BindLanaguage, I18Ns.BindLanaguage_Tip));

                                if (item_bind_language.arraySize > 0)
                                {
                                    for(var i = 0; i < item_bind_language.arraySize; i++)
                                    {
                                        var _rect = rect;
                                        _rect.y += singleLine * (i + 1);
                                        _rect.x += 130;

                                        var _rect_content = _rect;
                                        _rect_content.width = 180;

                                        SerializedProperty item_language = item_bind_language.GetArrayElementAtIndex(i);
                                        EditorGUI.PropertyField(_rect_content, item_language, GUIContent.none);

                                        var _rect_del_btn = _rect;
                                        _rect_del_btn.width = 25;
                                        _rect_del_btn.x += _rect_content.width + 5;
                                        if(GUI.Button(_rect_del_btn, new GUIContent("×", "delete")))
                                        {
                                            item_bind_language.DeleteArrayElementAtIndex(i);
                                        }
                                    }
                                }

                                var rect_language_add_btn = rect;
                                rect_language_add_btn.y += singleLine * (item_bind_language.arraySize + 1);
                                rect_language_add_btn.x += 130;
                                rect_language_add_btn.width = 110;
                                if(GUI.Button(rect_language_add_btn, "Add Language"))
                                {
                                    int _index = item_bind_language.arraySize;
                                    item_bind_language.InsertArrayElementAtIndex(_index);
                                    var _item = item_bind_language.GetArrayElementAtIndex(_index);
                                    _item.enumValueIndex = 0;
                                }
                                base_line += item_bind_language.arraySize + 1.5f;

                                //line 3 Json Dicts
                                var rect_json_dict_title = rect;
                                rect_json_dict_title.y += (base_line) * singleLine;
                                rect_json_dict_title.width = 125;
                                EditorGUI.LabelField(rect_json_dict_title, new GUIContent(I18Ns.JsonDict));

                                if(item_json_dict.arraySize > 0)
                                {
                                    var _json_base_dict = rect;
                                    _json_base_dict.y += base_line * singleLine;
                                    for (var i = 0; i < item_json_dict.arraySize; i++)
                                    {
                                        var _rect = _json_base_dict;
                                        _rect.y += (singleLine * 3.5f) * i;
                                        _rect.x += 130;
                                        _rect.width -= 130;

                                        SerializedProperty _item_data = item_json_dict.GetArrayElementAtIndex(i);
                                        SerializedProperty _item_loadPath = _item_data.FindPropertyRelative("LoadPath");
                                        SerializedProperty _item_editorloadPath = _item_data.FindPropertyRelative("EditorLoadPath");
                                        SerializedProperty _item_groupname = _item_data.FindPropertyRelative("GroupName");
                                        SerializedProperty _item_base64 = _item_data.FindPropertyRelative("Base64Value");

                                        //line 1: load path
                                        var _rect_load_path = _rect;
                                        _rect_load_path.width -= 30;
                                        EditorGUI.PropertyField(_rect_load_path, _item_loadPath,new GUIContent(I18Ns.JsonLoadPath, I18Ns.JsonLoadPath_Tips));
                                        //line 2: editor load path;
                                        var _rect_editor_load_path = _rect;
                                        _rect_editor_load_path.y += singleLine;
                                        _rect_editor_load_path.width -= 30;
                                        _rect_editor_load_path.width -= 55;
                                        EditorGUI.PropertyField(_rect_editor_load_path, _item_editorloadPath, new GUIContent(I18Ns.EditorJsonLoadPath, I18Ns.EditorJsonLoadPath_Tips));
                                        var _rect_editor_load_path_btn = _rect;
                                        _rect_editor_load_path_btn.y += singleLine;
                                        _rect_editor_load_path_btn.width = 50;
                                        _rect_editor_load_path_btn.x += _rect_editor_load_path.width + 5;
                                        if(GUI.Button(_rect_editor_load_path_btn, "Select"))
                                        {
                                            var path = EditorUtility.OpenFilePanel("Select Lua Entry File", "Assets/", "");
                                            if (!path.IsNullOrEmpty())
                                            {
                                                var root_path = Directory.GetCurrentDirectory().Replace("\\", "/");
                                                if (path.StartsWith(root_path))
                                                {
                                                    path = path.Substring(root_path.Length + 1, path.Length - root_path.Length - 1);
                                                    path = path.Replace("\\", "/");
                                                    _item_editorloadPath.stringValue = path;
                                                }
                                                else
                                                    Debug.LogError("Invalid Path: " + path);
                                            }
                                        }
                                        //line 3: group name and base 64
                                        var _rect_groupName = _rect;
                                        _rect_groupName.y += singleLine * 2;
                                        _rect_groupName.width -= 30;
                                        _rect_groupName.width -= 125;
                                        EditorGUI.PropertyField(_rect_groupName, _item_groupname, new GUIContent("GroupName"));
                                        var _rect_base64 = _rect;
                                        _rect_base64.y += singleLine * 2;
                                        _rect_base64.x += _rect_groupName.width + 5;
                                        _rect_base64.width = 120;
                                        _item_base64.boolValue = EditorGUI.ToggleLeft(_rect_base64, new GUIContent("Value Is Base64",I18Ns.JsonB64_Tips), _item_base64.boolValue);
                                        //delete
                                        var _rect_del_btn = _rect;
                                        _rect_del_btn.width = 20;
                                        _rect_del_btn.x += _rect.width - 25;
                                        _rect_del_btn.height += singleLine * 2;
                                        if(GUI.Button(_rect_del_btn, new GUIContent("×", "Delete")))
                                        {
                                            item_json_dict.DeleteArrayElementAtIndex(i);
                                        }
                                    }
                                }
                                var rect_json_add_btn = rect;
                                rect_json_add_btn.y += (base_line * singleLine) + ((singleLine * 3.5f) * item_json_dict.arraySize);
                                rect_json_add_btn.x += 130;
                                rect_json_add_btn.width = 110;
                                if (GUI.Button(rect_json_add_btn, "Add Json Dict"))
                                {
                                    int _index = item_json_dict.arraySize;
                                    item_json_dict.InsertArrayElementAtIndex(_index);
                                    var _item = item_json_dict.GetArrayElementAtIndex(_index);
                                    var _item_loadPath = _item.FindPropertyRelative("LoadPath");
                                    var _item_base64 = _item.FindPropertyRelative("Base64Value");
                                    var _item_groupName = _item.FindPropertyRelative("GroupName");
                                    var _item_editorLoadPath = _item.FindPropertyRelative("EditorLoadPath");

                                    _item_loadPath.stringValue = string.Empty;
                                    _item_editorLoadPath.stringValue = string.Empty;
                                    _item_base64.boolValue = false;
                                    _item_groupName.stringValue = I18NConst.DefaultGroupName;
                                }
                                base_line += (item_json_dict.arraySize * 3.5f) + 1.5f;

                                //line 4 Asset Dicts
                                var rect_asset_dict_title = rect;
                                rect_asset_dict_title.y += (base_line) * singleLine;
                                rect_asset_dict_title.width = 125;
                                EditorGUI.LabelField(rect_asset_dict_title, new GUIContent("Asset Dict"));

                                if(item_asset_dict.arraySize > 0)
                                {
                                    var _rect_base = rect;
                                    _rect_base.y += base_line * singleLine;
                                    for(var i = 0; i < item_asset_dict.arraySize; i++)
                                    {
                                        var _rect = _rect_base;
                                        _rect.x += 130;
                                        _rect.y += singleLine * i;
                                        _rect.width -= 130;

                                        SerializedProperty _item_data = item_asset_dict.GetArrayElementAtIndex(i);
                                        SerializedProperty _item_asset = _item_data.FindPropertyRelative("Asset");
                                        SerializedProperty _item_groupName = _item_data.FindPropertyRelative("GroupName");

                                        var _rect_groupName_title = _rect;
                                        _rect_groupName_title.width = 85;
                                        EditorGUI.LabelField(_rect_groupName_title, new GUIContent("GroupName"));

                                        var _rect_groupName = _rect;
                                        _rect_groupName.x += 90;
                                        _rect_groupName.width = 135;
                                        EditorGUI.PropertyField(_rect_groupName, _item_groupName, GUIContent.none);

                                        var _rect_asset = _rect;
                                        _rect_asset.x += 90 + 135+ 5;
                                        _rect_asset.width = 160;
                                        EditorGUI.PropertyField(_rect_asset, _item_asset, GUIContent.none);

                                        var _rect_btn_del = _rect;
                                        _rect_btn_del.x += 90 + 135 + 5 + 160 + 5;
                                        _rect_btn_del.width = 25;
                                        if(GUI.Button(_rect_btn_del, new GUIContent("×", "Delete")))
                                        {
                                            item_asset_dict.DeleteArrayElementAtIndex(i);
                                        }

                                    }
                                }

                                var rect_asset_btn_add = rect;
                                rect_asset_btn_add.y += (base_line + item_asset_dict.arraySize) * singleLine;
                                rect_asset_btn_add.x += 130;
                                rect_asset_btn_add.width = 110;
                                if(GUI.Button(rect_asset_btn_add,"Add Asset Dict"))
                                {
                                    int _index = item_asset_dict.arraySize;
                                    item_asset_dict.InsertArrayElementAtIndex(_index);
                                    var _item = item_asset_dict.GetArrayElementAtIndex(_index);
                                    var _item_asset = _item.FindPropertyRelative("Asset");
                                    var _item_group = _item.FindPropertyRelative("GroupName");

                                    _item_asset.objectReferenceValue = null;
                                    _item_group.stringValue = I18NConst.DefaultGroupName;
                                }
                            };
                            mList_Regions.elementHeightCallback = (index) =>
                            {
                                SerializedProperty itemData = mList_Regions.serializedProperty.GetArrayElementAtIndex(index);
                                SerializedProperty item_bind_language = itemData.FindPropertyRelative("BindLanguage");
                                SerializedProperty item_json_dict = itemData.FindPropertyRelative("JsonDicts");
                                SerializedProperty item_asset_dict = itemData.FindPropertyRelative("AssetDicts");
                                float line = 1;
                                //绑定语言
                                line += item_bind_language.arraySize + 1.5f;
                                //全局Json字典
                                line += (item_json_dict.arraySize * 3.5f) + 1.5f;
                                //Asset字典
                                line += (item_asset_dict.arraySize) +1;

                                return (EditorGUIUtility.singleLineHeight + 2) * line + 2;
                            };
                            mList_Regions.onAddCallback = (list) =>
                            {
                                if (list.serializedProperty != null)
                                {
                                    list.serializedProperty.arraySize++;
                                    list.index = list.serializedProperty.arraySize - 1;

                                    SerializedProperty itemData = list.serializedProperty.GetArrayElementAtIndex(list.index);
                                    SerializedProperty item_name = itemData.FindPropertyRelative("Name");
                                    SerializedProperty item_bind_lan = itemData.FindPropertyRelative("BindLanguage");
                                    SerializedProperty item_json = itemData.FindPropertyRelative("JsonDicts");
                                    SerializedProperty item_asset = itemData.FindPropertyRelative("AssetDicts");
                                    item_name.stringValue = string.Empty;
                                    item_bind_lan.ClearArray();
                                    item_json.ClearArray();
                                    item_asset.ClearArray();

                                }
                                else
                                {
                                    ReorderableList.defaultBehaviours.DoAddButton(list);
                                }
                            };
                            mList_Regions.drawHeaderCallback = (rect) =>
                            {
                                EditorGUI.LabelField(rect, I18Ns.RegionList);
                            };
                        }

                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(mConfig_SerObj.FindProperty("EnableI18N"));
                        GUILayout.Space(10);
                        mList_Regions.DoLayoutList();

                        if (mConfig_SerObj != null)
                            mConfig_SerObj.ApplyModifiedProperties();
                    }
                    EditorGUILayout.EndVertical();
                },
                deactivateHandler = () =>
                {
                    if (mConfig != null)
                    {
                        EditorUtility.SetDirty(mConfig);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                },
            };
        }

        private static void refreshData()
        {
            mConfig = XConfig.GetConfig<I18NConfig>(I18NConst.ConfigPath_Resources, AssetLoadType.Resources, false);
            if (mConfig != null)
                mConfig_SerObj = new SerializedObject(mConfig);

            mDataRefreshed = true;
        }



        private static class Styles
        {
            private static GUIStyle _style_btn_normal; //字体比原版稍微大一号
            public static GUIStyle style_btn_normal
            {
                get
                {
                    if (_style_btn_normal == null)
                    {
                        _style_btn_normal = new GUIStyle(GUI.skin.button);
                        _style_btn_normal.fontSize = 13;
                    }
                    return _style_btn_normal;
                }
            }

            private static GUIStyle _body;
            public static GUIStyle body
            {
                get
                {
                    if (_body == null)
                    {
                        _body = new GUIStyle();
                        _body.padding.left = 15;
                        _body.padding.right = 15;
                    }
                    return _body;
                }
            }
        }

        private static class I18Ns
        {
            private static bool? _isChinese;
            private static bool IsChinese
            {
                get
                {
                    if (_isChinese == null)
                    {
                        _isChinese = (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified);
                    }
                    return _isChinese.Value;
                }
            }

            private static bool? _nihongo_desuka;
            private static bool NihongoDesuka
            {
                get
                {
                    if (_nihongo_desuka == null)
                        _nihongo_desuka = (Application.systemLanguage == SystemLanguage.Japanese);
                    return _nihongo_desuka.Value;
                }
            }

            public static string NoConfig
            {
                get
                {
                    if (IsChinese)
                        return "在首次使用TinaX I18N 的设置工具前，请先创建配置文件";
                    if (NihongoDesuka)
                        return "TinaX I18N セットアップツールを初めて使用する前に、構成ファイルを作成してください";
                    return "Before using the TinaX I18N setup tool for the first time, please create a configuration file";
                }
            }

            public static string BtnCreateConfigFile
            {
                get
                {
                    if (IsChinese)
                        return "创建配置文件";
                    if (NihongoDesuka)
                        return "構成ファイルを作成する";
                    return "Create Configure File";
                }
            }

            public static string RegionList
            {
                get
                {
                    if (IsChinese)
                        return "地区配置";
                    if (NihongoDesuka)
                        return "地域構成";
                    return "Regional configuration";
                }
            }

            public static string RegionName
            {
                get
                {
                    if (IsChinese)
                        return "地区名";
                    if (NihongoDesuka)
                        return "地域名";
                    return "Region Name";
                }
            }

            public static string BindLanaguage
            {
                get
                {
                    if (IsChinese)
                        return "绑定语言";
                    if (NihongoDesuka)
                        return "バインディング言語";
                    return "Binding language";
                }
            }

            public static string BindLanaguage_Tip
            {
                get
                {
                    if (IsChinese)
                        return "用于根据系统语言自动匹配区域";
                    if (NihongoDesuka)
                        return "システム言語に応じた領域の自動マッチング";
                    return "Used to automatically match regions based on system language";
                }
            }

            public static string JsonDict
            {
                get
                {
                    if (IsChinese)
                        return "全局 Json字典";
                    if (NihongoDesuka)
                        return "グローバルJson辞書";
                    return "Global Json Dictionary";
                }
            }

            public static string JsonLoadPath
            {
                get
                {
                    if (IsChinese)
                        return "Json加载路径";
                    if (NihongoDesuka)
                        return "Jsonの読み込みパス";
                    return "Json Load Path";
                }
            }

            public static string JsonLoadPath_Tips
            {
                get
                {
                    if (IsChinese)
                        return "请输入可使用TinaX内置资产管理接口\"TinaX.Services.IAssetService\"加载到的地址。";
                    if (NihongoDesuka)
                        return "TinaXの内蔵資産管理インターフェース「TinaX.Services.IAssetServiceを利用して読み込めるアドレスを入力してください。";
                    return "Please enter an address that can be loaded using TinaX's built-in asset management interface \"TinaX.Services.IAssetService\".";
                }
            }

            public static string EditorJsonLoadPath
            {
                get
                {
                    if (IsChinese)
                        return "Json编辑器加载路径";
                    return "Json Load Path In Editor";
                }
            }

            public static string EditorJsonLoadPath_Tips
            {
                get
                {
                    if (IsChinese)
                        return "请输入可使用\"UnityEditor.AssetDatabase\"加载到的地址。";
                    if (NihongoDesuka)
                        return "「UnityEditor.AssetDatabase」で読み込めるアドレスを入力してください。";
                    return "Please enter an address that can be loaded using \"UnityEditor.AssetDatabase\".";
                }
            }

            public static string JsonB64_Tips
            {
                get
                {
                    if (IsChinese)
                        return "勾选后，读取该Json文件时将自动将Value进行Base64解码。";
                    if (NihongoDesuka)
                        return "チェックを入れると、このJsonファイルを読み込む際に自動的にBase64でデコードされます。";
                    return "When checked, Value will be Base64 decoded automatically when reading the Json file.";
                }
            }

        }
    }
}

