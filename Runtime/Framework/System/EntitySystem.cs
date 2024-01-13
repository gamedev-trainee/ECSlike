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
                int count = m_wantedEntities.getCount();
                for (int i = 0; i < count; i++)
                {
                    onUpdateEntity(m_wantedEntities.getEntityComponentsAt(i));
                }
            }
            m_wantedEntities.endLock();
        }

        protected abstract System.Type[] getWantedTypes();

        protected abstract void onUpdateEntity(IComponent[] components);
    }
}
