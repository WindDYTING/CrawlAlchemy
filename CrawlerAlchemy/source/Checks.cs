namespace CrawlerAlchemy
{
    public static class Checks
    {
        public static T EnsureNotNull<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj;
        }
    }
}
