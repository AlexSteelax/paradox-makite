using MakItE.Core.Models.Collection;
using MakItE.Core.Services;
using System.Diagnostics;

namespace MakItE.Core.Models
{
    public partial class PCollection<T>
        where T : notnull
    {
        readonly IMemento _memento;

        internal PCollectionNode<T>? head;
        internal int count;
        internal long version;

        private bool _canMemento;

        public PCollection(IMemento memento)
        {
            _memento = memento;
            _canMemento = true;
        }
        public PCollection(IMemento memento, IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            _canMemento = false;

            foreach (T item in collection)
            {
                ArgumentNullException.ThrowIfNull(item);
                AddLast(item);
            }

            version = 0;

            _memento = memento;
            _canMemento = true;
        }

        public int Count => count;
        public long Version => version;

        public IEnumerable<T> List => this.Select(item => item.Value);
        void AddAfter(PCollectionNode<T> node, PCollectionNode<T> newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);
            InternalInsertNodeBefore(node.next!, newNode);
            newNode.list = this;
        }
        public PCollectionNode<T> AddAfter(PCollectionNode<T> node, T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            ValidateNode(node);
            var newNode = new PCollectionNode<T>(node.list!, value);
            InternalInsertNodeBefore(node.next!, newNode);

            return newNode;
        }
        void AddBefore(PCollectionNode<T> node, PCollectionNode<T> newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);
            InternalInsertNodeBefore(node, newNode);
            newNode.list = this;
            if (node == head)
            {
                head = newNode;
            }
        }
        public PCollectionNode<T> AddBefore(PCollectionNode<T> node, T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            ValidateNode(node);
            var newNode = new PCollectionNode<T>(node.list!, value);
            InternalInsertNodeBefore(node, newNode);
            if (node == head)
                head = newNode;

            return newNode;
        }
        public PCollectionNode<T> AddFirst(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            var newNode = new PCollectionNode<T>(this, value);
            if (head == null)
            {
                InternalInsertNodeToEmptyList(newNode);
            }
            else
            {
                InternalInsertNodeBefore(head, newNode);
                head = newNode;
            }
            return newNode;
        }
        public PCollectionNode<T> AddLast(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            var newNode = new PCollectionNode<T>(this, value);
            if (head == null)
            {
                InternalInsertNodeToEmptyList(newNode);
            }
            else
            {
                InternalInsertNodeBefore(head, newNode);
            }
            return newNode;
        }
        public void Clear()
        {
            var _version = version;
            var _items = new List<PCollectionNode<T>>();

            var current = head;
            while (current != null)
            {
                var temp = current;
                current = current.Next;
                temp.Invalidate();

                _items.Add(temp);
            }

            head = null;
            count = 0;
            version++;

            CreateSnapshot(() =>
            {
                foreach (var item in _items)
                {
                    item.list = this;
                    if (head == null)
                    {
                        InternalInsertNodeToEmptyList(item);
                    }
                    else
                    {
                        InternalInsertNodeBefore(head, item);
                    }
                }
                version = _version;
            });
        }
        public void Remove(PCollectionNode<T> node)
        {
            ValidateNode(node);
            InternalRemoveNode(node);
        }

        void InternalRemoveNode(PCollectionNode<T> node)
        {
            var _version = version;

            Debug.Assert(node.list == this, "Deleting the node from another list!");
            Debug.Assert(head != null, "This method shouldn't be called on empty list!");

            if (node.next == node)
            {
                Debug.Assert(count == 1 && head == node, "this should only be true for a list with only one node");
                head = null;

                node.Invalidate();
                count--;
                version++;

                CreateSnapshot(() =>
                {
                    node.list = this;
                    InternalInsertNodeToEmptyList(node);
                    version = _version;
                });
            }
            else
            {
                var _prev = node.prev;
                var _next = node.next;
                var _before = node == head;

                node.next!.prev = node.prev;
                node.prev!.next = node.next;
                if (head == node)
                {
                    head = node.next;
                }

                node.Invalidate();
                count--;
                version++;

                CreateSnapshot(() =>
                {
                    if (_before) AddBefore(_next!, node);
                    else AddAfter(_prev!, node);

                    version = _version;
                });
            }
        }
        void InternalInsertNodeToEmptyList(PCollectionNode<T> newNode)
        {
            var _version = version;

            Debug.Assert(head == null && count == 0, "PCollection must be empty when this method is called!");
            newNode.next = newNode;
            newNode.prev = newNode;
            head = newNode;
            version++;
            count++;

            CreateSnapshot(() =>
            {
                Remove(newNode);
                version = _version;
            });
        }
        void InternalInsertNodeBefore(PCollectionNode<T> node, PCollectionNode<T> newNode)
        {
            var _version = version;

            newNode.next = node;
            newNode.prev = node.prev;
            node.prev!.next = newNode;
            node.prev = newNode;
            version++;
            count++;

            CreateSnapshot(() =>
            {
                Remove(newNode);
                version = _version;
            });
        }
        void ValidateNode(PCollectionNode<T> node)
        {
            ArgumentNullException.ThrowIfNull(node);
            
            if (node.list != this)
                throw new InvalidOperationException("External PCollectionNode");
        }
        static void ValidateNewNode(PCollectionNode<T> node)
        {
            ArgumentNullException.ThrowIfNull(node);

            if (node.list != null)
                throw new InvalidOperationException("PCollectionNodeIsAttached");
        }
        internal void CreateSnapshot(Action action)
        {
            if (_canMemento)
            {
                var snapshot = new Snapshot(() =>
                {
                    _canMemento = false;
                    action();
                    _canMemento = true;
                });
                _memento.Add(snapshot);
            }
        }

        class Snapshot: ISnapshot
        {
            readonly Action _action;

            public Snapshot(Action action) => _action  = action;

            public void Apply() => _action();
        }
    }
}
