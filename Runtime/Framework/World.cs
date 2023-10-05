using System.Collections.Generic;
using UnityEngine;

namespace ECSlike
{
    public class World
    {
        public World(ComponentRegisterData[] componentRegisters)
        {
            initEntity();
            initComponent(componentRegisters);
            initSystem();
        }

        #region Cache

        public static readonly int ComponentCacheCapacity = 64;

        private List<IComponent[]> m_freeEntityComponentArrays = new List<IComponent[]>();

        protected void initCache()
        {
            m_freeEntityComponentArrays.Capacity = ComponentCacheCapacity;
        }

        protected IComponent[] requestEntityComponents()
        {
            if (m_freeEntityComponentArrays.Count > 0)
            {
                IComponent[] components = m_freeEntityComponentArrays[m_freeEntityComponentArrays.Count - 1];
                m_freeEntityComponentArrays.RemoveAt(m_freeEntityComponentArrays.Count - 1);
                int count = components.Length;
                for (int i = 0; i < count; i++)
                {
                    components[i] = null;
                }
                return components;
            }
            return new IComponent[getComponentTotal()];
        }

        protected void recycleEntityComponents(IComponent[] components)
        {
            m_freeEntityComponentArrays.Add(components);
        }

        #endregion

        #region Entity

        public static readonly int EntityCapacity = 128;
        public static readonly int EntityListCapacity = 32;

        private int m_entityIDCounter = 0;
        private Dictionary<int, int> m_entityComponentMasks = new Dictionary<int, int>();
        private Dictionary<int, IComponent[]> m_entityComponents = new Dictionary<int, IComponent[]>();
        private Dictionary<int, EntityList> m_entityLists = new Dictionary<int, EntityList>();

        protected void initEntity()
        {
            m_entityComponentMasks.EnsureCapacity(EntityCapacity);
            m_entityComponents.EnsureCapacity(EntityCapacity);
            m_entityLists.EnsureCapacity(EntityListCapacity);
        }

        public int createEntity()
        {
            int id = ++m_entityIDCounter;
            m_entityComponentMasks.Add(id, 0);
            m_entityComponents.Add(id, null);
            return id;
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
            IComponent[] components;
            if (m_entityComponents.TryGetValue(entity, out components))
            {
                m_entityComponentMasks.Remove(entity);
                m_entityComponents.Remove(entity);
                recycleEntityComponents(components);
            }
        }

        public void addComponent(int entity, IComponent component)
        {
            int componentID = getComponentID(component.GetType());
            int componentIndex = componentID - 1;
            int oldComponentMask;
            int newComponentMask;
            IComponent[] components;
            if (!m_entityComponentMasks.TryGetValue(entity, out oldComponentMask))
            {
                newComponentMask = 1 << componentID;
                m_entityComponentMasks.Add(entity, newComponentMask);
                components = requestEntityComponents();
                m_entityComponents.Add(entity, components);
            }
            else
            {
                newComponentMask = oldComponentMask | (1 << componentID);
                m_entityComponentMasks[entity] = newComponentMask;
                components = m_entityComponents[entity];
                if (components == null)
                {
                    components = requestEntityComponents();
                    m_entityComponents[entity] = components;
                }
            }
            components[componentIndex] = component;
            modifyEntityList(entity, oldComponentMask, newComponentMask);
        }

        public void addComponent(int entity, IComponentConfig config)
        {
            IComponent component = createComponent(config);
            if (component != null)
            {
                addComponent(entity, component);
            }
        }

        public void removeComponent(int entity, IComponent component)
        {
            int oldComponentMask;
            if (!m_entityComponentMasks.TryGetValue(entity, out oldComponentMask))
            {
                return;
            }
            int componentID = getComponentID(component.GetType());
            int newComponentMask = oldComponentMask & (~(1 << componentID));
            m_entityComponentMasks[entity] = newComponentMask;
            int index = componentID - 1;
            IComponent[] components = m_entityComponents[entity];
            components[index] = null;
            modifyEntityList(entity, oldComponentMask, newComponentMask);
        }

        public T getComponent<T>(int entity) where T : IComponent
        {
            IComponent[] entityComponents;
            if (!m_entityComponents.TryGetValue(entity, out entityComponents))
            {
                return default(T);
            }
            System.Type type = typeof(T);
            int componentID = getComponentID(type);
            int index = componentID - 1;
            return (T)entityComponents[index];
        }

