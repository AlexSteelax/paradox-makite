namespace MakItE.Core.Models
{
    public readonly record struct PdxString : IValue, INode
    {
        public readonly string Value { get; }
        public PdxString(string value) => Value = value;
        public static PdxString Empty() => new();
    }
}
