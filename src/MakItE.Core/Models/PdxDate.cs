namespace MakItE.Core.Models
{
    public readonly record struct PdxDate : IValue, INode
    {
        public readonly DateOnly Value { get; }

        public PdxDate(DateOnly value) => Value = value;
        public PdxDate(DateTime value) => Value = DateOnly.FromDateTime(value);
        public PdxDate(int year, int month, int day) => Value = new DateOnly(year, month, day);
    }
}