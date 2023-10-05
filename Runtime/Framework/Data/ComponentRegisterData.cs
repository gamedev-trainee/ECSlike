namespace ECSlike
{
    public struct ComponentRegisterData
    {
        public System.Type componentType;
        public System.Type configType;
        public System.Func<IComponentConfig, IComponent> componentCreator;

        public ComponentRegisterData(System.Type vComponentType, System.Type vConfigType, System.Func<IComponentConfig, IComponent> vComponentCreator)
        {
            componentType = vComponentType;
            configType = vConfigType;
            componentCreator = vComponentCreator;
        }
    }
}
