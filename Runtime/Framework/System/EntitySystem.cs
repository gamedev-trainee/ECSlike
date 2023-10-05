namespace ECSlike
{
    public abstract class EntitySystem : AbstractSystem
    {
        public static readonly int EntityCapacity = 32;

        private bool m_inited = false;
        private System.Type[] m_wantedTypes = null;
        private EntityList m_entityList = null;

        protected void init(World world)
        {
            if (m_inited) return;
            m_inited = true;
            m_wantedTypes = getWantedComponentTypes();
            m_entityList = world.allocateEntityList(m_wantedTypes);
        }

        public sealed override void update(World world)
        {
            init(world);
            m_entityList.eachEntity(onUpdateEntity);
        }

        protected abstract System.Type[] getWantedComponentTypes();

        protected abstract void onUpdateEntity(IComponent[] components);
    }
}
