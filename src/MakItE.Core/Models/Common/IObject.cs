using System;

namespace MakItE.Core.Models.Common
{
    public interface IObject
    {
        T As<T>() where T : IObject
        {
            if (this is T v)
            {
                return v;
            }
            throw new InvalidCastException($"The cast from {GetType().Name} to {typeof(T).Name} is invalid");
        }

        static PColor NewColor(decimal v1, decimal v2, decimal v3, PdxColorKind kind) => new(v1, v2, v3, kind);

        static PComment NewComment(params string[] values) => new(values);
        static PComment NewComment(IEnumerable<string> values) => new(values);

        static PDate NewDate(DateOnly value) => new PDate(value);
        static PDate NewDate(DateTime value) => new PDate(DateOnly.FromDateTime(value));
        static PDate NewDate(int year, int month, int day) => new PDate(new DateOnly(year, month, day));

        static PExpression NewExpression(string value) => new PExpression(value);

        static PLabel NewLabel(string value) => new PLabel(value);

        static PList NewList(params IObject[] nodes) => new(nodes);
        static PList NewList(IEnumerable<IObject> nodes) => new(nodes);
        static PList NewList() => new(Enumerable.Empty<IObject>());

        static PMatch NewMatch(IObject valuel, IObject valuer, PdxCompareKind opt = PdxCompareKind.Equal) => new(valuel, valuer, opt);

        static PNode NewNode(IObject key, IObject value) => new PNode(key, value);

        static PNumber NewNumber(decimal value, PdxNumberKind kind = PdxNumberKind.None) => new(value, kind);

        static PString NewString(string value) => new(value);

        static PVariable NewVariable(string value) => new(value);
    }
}
