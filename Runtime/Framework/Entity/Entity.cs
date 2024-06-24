namespace ECSlike
{
    public struct Entity
    {
        public static readonly Entity Null = new Entity();

        public int id;
        public World world;

        public bool isNull()
        {
            return id == 0;
        }

        public void addComponent(IComponentConfig config)
        {
            world.addComponent(id, config);
        }

        public T addComponent<T>() where T : IComponent, new()
        {
            return world.addComponent<T>(id);
        }

        public T getComponent<T>() where T : IComponent
        {
            return world.getComponent<T>(id);
        }

        public T getOrAddComponent<T>() where T : IComponent, new()
        {
            return world.getOrAddComponent<T>(id);
        }

        public bool removeComponent<T>() where T : IComponent
        {
            return world.removeComponent<T>(id);
        }

        public void destroy()
        {
            if (id > 0 && world != null)
            {
                world.destroyEntity(id);
            }
            id = 0;
            world = null;
        }
    }
}
