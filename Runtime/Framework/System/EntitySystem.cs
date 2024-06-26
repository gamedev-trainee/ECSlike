﻿namespace ECSlike
{
    public abstract class EntitySystem : AbstractSystem
    {
        private System.Type[] m_wantedTypes = null;
        private System.Type[] m_unwantedTypes = null;
        private TypeEntityList m_wantedEntities = null;

        protected override void onInit(World world)
        {
            m_wantedTypes = getWantedTypes();
            m_unwantedTypes = getUnwantedTypes();
            m_wantedEntities = world.getTypeEntityList(m_wantedTypes, m_unwantedTypes);
        }

        protected sealed override void onUpdate(World world)
        {
            m_wantedEntities.beginLock();
            {
                int id;
                int count = m_wantedEntities.getCount();
                for (int i = 0; i < count; i++)
                {
                    id = m_wantedEntities.getEntityAt(i);
                    onUpdateEntity(new Entity()
                    {
                        id = id,
                        world = world,
                    });
                }
            }
            m_wantedEntities.endLock();
        }

        protected abstract System.Type[] getWantedTypes();
        protected abstract System.Type[] getUnwantedTypes();

        protected abstract void onUpdateEntity(Entity entity);
    }
}
