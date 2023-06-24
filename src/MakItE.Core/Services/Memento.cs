namespace MakItE.Core.Services
{
    internal class Memento : IMemento
    {
        readonly Stack<ISnapshot> _stack = new();

        public int QueueCount => _stack.Count;

        public void Rollback()
        {
            while (_stack.Count > 0)
                _stack.Pop().Apply();
        }

        public void Reset()
        {
            _stack.Clear();
        }

        public void Add(ISnapshot snapshot) => _stack.Push(snapshot);
    }
}
