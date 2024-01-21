namespace ECSlike
{
    public class ConfigFieldAttribute : System.Attribute
    {

    }

    public class InitFieldAttribute : System.Attribute
    {
        public string content = string.Empty;
    }

    public class NonConfigClassAttribute : System.Attribute
    {

    }
}
