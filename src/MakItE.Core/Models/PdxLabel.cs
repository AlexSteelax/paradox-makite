namespace MakItE.Core.Models
{
    public readonly record struct PdxLabel : IKey, IValue, INode
    {
        public readonly string Value { get; }
        public PdxLabel(string value) => Value = value;
    }
}
