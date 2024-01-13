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
                if (GUILayout.Button("+", GUI.skin.button, GUILayout.Width(30f)))
                {
                    m_preferences.worlds.Add(new ECSlikeWorldInfo());
                }
                EditorGUILayout.Space();
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
            GUILayout.Space(4f);
            int count = m_preferences.worlds.Count;
            for (int i = 0; i < count; i++)
            {
                GUILayout.Box("", GUI.skin.textArea, GUILayout.Height(1f), GUILayout.ExpandWidth(true));
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(20), GUILayout.Height(40f));
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("World Name", GUILayout.Width(EditorGUIUtility.labelWidth));
                            m_preferences.worlds[i].worldName = EditorGUILayout.TextField(m_preferences.worlds[i].worldName);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Input", GUILayout.Width(EditorGUIUtility.labelWidth));
                            m_preferences.worlds[i].input = EditorGUILayout.TextField(m_preferences.worlds[i].input);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Output", GUILayout.Width(EditorGUIUtility.labelWidth));
                            m_preferences.worlds[i].output = EditorGUILayout.TextField(m_preferences.worlds[i].output);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                    if (i == 0)
                    {
                        EditorGUILayout.LabelField("默认", GUILayout.Width(50f), GUILayout.Height(40f));
                    }
                    else
                    {
                        if (GUILayout.Button("X", GUI.skin.button, GUILayout.Width(50f), GUILayout.Height(40f)))
                        {
                            m_preferences.worlds.RemoveAt(i);
                            EditorGUILayout.EndHorizontal();
                            break;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Box("", GUI.skin.textArea, GUILayout.Height(1f), GUILayout.ExpandWidth(true));
        }
    }
}
