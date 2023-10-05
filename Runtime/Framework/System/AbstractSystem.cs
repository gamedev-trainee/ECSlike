namespace ECSlike
{
    public abstract class AbstractSystem : ISystem
    {
        public abstract void update(World world);
    }
}
