using System.Collections.Generic;

namespace ECSlike
{
    public class TypeEntityList
    {
        private System.Type[] m_types = null;
        private EntityList m_entityList = null;
        private Dictionary<int, IComponent[]> m_entityComponents = new Dictionary<int, IComponent[]>();

        public TypeEntityList(System.Type[] types, EntityList entityList)
        {
            m_types = types;
            m_entityList = entityList;
        }

        public bool containsType(System.Type type)
        {
            int count = m_types.Length;
            for (int i = 0; i < count; i++)
            {
                if (m_types[i] == type)
                {
                    return true;
                }
            }
            return false;
        }

        protected int indexOfType(System.Type type)
        {
            int count = m_types.Length;
            for (int i = 0; i < count; i++)
            {
                if (m_types[i] == type)
                {
                    return i;
                }
            }
            return -1;
        }

        public void addEntity(int value, IComponent component)
        {
            m_entityList.addEntity(value);
            IComponent[] components;
            if (!m_entityComponents.TryGetValue(value, out components))
            {
                components = new IComponent[m_types.Length];
                m_entityComponents[value] = components;
            }
            components[indexOfType(component.GetType())] = component;
        }

        public void removeEntity(int value, IComponent component)
        {
            m_entityList.removeEntity(value);
            IComponent[] components;
            if (!m_entityComponents.TryGetValue(value, out components))
            {
                return;
            }
            components[indexOfType(component.GetType())] = null;
        }

        public IComponent[] getEntityComponents(int value)
        {
            return m_entityComponents[value];
        }

        public IComponent[] getEntityComponentsAt(int index)
        {
            int entity = m_entityList.getAt(index);
            return getEntityComponents(entity);
        }

        public void beginLock()
        {
            m_entityList.beginLock();
        }

        public void endLock()
        {
            m_entityList.endLock();
        }

        public int getCount()
        {
            return m_entityList.getCount();
        }
    }
}
