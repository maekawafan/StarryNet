using System.Collections.Generic;

namespace StarryNet.StarryData
{
    public abstract class StarryDataName
    {
        public string name { get; protected set; }

        public StarryDataName() { }

        protected StarryDataName(string name)
        {
            this.name = name;
        }

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
        public StarryDataName() : base() { }
        protected StarryDataName(string name) : base(name) { }

        public abstract T Get();

        public bool Exist()
        {
            return Get() != null;
        }

        public void Set<T2>(T2 value) where T2 : StarryData
        {
            this.name = value.name;
        }
    }
}
