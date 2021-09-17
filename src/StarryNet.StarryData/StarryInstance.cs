namespace StarryNet.StarryData
{
    public abstract class StarryInstance
    {

    }

    public abstract class StarryInstance<T> : StarryInstance where T : StarryData
    {
        public T data;
        public uint id { get { return data.id; } }
    }
}