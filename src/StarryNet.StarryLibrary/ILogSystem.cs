namespace StarryNet.StarryLibrary
{
    public interface ILogSystem
    {
        void Write(string value);
        void Write<T>(T value);
        void Write(string title, string value);
        void Write<T>(string title, T value);
        void Error(string value);
        void Error<T>(T value);
        void Error(string title, string value);
        void Error<T>(string title, T value);
        void Info(string value);
        void Info<T>(T value);
        void Info(string title, string value);
        void Info<T>(string title, T value);
        void Debug(string value);
        void Debug<T>(T value);
        void Debug(string title, string value);
        void Debug<T>(string title, T value);
    }
}
