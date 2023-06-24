namespace MakItE.Core.Models.Common
{
    public enum PdxCompareKind
    {
        Equal,
        NotEqual,
        Less,
        Greater,
        LessOrEqual,
        GreaterOrEqual
    }
    /// <summary>
    /// Example:
    ///     5 > 4
    ///     label2 == "some string"
    /// </summary>
    public sealed class PMatch : IObject
    {
        public readonly IObject ValueL;
        public readonly IObject ValueR;
        public readonly PdxCompareKind Operator;

        internal PMatch(IObject valuel, IObject valuer, PdxCompareKind opt) => (ValueL, ValueR, Operator) = (valuel, valuer, opt);

        #region Operators overloading
        public static bool operator ==(PMatch lhs, PMatch rhs) => lhs.ValueL == rhs.ValueL && lhs.ValueR == rhs.ValueR && lhs.Operator == rhs.Operator;
        public static bool operator !=(PMatch lhs, PMatch rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => obj is PMatch v && v == this;
        public override int GetHashCode() => (ValueL, ValueR, Operator).GetHashCode();
        #endregion
    }
}
