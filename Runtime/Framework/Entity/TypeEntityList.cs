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

        public System.Type[] getTypes()
        {
            return m_types;
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

        public void addEntity(int value, IComponent[] components)
        {
            m_entityList.addEntity(value);
            m_entityComponents.Add(value, components);
        }

        public void removeEntity(int value)
        {
            m_entityList.removeEntity(value);
            m_entityComponents.Remove(value);
        }

        public IComponent[] getEntityComponents(int value)
        {
            return m_entityComponents[value];
        }

        public void getEntityAndComponentsAt(int index, out int entity, out IComponent[] components)
        {
            entity = m_entityList.getAt(index);
            components = getEntityComponents(entity);
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
