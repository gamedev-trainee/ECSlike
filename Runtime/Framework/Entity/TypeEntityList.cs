namespace ECSlike
{
    public class TypeEntityList
    {
        private System.Type[] m_wantedTypes = null;
        private System.Type[] m_unwantedTypes = null;
        private EntityList m_entityList = null;

        public TypeEntityList(System.Type[] wantedTypes, System.Type[] unwantedTypes, EntityList entityList)
        {
            m_wantedTypes = wantedTypes;
            m_unwantedTypes = unwantedTypes;
            m_entityList = entityList;
        }

        public System.Type[] getWantedTypes()
        {
            return m_wantedTypes;
        }

        public System.Type[] getUnwantedTypes()
        {
            return m_unwantedTypes;
        }

        public bool containsWantedType(System.Type type)
        {
            if (m_wantedTypes != null)
            {
                int count = m_wantedTypes.Length;
                for (int i = 0; i < count; i++)
                {
                    if (m_wantedTypes[i] == type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool containsUnwantedType(System.Type type)
        {
            if (m_unwantedTypes != null)
            {
                int count = m_unwantedTypes.Length;
                for (int i = 0; i < count; i++)
                {
                    if (m_unwantedTypes[i] == type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void addEntity(int value)
        {
            m_entityList.addEntity(value);
        }

        public void removeEntity(int value)
        {
            m_entityList.removeEntity(value);
        }

        public int getEntityAt(int index)
        {
            return m_entityList.getAt(index);
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
