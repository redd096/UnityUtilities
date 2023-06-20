
namespace redd096
{
    public class SimpleInstance<T> : Singleton<SimpleInstance<T>>
    {
        protected override bool isDontDestroyOnLoad => false;
        protected override bool automaticallyUnparentOnAwake => false;
    }
}