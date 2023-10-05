using System.Collections.Generic;

namespace ECSlike
{
    public class EntityList
    {
        private bool m_executing = false;
        private List<int> m_list = new List<int>();
        private List<int> m_addList = new List<int>();
        private List<int> m_removeList = new List<int>();
        private Dictionary<int, IComponent[]> m_componentMap = new Dictionary<int, IComponent[]>();

        public void addEntity(int entity, IComponent[] components)
        {
            if (m_list.Contains(entity)) return;
            if (m_executing)
            {
                if (m_addList.Contains(entity)) return;
                m_componentMap.Add(entity, components);
                m_addList.Add(entity);
            }
            else
            {
                m_componentMap.Add(entity, components);
                onAddEntity(entity);
            }
        }

        protected void onAddEntity(int entity)
        {
            m_list.Add(entity);
        }

        public void removeEntity(int entity)
        {
            if (!m_list.Contains(entity)) return;
            if (m_executing)
            {
                m_addList.Remove(entity);
                if (m_removeList.Contains(entity)) return;
                m_removeList.Add(entity);
            }
            else
            {
                onRemoveEntity(entity);
            }
        }

        protected void onRemoveEntity(int entity)
        {
            m_list.Remove(entity);
            m_componentMap.Remove(entity);
        }

        public void eachEntity(System.Action<IComponent[]> handler)
        {
            m_executing = true;
            int count = m_list.Count;
            for (int i = 0; i < count; i++)
            {
                handler.Invoke(m_componentMap[m_list[i]]);
            }
            m_executing = false;
            flush();
        }

        protected void flush()
        {
            if (m_removeList.Count > 0)
            {
                int count = m_removeList.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    onRemoveEntity(m_removeList[i]);
                }
                m_removeList.Clear();
            }
            if (m_addList.Count > 0)
            {
                int count = m_addList.Count;
                for (int i = 0; i < count; i++)
                {
                    onAddEntity(m_addList[i]);
                }
                m_addList.Clear();
            }
        }

        public int getCount()
        {
            return m_list.Count;
        }
    }
}
