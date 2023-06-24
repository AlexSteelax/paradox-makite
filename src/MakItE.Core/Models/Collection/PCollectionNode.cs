using System.Diagnostics.CodeAnalysis;

namespace MakItE.Core.Models.Collection
{
    public sealed class PCollectionNode<T>
        where T : notnull
    {
        internal PCollection<T>? list;
        internal PCollectionNode<T>? prev;
        internal PCollectionNode<T>? next;

        internal T value;

        [NotNull]
        public T Value => value;

        internal PCollectionNode(PCollection<T> list, T value)
        {
            this.list = list;
            this.value = value;
        }

        internal PCollectionNode<T>? Next => next is null || next == list!.head ? null : next;
        internal PCollectionNode<T>? Previous => prev is null || this == list!.head ? null : prev;


        public void Remove()
        {
            if (list is null)
                throw new InvalidOperationException("Unnamed PCollectionNode");

            list!.Remove(this);
        }

        public void Update([NotNull] T value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (list is null)
                throw new InvalidOperationException("Unnamed PCollectionNode");

            var _version = list!.version;
            var _value = this.value;

            this.value = value;
            list!.version++;

            list!.CreateSnapshot(() =>
            {
                this.value = _value;
                list!.version = _version;
            });
        }

        internal void Invalidate()
        {
            list = null;
            next = null;
            prev = null;
        }
    }
}
