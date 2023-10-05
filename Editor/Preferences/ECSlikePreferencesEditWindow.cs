using UnityEditor;
using UnityEngine;

namespace ECSlike
{
    public class ECSlikePreferencesEditWindow : EditorWindow
    {
        [MenuItem("ECSlike/Preferences", false, 1100)]
        public static void Open()
        {
            GetWindow<ECSlikePreferencesEditWindow>("ECSlike Preferences").open();
        }

        private ECSlikePreferences m_preferences = null;

        public void open()
        {
            m_preferences = ECSlikePreferences.Load();
            setSize(600, 200);
            Show();
        }

        public void setSize(float width, float height)
        {
            Rect fullRect = EditorGUIUtility.GetMainWindowPosition();
            Rect rect = position;
            rect.x = rect.x + fullRect.width * 0.5f - width * 0.5f;
            rect.y = rect.y + fullRect.height * 0.5f - height * 0.5f;
            rect.width = width;
            rect.height = height;
            position = rect;
        }

        private void OnGUI()
        {
            if (m_preferences == null) return;
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save", GUI.skin.button, GUILayout.Width(50f)))
                {
                    if (ECSlikePreferences.Save())
                    {
                        ShowNotification(new GUIContent("Save Success"));
                    }
                    else
                    {
                        ShowNotification(new GUIContent("Save Fail"));
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("World Name", GUILayout.Width(EditorGUIUtility.labelWidth));
                m_preferences.worldName = EditorGUILayout.TextField(m_preferences.worldName);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Generate Output", GUILayout.Width(EditorGUIUtility.labelWidth));
                m_preferences.generateOutput = EditorGUILayout.TextField(m_preferences.generateOutput);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
