using TinaX;
using TinaX.I18N;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace TinaXEditor.I18N.Internal
{
    [InitializeOnLoad]
    class I18NEditorExtension
    {
        static Texture m_icon_earth;

        static I18NEditorExtension()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

		static void OnToolbarGUI()
		{
            if (m_icon_earth == null)
                m_icon_earth = AssetDatabase.LoadAssetAtPath<Texture>("Packages/io.nekonya.tinax.i18n/Editor/Res/Icon/earth.png");

            GUILayout.FlexibleSpace();
            if(GUILayout.Button(new GUIContent(m_icon_earth, "TinaX I18N"), EditorStyles.toolbarButton, GUILayout.MaxWidth(25), GUILayout.MaxHeight(20)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Refresh Data"), false, () =>
                {
                    I18NEditorManager.RefreshData();
                });
                menu.ShowAsContext();
            }
            if (!I18NEditorManager.EditorCurrentRegionName.IsNullOrEmpty())
            {
                if(GUILayout.Button(I18NEditorManager.EditorCurrentRegionName, EditorStyles.toolbarPopup))
                {
                    string cur_name = I18NEditorManager.EditorCurrentRegionName;
                    string[] region_names = I18NEditorManager.GetRegionNames();
                    GenericMenu _menu = new GenericMenu();
                    foreach(var item in region_names)
                    {
                        string name = item;
                        _menu.AddItem(new GUIContent(name),
                            (name == cur_name),
                            () => 
                            { 
                                if(name != cur_name)
                                {
                                    I18NEditorManager.RefreshDataByRegionName(name);
                                    if (Application.isPlaying)
                                    {
                                        if(XCore.MainInstance != null && XCore.MainInstance.IsRunning)
                                        {
                                            XCore.MainInstance.GetService<II18N>().UseRegionAsync(name, e =>
                                            {
                                                if (e != null)
                                                    throw e;
                                            });
                                        }
                                    }
                                }
                            });
                    }
                    _menu.ShowAsContext();
                }

            }
        }
	}
}
