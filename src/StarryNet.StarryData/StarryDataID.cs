using System.Collections.Generic;

namespace StarryNet.StarryData
{
    public abstract class StarryDataID
    {
        public readonly uint id;

        public class Comparer : IEqualityComparer<StarryDataID>
        {
            public bool Equals(StarryDataID x, StarryDataID y)
            {
                return x.id == y.id;
            }

            public int GetHashCode(StarryDataID obj)
            {
                return obj.id.GetHashCode();
            }
        }
    }

    public abstract class StarryDataID<T> : StarryDataID, IStarryDataReference<T>
    {
        public abstract T Get();

        public bool Exist()
        {
            return Get() != null;
        }
    }
}
