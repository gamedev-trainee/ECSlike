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

        internal Entity createEntity()
        {
            m_entityCounter++;
            int id = m_entityCounter;
            m_components.Add(id, new Dictionary<System.Type, IComponent>());
            return new Entity()
            {
                id = id,
                world = m_world,
            };
        }

        internal void destroyEntity(int entity)
        {
            foreach (KeyValuePair<string, EntityList> kv in m_entityLists)
            {
                kv.Value.removeEntity(entity);
            }
            foreach (KeyValuePair<string, TypeEntityList> kv in m_typeEntityLists)
            {
                kv.Value.removeEntity(entity);
            }
            m_components.Remove(entity);
        }

        internal bool addComponent<T>(int entity, T component) where T : IComponent
        {
            System.Type type = component.GetType();
            Dictionary<System.Type, IComponent> map;
            if (!m_components.TryGetValue(entity, out map))
            {
                return false;
            }
            if (map.ContainsKey(type))
            {
                return false;
            }
            map[type] = component;
            addTypeEntity(type, entity, map);
            return true;
        }

        internal bool removeComponent<T>(int entity) where T : IComponent
        {
            Dictionary<System.Type, IComponent> map;
            if (!m_components.TryGetValue(entity, out map))
            {
                return false;
            }
            System.Type type = typeof(T);
            map.Remove(type);
            removeTypeEntity(type, entity, map);
            return true;
        }

        internal T getComponent<T>(int entity) where T : IComponent
        {
            Dictionary<System.Type, IComponent> map;
            if (!m_components.TryGetValue(entity, out map))
            {
                return default(T);
            }
            System.Type type = typeof(T);
            IComponent component;
            if (!map.TryGetValue(type, out component))
            {
                return default(T);
            }
            return (T)component;
        }

        internal IComponent getComponent(int entity, System.Type type)
        {
            Dictionary<System.Type, IComponent> map;
            if (!m_components.TryGetValue(entity, out map))
            {
                return null;
            }
            IComponent component;
            if (!map.TryGetValue(type, out component))
            {
                return null;
            }
            return component;
        }

        internal IComponent[] getComponents(int entity, System.Type[] types)
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

        internal Dictionary<System.Type, IComponent> getComponents(int entity)
        {
            return m_components[entity];
        }

        internal TypeEntityList getTypeEntityList(System.Type[] wantedTypes, System.Type[] unwantedTypes)
        {
            string wantedMaskKey = string.Empty;
            string unwantedMaskKey = string.Empty;
            List<int> wantedTypeMasks = new List<int>();
            List<int> unwantedTypeMasks = new List<int>();
            int count;
            if (wantedTypes != null && wantedTypes.Length > 0)
            {
                count = wantedTypes.Length;
                for (int i = 0; i < count; i++)
                {
                    wantedTypeMasks.Add(m_world.getComponentID(wantedTypes[i]));
                }
                wantedMaskKey = wantedTypeMasks.Count > 0 ? string.Join("_", wantedTypeMasks) : string.Empty;
            }
            if (unwantedTypes != null && unwantedTypes.Length > 0)
            {
                count = unwantedTypes.Length;
                for (int i = 0; i < count; i++)
                {
                    unwantedTypeMasks.Add(m_world.getComponentID(unwantedTypes[i]));
                }
                unwantedMaskKey = unwantedTypeMasks.Count > 0 ? string.Join("_", unwantedTypeMasks) : string.Empty;
            }
            string maskKey = string.Format("{0}-{1}", wantedMaskKey, unwantedMaskKey);
            TypeEntityList typeEntityList;
            if (m_typeEntityLists.TryGetValue(maskKey, out typeEntityList))
            {
                return typeEntityList;
            }
            wantedTypeMasks.Sort();
            unwantedTypeMasks.Sort();
            string sortedWantedMaskKey = wantedTypeMasks.Count > 0 ? string.Join("_", wantedTypeMasks) : string.Empty;
            string sortedUnwantedMaskKey = unwantedTypeMasks.Count > 0 ? string.Join("_", unwantedTypeMasks) : string.Empty;
            string sortedMaskKey = string.Format("{0}-{1}", sortedWantedMaskKey, sortedUnwantedMaskKey);
            EntityList entityList;
            if (!m_entityLists.TryGetValue(sortedMaskKey, out entityList))
            {
                entityList = new EntityList();
                m_entityLists.Add(sortedMaskKey, entityList);
            }
            typeEntityList = new TypeEntityList(wantedTypes, unwantedTypes, entityList);
            m_typeEntityLists.Add(maskKey, typeEntityList);
            return typeEntityList;
        }

        protected void addTypeEntity(System.Type type, int entity, Dictionary<System.Type, IComponent> componentMap)
        {
            foreach (KeyValuePair<string, TypeEntityList> kv in m_typeEntityLists)
            {
                if (kv.Value.containsUnwantedType(type))
                {
                    kv.Value.removeEntity(entity);
                }
                else if (kv.Value.containsWantedType(type))
                {
                    if (containsAllTypes(componentMap, kv.Value.getWantedTypes(), kv.Value.getUnwantedTypes()))
                    {
                        kv.Value.addEntity(entity);
                    }
                }
            }
        }

        protected void removeTypeEntity(System.Type type, int entity, Dictionary<System.Type, IComponent> componentMap)
        {
            foreach (KeyValuePair<string, TypeEntityList> kv in m_typeEntityLists)
            {
                if (kv.Value.containsWantedType(type))
                {
                    kv.Value.removeEntity(entity);
                }
                else if (kv.Value.containsUnwantedType(type))
                {
                    if (containsAllTypes(componentMap, kv.Value.getWantedTypes(), kv.Value.getUnwantedTypes()))
                    {
                        kv.Value.addEntity(entity);
                    }
                }
            }
        }

        protected bool containsAllTypes(Dictionary<System.Type, IComponent> componentMap, System.Type[] wantedTypes, System.Type[] unwantedTypes)
        {
            int count;
            if (wantedTypes != null)
            {
                count = wantedTypes.Length;
                for (int i = 0; i < count; i++)
                {
                    if (!componentMap.ContainsKey(wantedTypes[i]))
                    {
                        return false;
                    }
                }
            }
            if (unwantedTypes != null)
            {
                count = unwantedTypes.Length;
                for (int i = 0; i < count; i++)
                {
                    if (componentMap.ContainsKey(unwantedTypes[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
