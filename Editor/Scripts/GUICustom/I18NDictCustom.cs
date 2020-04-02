using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX;
using TinaX.I18N.Internal;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TinaXEditor.I18N.Internal
{
    [CustomEditor(typeof(I18NDict), true)]
    public class I18NDictCustom : Editor
    {
        ReorderableList mList;
        public override void OnInspectorGUI()
        {
            if(mList == null)
            {
                mList = new ReorderableList(this.serializedObject,
                                this.serializedObject.FindProperty("data"),
                                true,   //draggable
                                true,   //display head
                                true,   //add button
                                true);
                mList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    SerializedProperty item_data = mList.serializedProperty.GetArrayElementAtIndex(index);
                    SerializedProperty item_key = item_data.FindPropertyRelative("k");
                    SerializedProperty item_value = item_data.FindPropertyRelative("v");

                    var rect_key_title = rect;
                    rect_key_title.width = 35;
                    EditorGUI.LabelField(rect_key_title, new GUIContent("key"));

                    var rect_key = rect;
                    rect_key.width -= 40;
                    rect_key.x += 40;
                    EditorGUI.PropertyField(rect_key, item_key, GUIContent.none);

                    var rect_value_title = rect;
                    rect_value_title.width = 35;
                    rect_value_title.y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.LabelField(rect_value_title, new GUIContent("value"));

                    var rect_value = rect;
                    rect_value.width -= 40;
                    rect_value.x += 40;
                    rect_value.y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.PropertyField(rect_value, item_value, GUIContent.none);

                };
                mList.elementHeightCallback = (index) =>
                {
                    return (EditorGUIUtility.singleLineHeight + 2) * 2 + 5;
                };
                mList.drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "I18N Dict - " + target.name);
                };
            }
            mList.DoLayoutList();
            this.serializedObject.ApplyModifiedProperties();

            if(GUILayout.Button("Editor In Window"))
            {
                TinaXEditor.Utils.InspectorInWindow.ShowInspector(this.target);
            }
            //base.OnInspectorGUI();
        }
    }
}