        public IComponent[] getComponents(int entity, int componentMask)
        {
            IComponent[] entityComponents;
            if (!m_entityComponents.TryGetValue(entity, out entityComponents))
            {
                return null;
            }
            int[] componentIDs;
            if (!m_componentMaskToIDs.TryGetValue(componentMask, out componentIDs))
            {
                return null;
            }
            int count = componentIDs.Length;
            IComponent[] components = new IComponent[count];
            for (int i = 0; i < count; i++)
            {
                components[i] = entityComponents[componentIDs[i] - 1];
            }
            return components;
        }

        public EntityList allocateEntityList(System.Type[] componentTypes)
        {
            if (componentTypes == null || componentTypes.Length <= 0)
            {
                return null;
            }
            int componentMask = 0;
            int componentID;
            int count = componentTypes.Length;
            for (int i = 0; i < count; i++)
            {
                componentID = getComponentID(componentTypes[i]);
                componentMask |= 1 << componentID;
            }
            EntityList entityList;
            if (!m_entityLists.TryGetValue(componentMask, out entityList))
            {
                entityList = new EntityList();
                m_entityLists[componentMask] = entityList;
                int[] componentIDs = new int[count];
                for (int i = 0; i < count; i++)
                {
                    componentIDs[i] = getComponentID(componentTypes[i]);
                }
                m_componentMaskToIDs.Add(componentMask, componentIDs);
            }
            if (m_entityComponentMasks.Count > 0)
            {
                foreach (KeyValuePair<int, int> kv in m_entityComponentMasks)
                {
                    if (kv.Value == componentMask)
                    {
                        entityList.addEntity(kv.Key, getComponents(kv.Key, componentMask));
                    }
                }
            }
            return entityList;
        }

        protected void modifyEntityList(int entity, int oldComponentMask, int newComponentMask)
        {
            foreach (KeyValuePair<int, EntityList> kv in m_entityLists)
            {
                if ((oldComponentMask & kv.Key) == kv.Key)
                {
                    if ((newComponentMask & kv.Key) != kv.Key)
                    {
                        kv.Value.removeEntity(entity);
                    }
                }
                else
                {
                    if ((newComponentMask & kv.Key) == kv.Key)
                    {
                        kv.Value.addEntity(entity, getComponents(entity, newComponentMask));
                    }
                }
            }
        }

        #endregion

        #region Component

        private int m_componentTotal = 0;
        private Dictionary<System.Type, int> m_components = new Dictionary<System.Type, int>();
        private Dictionary<int, int[]> m_componentMaskToIDs = new Dictionary<int, int[]>();
        private Dictionary<System.Type, System.Func<IComponentConfig, IComponent>> m_componentCreators = new Dictionary<System.Type, System.Func<IComponentConfig, IComponent>>();

        protected void initComponent(ComponentRegisterData[] componentRegisters)
        {
            m_componentTotal = componentRegisters.Length;
            m_components.EnsureCapacity(m_componentTotal);
            for (int i = 0; i < m_componentTotal; i++)
            {
                m_components.Add(componentRegisters[i].componentType, i + 1);
                if (componentRegisters[i].configType != null && componentRegisters[i].componentCreator != null)
                {
                    m_componentCreators.Add(componentRegisters[i].configType, componentRegisters[i].componentCreator);
                }
            }
        }

        protected IComponent createComponent(IComponentConfig config)
        {
            System.Func<IComponentConfig, IComponent> creator;
            if (m_componentCreators.TryGetValue(config.GetType(), out creator))
            {
                return creator.Invoke(config);
            }
            return null;
        }

        protected int getComponentTotal()
        {
            return m_componentTotal;
        }

        protected int getComponentID(System.Type type)
        {
            return m_components[type];
        }

        protected int getComponentsMask(System.Type[] types)
        {
            int mask = 0;
            if (types != null && types.Length > 0)
            {
                int componentID;
                int count = types.Length;
                for (int i = 0; i < count; i++)
                {
                    componentID = getComponentID(types[i]);
                    mask |= 1 << componentID;
                }
            }
            return mask;
        }

        #endregion

        #region System

        public static readonly int SystemCapacity = 64;

        private List<ISystem> m_systems = new List<ISystem>();

        protected void initSystem()
        {
            m_systems.Capacity = SystemCapacity;
        }

        public void addSystem<T>() where T : ISystem, new()
        {
            m_systems.Add(new T());
        }

        public void update()
        {
            int count = m_systems.Count;
            for (int i = 0; i < count; i++)
            {
                m_systems[i].update(this);
            }
        }

        #endregion
    }
}
