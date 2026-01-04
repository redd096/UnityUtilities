
namespace redd096.Singletons
{
    /// <summary>
    /// This is useful to have the instance static variable, but can't be DontDestroyOnLoad and can have multiple instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleInstance<T> : Singleton<T> where T : SimpleInstance<T>
  {
      protected override bool isDontDestroyOnLoad => false;
      protected override bool automaticallyUnparentOnSetDontDestroyOnLoad => false;
      protected override bool destroyCopies => false;
  }
}