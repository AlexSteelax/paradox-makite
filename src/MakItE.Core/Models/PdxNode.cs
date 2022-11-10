namespace MakItE.Core.Models
{
    public enum PdxNodeKind
    {
        Set,
        Equal,
        NotEqual,
        Less,
        Greater,
        LessOrEqual,
        GreaterOrEqual
    }
    public sealed record PdxNode: INode
    {
        public IKey Key { get; init; }
        public IValue Value { get; init; }
        public PdxNodeKind Kind { get; init; }

        public PdxNode(IKey key, IValue value, PdxNodeKind kind = PdxNodeKind.Set) => (Key, Value, Kind) = (key, value, kind);
    }
}