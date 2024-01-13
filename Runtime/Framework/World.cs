using System.Collections.Generic;
using UnityEngine;

namespace ECSlike
{
    public class World
    {
        private EntityManager m_entityManager = null;
        private ComponentManager m_componentManager = null;
        private SystemManager m_systemManager = null;

        public World(ComponentRegisterData[] componentRegisters)
        {
            m_entityManager = new EntityManager(this);
            m_componentManager = new ComponentManager(componentRegisters);
            m_systemManager = new SystemManager(this);
        }

        public int createEntity()
        {
            return m_entityManager.createEntity();
        }

        public int createEntityBy(GameObject go)
        {
            return createEntityBy(go.GetComponents<IComponentConfig>());
        }

        public int createEntityBy(IComponentConfig[] componentConfigs)
        {
            int entity = createEntity();
            if (componentConfigs != null && componentConfigs.Length > 0)
            {
                int count = componentConfigs.Length;
                for (int i = 0; i < count; i++)
                {
                    addComponent(entity, componentConfigs[i]);
                }
            }
            return entity;
        }

        public void destroyEntity(int entity)
        {
            m_entityManager.destroyEntity(entity);
        }

        internal TypeEntityList getTypeEntityList(System.Type[] types)
        {
            return m_entityManager.getTypeEntityList(types);
        }

        internal int getComponentID(System.Type type)
        {
            return m_componentManager.getComponentID(type);
        }

        public void addComponent(int entity, IComponentConfig config)
        {
            IComponent component = m_componentManager.createComponent(config);
            if (component != null)
            {
                m_entityManager.addComponent(entity, component);
            }
        }

        public Dictionary<System.Type, IComponent> getComponents(int entity)
        {
            return m_entityManager.getComponents(entity);
        }

        public void addSystem<T>() where T : ISystem, new()
        {
            m_systemManager.addSystem<T>();
        }

        public void update()
        {
            m_systemManager.update();
        }
    }
}
