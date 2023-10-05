using UnityEngine;

namespace ECSlike
{
    [System.Serializable]
    public class ECSlikePreferences
    {
        public const string PATH = "Library/ECSlike/preferences.json";

        private static ECSlikePreferences ms_instance = null;

        public static ECSlikePreferences Load()
        {
            if (ms_instance == null)
            {
                if (System.IO.File.Exists(PATH))
                {
                    string content = System.IO.File.ReadAllText(PATH);
                    ms_instance = JsonUtility.FromJson<ECSlikePreferences>(content);
                }
                else
                {
                    ms_instance = new ECSlikePreferences();
                    string dir = System.IO.Path.GetDirectoryName(PATH);
                    if (!System.IO.Directory.Exists(dir))
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }

                }
            }
            return ms_instance;
        }

        public static bool Save()
        {
            if (ms_instance != null)
            {
                string content = JsonUtility.ToJson(ms_instance);
                System.IO.File.WriteAllText(PATH, content);
                return true;
            }
            return false;
        }

        public string worldName = "ECSWorld";
        public string generateOutput = "Assets/Scripts/ECS/Generated";
    }
}
