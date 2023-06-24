using MakItE.Core.Models.Markers;

namespace MakItE.Core.Models.Common
{
    /// <summary>
    /// Example:
    ///     my_label$V1|2$_suffix
    /// </summary>
    [Keyable, Valueable, LeftMatchable, RightMatchable]
    public sealed class PLabel : IObject
    {
        public readonly string Value;

        internal PLabel(string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            Value = value;
        }

        #region Operators overloading
        public static bool operator ==(PLabel lhs, string rhs) => lhs.Value == rhs;
        public static bool operator !=(PLabel lhs, string rhs) => lhs.Value != rhs;
        public static bool operator ==(PLabel lhs, PLabel rhs) => lhs.Value == rhs.Value;
        public static bool operator !=(PLabel lhs, PLabel rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => obj is PLabel v && v == this;
        public override int GetHashCode() => Value.GetHashCode();
        #endregion
    }
}
