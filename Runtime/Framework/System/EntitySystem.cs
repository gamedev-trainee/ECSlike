namespace ECSlike
{
    public abstract class EntitySystem : AbstractSystem
    {
        private System.Type[] m_wantedTypes = null;
        private TypeEntityList m_wantedEntities = null;

        protected override void onInit(World world)
        {
            m_wantedTypes = getWantedTypes();
            m_wantedEntities = world.getTypeEntityList(m_wantedTypes);
        }

        protected sealed override void onUpdate(World world)
        {
            m_wantedEntities.beginLock();
            {
                int entity;
                IComponent[] components;
                int count = m_wantedEntities.getCount();
                for (int i = 0; i < count; i++)
                {
                    m_wantedEntities.getEntityAndComponentsAt(i, out entity, out components);
                    onUpdateEntity(world, entity, components);
                }
            }
            m_wantedEntities.endLock();
        }

        protected abstract System.Type[] getWantedTypes();

        protected abstract void onUpdateEntity(World world, int entity, IComponent[] components);
    }
}
