namespace WebProject.Extensions
{
    public class LazyService<T> : Lazy<T> where T : class
    {
        public LazyService(IServiceProvider provider)
            : base(() => provider.GetRequiredService<T>())
        {
        }
    }
}
