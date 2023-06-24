using MakItE.Core.Models;
using MakItE.Core.Services;

namespace UnitTest.ParadoxParser
{
    public class PCollectionTest
    {
        //new value
        const int NV = 100;

        [Fact]
        public void Enumerate()
        {
            var memento = new Memento();
            var original = new int[] { 1, 2, 3, 4 };

            var o = new PCollection<int>(memento, original);

            Assert.Equal(original, o.List);
            Assert.Equal(0, memento.QueueCount);
        }

        [Theory]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, NV, 2, 3 }, 1)]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, 2, NV, 3 }, 2)]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3, NV }, 3)]
        [InlineData(new int[] { 1 }, new int[] { 1, NV }, 1)]
        public void AddAfter(int[] original, int[] modified, int point)
        {
            var memento = new Memento();
            var o = new PCollection<int>(memento, original);
            var version = o.version;
            var node = o.FirstOrDefault(v => v.Value == point);

            Assert.NotNull(node);

            o.AddAfter(node, NV);

            Assert.Equal(modified, o.List);

            memento.Rollback();

            Assert.Equal(original, o.List);
            Assert.Equal(version, o.version);
            Assert.Equal(0, memento.QueueCount);
        }

        [Theory]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { NV, 1, 2, 3 }, 1)]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, NV, 2, 3 }, 2)]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, 2, NV, 3 }, 3)]
        [InlineData(new int[] { 1 }, new int[] { NV, 1 }, 1)]
        public void AddBefore(int[] original, int[] modified, int point)
        {
            var memento = new Memento();
            var o = new PCollection<int>(memento, original);
            var version = o.version;
            var node = o.FirstOrDefault(v => v.Value == point);

            Assert.NotNull(node);

            o.AddBefore(node, NV);

            Assert.Equal(modified, o.List);

            memento.Rollback();

            Assert.Equal(original, o.List);
            Assert.Equal(version, o.version);
            Assert.Equal(0, memento.QueueCount);
        }

        [Theory]
        [InlineData(new int[] { 1, 2 }, new int[] { NV, 1, 2 })]
        [InlineData(new int[] { 1 }, new int[] { NV, 1 })]
        [InlineData(new int[] { }, new int[] { NV })]
        public void AddFirst(int[] original, int[] modified)
        {
            var memento = new Memento();
            var o = new PCollection<int>(memento, original);
            var version = o.version;

            o.AddFirst(NV);

            Assert.Equal(modified, o.List);

            memento.Rollback();

            Assert.Equal(original, o.List);
            Assert.Equal(version, o.version);
            Assert.Equal(0, memento.QueueCount);
        }

        [Theory]
        [InlineData(new int[] { 1, 2 }, new int[] { 1, 2, NV })]
        [InlineData(new int[] { 1 }, new int[] { 1, NV })]
        [InlineData(new int[] { }, new int[] { NV })]
        public void AddLast(int[] original, int[] modified)
        {
            var memento = new Memento();
            var o = new PCollection<int>(memento, original);
            var version = o.version;

            o.AddLast(NV);

            Assert.Equal(modified, o.List);

            memento.Rollback();

            Assert.Equal(original, o.List);
            Assert.Equal(version, o.version);
            Assert.Equal(0, memento.QueueCount);
        }

        [Theory]
        [InlineData(new int[] { 1, 2 }, new int[] { })]
        [InlineData(new int[] { 1 }, new int[] { })]
        [InlineData(new int[] { }, new int[] { })]
        public void Clear(int[] original, int[] modified)
        {
            var memento = new Memento();
            var o = new PCollection<int>(memento, original);
            var version = o.version;

            o.Clear();

            Assert.Equal(modified, o.List);

            memento.Rollback();

            Assert.Equal(original, o.List);
            Assert.Equal(version, o.version);
            Assert.Equal(0, memento.QueueCount);
        }

        [Theory]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { NV, 2, 3 }, 1)]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, NV, 3 }, 2)]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, 2, NV }, 3)]
        public void Update(int[] original, int[] modified, int point)
        {
            var memento = new Memento();
            var o = new PCollection<int>(memento, original);
            var version = o.version;
            var node = o.FirstOrDefault(v => v.Value == point);

            Assert.NotNull(node);

            node.Update(NV);

            Assert.Equal(modified, o.List);

            memento.Rollback();

            Assert.Equal(original, o.List);
            Assert.Equal(version, o.version);
            Assert.Equal(0, memento.QueueCount);
        }

        [Theory]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { 2, 3 }, 1)]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, 3 }, 2)]
        [InlineData(new int[] { 1, 2, 3 }, new int[] { 1, 2 }, 3)]
        [InlineData(new int[] { 1, 2 }, new int[] { 2 }, 1)]
        [InlineData(new int[] { 1, 2 }, new int[] { 1 }, 2)]
        [InlineData(new int[] { 1 }, new int[] { }, 1)]
        public void Remove(int[] original, int[] modified, int point)
        {
            var memento = new Memento();
            var o = new PCollection<int>(memento, original);
            var version = o.version;
            var node = o.FirstOrDefault(v => v.Value == point);

            Assert.NotNull(node);

            node.Remove();

            Assert.Equal(modified, o.List);

            memento.Rollback();

            Assert.Equal(original, o.List);
            Assert.Equal(version, o.version);
            Assert.Equal(0, memento.QueueCount);
        }

        [Fact]
        public void RemoveRepeat()
        {
            var memento = new Memento();
            var o = new PCollection<int>(memento);
            var node = o.AddFirst(1);

            node.Remove();

            Assert.Throws<InvalidOperationException>(node.Remove);
            Assert.Equal(2, memento.QueueCount);
        }

        [Fact]
        public void UpdateRemoved()
        {
            var memento = new Memento();
            var o = new PCollection<int>(memento);
            var node = o.AddFirst(1);

            node.Remove();

            Assert.Throws<InvalidOperationException>(() => node.Update(2));
            Assert.Equal(2, memento.QueueCount);
        }
    }
}
