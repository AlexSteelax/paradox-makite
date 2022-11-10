namespace MakItE.Core.Models
{
    public enum PdxNumberKind
    {
        Default,
        Percent
    }
    public readonly record struct PdxNumber : IKey, IValue, INode
    {
        public readonly decimal Value { get; }
        public readonly PdxNumberKind Kind { get; }
        public PdxNumber(decimal value, PdxNumberKind kind = PdxNumberKind.Default) => (Value, Kind) = (value, kind);
        public static PdxNumber Zero() => new(decimal.Zero, PdxNumberKind.Default);
        /*
        public static bool operator ==(PdxNumber lhs, decimal rhs) => lhs.Value == rhs;
        public static bool operator !=(PdxNumber lhs, decimal rhs) => lhs.Value != rhs;
        public static bool operator >(PdxNumber lhs, decimal rhs) => lhs.Value > rhs;
        public static bool operator <(PdxNumber lhs, decimal rhs) => lhs.Value < rhs;
        public static bool operator >=(PdxNumber lhs, decimal rhs) => lhs.Value >= rhs;
        public static bool operator <=(PdxNumber lhs, decimal rhs) => lhs.Value <= rhs;

        public static bool operator ==(PdxNumber lhs, PdxNumberKind rhs) => lhs.Kind == rhs;
        public static bool operator !=(PdxNumber lhs, PdxNumberKind rhs) => lhs.Kind != rhs;
        */
    }
}