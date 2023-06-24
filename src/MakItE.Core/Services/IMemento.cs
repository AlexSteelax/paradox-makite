namespace MakItE.Core.Services
{
    public interface IMemento
    {
        void Add(ISnapshot snapshot);
        void Rollback();
        void Reset();
        int QueueCount { get; }
    }
    public interface ISnapshot
    {
        void Apply();
    }
}