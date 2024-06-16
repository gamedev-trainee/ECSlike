namespace ECSlike
{
    public class ConfigClassAttribute : System.Attribute
    {

    }

    public class ConfigFieldAttribute : System.Attribute
    {
        public string init = string.Empty;
        public string gizmosDrawer = string.Empty;
    }
}
