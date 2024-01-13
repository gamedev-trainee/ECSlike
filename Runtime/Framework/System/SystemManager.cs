using System.Collections.Generic;

namespace ECSlike
{
    public class SystemManager
    {
        private World m_world = null;

        private List<ISystem> m_systems = new List<ISystem>();

        public SystemManager(World world)
        {
            m_world = world;
        }

        public void addSystem<T>() where T : ISystem, new()
        {
            ISystem system = new T();
            system.init(m_world);
            m_systems.Add(system);
        }

        public void update()
        {
            int count = m_systems.Count;
            for (int i = 0; i < count; i++)
            {
                m_systems[i].update();
            }
        }
    }
}
