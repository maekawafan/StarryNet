namespace StarryNet.StarryData
{
    public interface IStarryDataReference
    {
        bool Exist();
    }

    public interface IStarryDataReference<T> : IStarryDataReference
    {
        T Get();
    }
}
