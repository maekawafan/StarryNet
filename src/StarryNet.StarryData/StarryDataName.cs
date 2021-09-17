using System.Collections.Generic;

namespace StarryNet.StarryData
{
    public abstract class StarryDataName
    {
        public readonly string name;

        public class Comparer : IEqualityComparer<StarryDataName>
        {
            public bool Equals(StarryDataName x, StarryDataName y)
            {
                return x.name == y.name;
            }

            public int GetHashCode(StarryDataName obj)
            {
                return obj.name.GetHashCode();
            }
        }
    }

    public abstract class StarryDataName<T> : StarryDataName, IStarryDataReference<T>
    {
        public abstract T Get();

        public bool Exist()
        {
            return Get() != null;
        }
    }
}
