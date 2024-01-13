namespace ECSlike
{
    public interface ISystem
    {
        void init(World world);
        void update();
    }
}
