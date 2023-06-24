using System.Collections;
using MakItE.Core.Models.Collection;

namespace MakItE.Core.Models
{
    public partial class PCollection<T> : IEnumerable<PCollectionNode<T>>
    {
        public IEnumerator<PCollectionNode<T>> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public struct Enumerator : IEnumerator<PCollectionNode<T>>, IEnumerator
        {
            public PCollectionNode<T> Current => _current!;
            object IEnumerator.Current => _current!;

            readonly PCollection<T> _list;
            PCollectionNode<T>? _node;
            readonly long _version;

            PCollectionNode<T>? _current;

            internal Enumerator(PCollection<T> list)
            {
                _list = list;
                _version = list.version;
                _node = list.head;
                _current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_version != _list.version)
                {
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                }
                if (_node == null)
                {
                    return false;
                }

                _current = _node;
                _node = _node.next;

                if (_node == _list.head)
                {
                    _node = null;
                }

                return true;
            }

            public void Reset()
            {
                if (_version != _list.version)
                {
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                }

                _current = default;
                _node = _list.head;
            }
        }
    }
}
