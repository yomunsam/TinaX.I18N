using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using TinaX.I18N.Components;
using UnityEngine.UI;

namespace TinaXEditor.I18N.Components
{
    [CustomEditor(typeof(TextLocalized))]
    public class TextLocalizedCustom : Editor
    {
        private Texture mSearchIcon;
        private Texture mSelectIcon;
        private bool mShowSearchGUI = false;
        private string mSearchText;
        private Dictionary<string, Dictionary<string, string>> mSearchResult;
        private Vector2 mV2_Result;
        private float max_width = 100;

        public override void OnInspectorGUI()
        {
            //I18N key
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("I18N Key", GUILayout.Width(65));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("I18NKey"),GUIContent.none);
            GUILayout.Button("Refresh", GUILayout.MaxWidth(55));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("I18N Group", GUILayout.Width(65));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("I18NGroup"),GUIContent.none);
            EditorGUILayout.EndHorizontal();

            // 搜索
            GUILayout.Space(10);
            if(!mShowSearchGUI)
            {
                if (GUILayout.Button("Search", GUILayout.MaxWidth(150)))
                {
                    mShowSearchGUI = true;
                }
            }

            if (mShowSearchGUI)
            {
                if (mSearchIcon == null)
                    mSearchIcon = AssetDatabase.LoadAssetAtPath<Texture>("Packages/io.nekonya.tinax.i18n/Editor/Res/Icon/Search.png");
                if (mSelectIcon == null)
                    mSelectIcon = AssetDatabase.LoadAssetAtPath<Texture>("Packages/io.nekonya.tinax.i18n/Editor/Res/Icon/select.png");

                EditorGUILayout.BeginHorizontal();
                mSearchText = EditorGUILayout.TextField(mSearchText, EditorStyles.toolbarSearchField);
                if(GUILayout.Button(new GUIContent(mSearchIcon, "Search"), GUILayout.MaxWidth(25), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight)))
                {
                    mSearchResult = I18NEditorManager.SearchKeyOrValue(mSearchText);
                }
                EditorGUILayout.EndHorizontal();

                if (mSearchResult != null)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if(mSearchResult.Count == 0)
                    {
                        GUILayout.Label("No Result, Please try to refresh data.");
                    }
                    else
                    {
                        var rect = EditorGUILayout.GetControlRect();
                        if(rect.width > 20)
                        {
                            max_width = rect.width;
                        }
                        mV2_Result = EditorGUILayout.BeginScrollView(mV2_Result, GUILayout.MaxHeight(200),GUILayout.MinHeight(40));
                        foreach(var item in mSearchResult)
                        {
                            GUILayout.Label(item.Key, EditorStyles.boldLabel);
                            foreach(var kv in item.Value)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(kv.Key, GUILayout.MaxWidth((max_width - 45) / 2));
                                EditorGUILayout.LabelField("|" + kv.Value, GUILayout.MaxWidth((max_width - 45) / 2));
                                if (GUILayout.Button(new GUIContent(mSelectIcon, "select it"), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight), GUILayout.MaxWidth(25)))
                                {
                                    this.serializedObject.FindProperty("I18NKey").stringValue = kv.Key;
                                    this.serializedObject.FindProperty("I18NGroup").stringValue = item.Key;
                                    var text = ((TextLocalized)target).GetComponent<Text>();
                                    if(text != null)
                                    {
                                        text.text = kv.Value;
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }

                    EditorGUILayout.EndVertical();
                }


            }

            this.serializedObject.ApplyModifiedProperties();
            //base.OnInspectorGUI();
        }
    }
}
