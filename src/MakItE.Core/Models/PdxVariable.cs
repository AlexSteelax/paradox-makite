namespace MakItE.Core.Models
{
    public readonly record struct PdxVariable : IValue, INode
    {
        public readonly string Value { get; }
        public PdxVariable(string value) => Value = value;
    }
}