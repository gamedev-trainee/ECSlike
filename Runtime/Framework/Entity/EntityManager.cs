using System.Collections.Generic;

namespace ECSlike
{
    public class EntityManager
    {
        private World m_world = null;

        private int m_entityCounter = 0;

        private Dictionary<int, Dictionary<System.Type, IComponent>> m_components = new Dictionary<int, Dictionary<System.Type, IComponent>>();

        private Dictionary<string, EntityList> m_entityLists = new Dictionary<string, EntityList>();
        private Dictionary<string, TypeEntityList> m_typeEntityLists = new Dictionary<string, TypeEntityList>();

        public EntityManager(World world)
        {
            m_world = world;
        }

        public int createEntity()
        {
            m_entityCounter++;
            int entity = m_entityCounter;
            m_components.Add(entity, new Dictionary<System.Type, IComponent>());
            return entity;
        }

        public void destroyEntity(int entity)
        {
            m_components.Remove(entity);
        }

        public bool addComponent<T>(int entity, T component) where T : IComponent
        {
            System.Type type = component.GetType();
            Dictionary<System.Type, IComponent> map;
            if (!m_components.TryGetValue(entity, out map))
            {
                return false;
            }
            else
            {
                if (map.ContainsKey(type))
                {
                    return false;
                }
            }
            map[type] = component;
            addTypeEntity(type, entity, map);
            return true;
        }

        public bool removeComponent<T>(int entity) where T : IComponent
        {
            Dictionary<System.Type, IComponent> map;
            if (!m_components.TryGetValue(entity, out map))
            {
                return false;
            }
            System.Type type = typeof(T);
            map.Remove(type);
            removeTypeEntity(type, entity);
            return true;
        }

        public T getComponent<T>(int entity) where T : IComponent
        {
            Dictionary<System.Type, IComponent> map;
            if (!m_components.TryGetValue(entity, out map))
            {
                return default(T);
            }
            return (T)map[typeof(T)];
        }

        public IComponent getComponent(int entity, System.Type type)
        {
            Dictionary<System.Type, IComponent> map;
            if (!m_components.TryGetValue(entity, out map))
            {
                return null;
            }
            return map[type];
        }

        public IComponent[] getComponents(int entity, System.Type[] types)
        {
            Dictionary<System.Type, IComponent> map;
            if (!m_components.TryGetValue(entity, out map))
            {
                return null;
            }
            int count = types.Length;
            IComponent[] components = new IComponent[count];
            for (int i = 0; i < count; i++)
            {
                components[i] = map[types[i]];
            }
            return components;
        }

        public Dictionary<System.Type, IComponent> getComponents(int entity)
        {
            return m_components[entity];
        }

        public TypeEntityList getTypeEntityList(System.Type[] types)
        {
            int count = types.Length;
            List<int> typeMasks = new List<int>();
            for (int i = 0; i < count; i++)
            {
                typeMasks.Add(m_world.getComponentID(types[i]));
            }
            string maskKey = string.Join("_", typeMasks);
            TypeEntityList typeEntityList;
            if (m_typeEntityLists.TryGetValue(maskKey, out typeEntityList))
            {
                return typeEntityList;
            }
            typeMasks.Sort();
            string sortedMaskKey = string.Join("_", typeMasks);
            EntityList entityList;
            if (!m_entityLists.TryGetValue(sortedMaskKey, out entityList))
            {
                entityList = new EntityList();
                m_entityLists.Add(sortedMaskKey, entityList);
            }
            typeEntityList = new TypeEntityList(types, entityList);
            m_typeEntityLists.Add(maskKey, typeEntityList);
            return typeEntityList;
        }

        protected void addTypeEntity(System.Type type, int entity, Dictionary<System.Type, IComponent> componentMap)
        {
            foreach (KeyValuePair<string, TypeEntityList> kv in m_typeEntityLists)
            {
                if (kv.Value.containsType(type))
                {
                    if (containsAllTypes(componentMap, kv.Value.getTypes()))
                    {
                        kv.Value.addEntity(entity, getComponents(entity, kv.Value.getTypes()));
                    }
                }
            }
        }

        protected void removeTypeEntity(System.Type type, int entity)
        {
            foreach (KeyValuePair<string, TypeEntityList> kv in m_typeEntityLists)
            {
                if (kv.Value.containsType(type))
                {
                    kv.Value.removeEntity(entity);
                }
            }
        }

        protected bool containsAllTypes(Dictionary<System.Type, IComponent> componentMap, System.Type[] types)
        {
            int count = types.Length;
            for (int i = 0; i < count; i++)
            {
                if (!componentMap.ContainsKey(types[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
