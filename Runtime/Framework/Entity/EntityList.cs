using System.Collections.Generic;

namespace ECSlike
{
    public class EntityList
    {
        private bool m_locked = false;

        private List<int> m_list = new List<int>();
        private List<int> m_adds = new List<int>();
        private List<int> m_removes = new List<int>();

        public void addEntity(int value)
        {
            if (m_locked)
            {
                m_adds.Add(value);
                m_removes.Remove(value);
            }
            else
            {
                m_list.Add(value);
            }
        }

        public void removeEntity(int value)
        {
            if (m_locked)
            {
                m_removes.Add(value);
                m_adds.Remove(value);
            }
            else
            {
                m_list.Remove(value);
            }
        }

        public void beginLock()
        {
            m_locked = true;
        }

        public void endLock()
        {
            m_locked = false;
            flush();
        }

        public void flush()
        {
            if (m_removes.Count > 0)
            {
                if (m_list.Count > 0)
                {
                    int count = m_removes.Count;
                    for (int i = 0; i < count; i++)
                    {
                        m_list.Remove(m_removes[i]);
                    }
                }
                m_removes.Clear();
            }
            if (m_adds.Count > 0)
            {
                int count = m_adds.Count;
                for (int i = 0; i < count; i++)
                {
                    m_list.Add(m_adds[i]);
                }
                m_adds.Clear();
            }
        }

        public int getCount()
        {
            return m_list.Count;
        }

        public int getAt(int index)
        {
            if (index < 0 || index >= m_list.Count) return 0;
            return m_list[index];
        }
    }
}
