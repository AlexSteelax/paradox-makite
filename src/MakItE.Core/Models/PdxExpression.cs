namespace MakItE.Core.Models
{
    public readonly record struct PdxExpression : IValue, INode
    {
        public readonly string Value { get; }
        public PdxExpression(string value) => Value = value;
    }
}
