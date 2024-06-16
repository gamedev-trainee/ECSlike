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

        internal TypeEntityList getTypeEntityList(System.Type[] wantedTypes, System.Type[] unwantedTypes)
        {
            return m_entityManager.getTypeEntityList(wantedTypes, unwantedTypes);
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

        public T addComponent<T>(int entity) where T : IComponent, new()
        {
            T component = new T();
            m_entityManager.addComponent(entity, component);
            return component;
        }

        public T getComponent<T>(int entity) where T : IComponent
        {
            return m_entityManager.getComponent<T>(entity);
        }

        public T getOrAddComponent<T>(int entity) where T : IComponent, new()
        {
            T component = m_entityManager.getComponent<T>(entity);
            if (component == null) component = new T();
            m_entityManager.addComponent(entity, component);
            return component;
        }

        public void removeComponent<T>(int entity) where T : IComponent
        {
            m_entityManager.removeComponent<T>(entity);
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
