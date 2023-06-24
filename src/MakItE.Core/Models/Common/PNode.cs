namespace MakItE.Core.Models.Common
{
    /// <summary>
    /// Example:
    ///     @var = 5
    ///     my_label = { ... }
    ///     5 = { ... }
    /// </summary>
    public sealed class PNode : IObject
    {
        public readonly IObject Key;
        public readonly IObject Value;
        
        internal PNode(IObject key, IObject value) => (Key, Value) = (key, value);

        static void ValidateAbleAttribute<T, TAttribute>(T value)
            where T: IObject
            where TAttribute : Attribute
        {
            var tbase = typeof(T);

            if (tbase.IsInterface)
                tbase = value.GetType();

            var state = Attribute.IsDefined(tbase, typeof(TAttribute));

            if (!state)
            {
                throw new InvalidOperationException($"Object is not marked with {typeof(TAttribute).Name}");
            }
        }

        #region Operators overloading
        public static bool operator ==(PNode lhs, PNode rhs) => lhs.Key == rhs.Key && lhs.Value == rhs.Value;
        public static bool operator !=(PNode lhs, PNode rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => obj is PNode v && v == this;
        public override int GetHashCode() => (Key, Value).GetHashCode();

        #endregion
    }
}