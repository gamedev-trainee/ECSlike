namespace ECSlike
{
    public abstract class AbstractSystem : ISystem
    {
        private World m_world = null;

        public void init(World world)
        {
            m_world = world;
            onInit(m_world);
        }

        public void update()
        {
            onUpdate(m_world);
        }

        protected abstract void onInit(World world);
        protected abstract void onUpdate(World world);
    }
}
