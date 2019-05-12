using System.Collections.Generic;

namespace ZeroDawn.Helper
{
    public class FileMapComparer : IEqualityComparer<KeyValuePair<long, long>>
    {
        public bool Equals(KeyValuePair<long, long> x, KeyValuePair<long, long> y)
        {
            return EqualityComparer<long>.Default.Equals(x.Key, y.Key);
        }

        public int GetHashCode(KeyValuePair<long, long> obj)
        {
            return EqualityComparer<long>.Default.GetHashCode(obj.Key);
        }
    }
}
