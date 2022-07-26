using System.Collections.Generic;

namespace StarryNet.StarryData
{
    public abstract class StarryDataID
    {
        public uint id;

        public StarryDataID() { }

        protected StarryDataID(uint id)
        {
            this.id = id;
        }

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
        public StarryDataID() : base() { }
        protected StarryDataID(uint id) : base(id) { }

        public abstract T Get();

        public bool Exist()
        {
            return Get() != null;
        }

        public void Set<T2>(T2 value) where T2 : StarryData
        {
            this.id = value.id;
        }
    }
}
