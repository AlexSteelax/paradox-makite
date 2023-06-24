namespace MakItE.Core.Services
{
    internal static class MementoService
    {
        public static IMemento Instance { get; }

        static MementoService()
        {
            Instance = new Memento();
        }
    }
}
