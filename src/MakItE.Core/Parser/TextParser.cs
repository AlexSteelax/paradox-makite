using MakItE.Core.Models;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System.Globalization;

namespace MakItE.Core.Parser
{
    //More accurate parsers
    internal static class TextParser
    {
        internal static TextParser<IValue> DoubleQuotedString { get; } =
            from open in Character.EqualTo('"')
            from chars in Character.ExceptIn(open, '\\')
                .Or(Character.EqualTo('\\')
                    .IgnoreThen(
                        Character.EqualTo('\\')
                        .Or(Character.EqualTo(open))
                        .Or(Character.EqualTo('/'))
                        .Or(Character.EqualTo('b').Value('\b'))
                        .Or(Character.EqualTo('f').Value('\f'))
                        .Or(Character.EqualTo('n').Value('\n'))
                        .Or(Character.EqualTo('r').Value('\r'))
                        .Or(Character.EqualTo('t').Value('\t'))
                        .Named("escape sequence")))
                .Many()
            from close in Character.EqualTo(open)
            select (IValue)new PdxString(new string(chars));

        internal static TextParser<IValue> SingleQuotedString { get; } =
            from open in Character.EqualTo('\'')
            from chars in Character.Except(open).Many()
            from close in Character.EqualTo(open)
            select (IValue)new PdxString(new string(chars));

        internal static TextParser<IValue> Number { get; } =
            from sign in Character.EqualTo('-').Optional()
            from whole in Numerics.Natural.Optional().Select(n => n?.ToStringValue() ?? "0")
            from frac in Character.EqualTo('.').IgnoreThen(Numerics.Natural.Optional().Select(n => n?.ToStringValue() ?? "0")).OptionalOrDefault("0")
            from kind in Character.EqualTo('%').AtLeastOnce().Value(PdxNumberKind.Percent).OptionalOrDefault(PdxNumberKind.Default)
            let value = decimal.Parse($"{sign}{whole}.{frac}", CultureInfo.InvariantCulture)
            select (IValue)new PdxNumber(value, kind);

        internal static TextParser<IValue> Date { get; } =
            from year in Numerics.Natural.Select(s => int.Parse(s.ToStringValue()))
            from _ in Character.EqualTo('.')
            from month in Numerics.Natural.Select(s => int.Parse(s.ToStringValue()))
            from __ in Character.EqualTo('.')
            from day in Numerics.Natural.Select(s => int.Parse(s.ToStringValue()))
            select (IValue)new PdxDate(year, month, day);

        internal static TextParser<IValue> Label { get; } =
            from content in Character.ExceptIn(' ', '\\', '"').AtLeastOnce()
            select (IValue)new PdxLabel(new string(content));

        internal static TextParser<IValue> Expression =>
            from open in Span.EqualTo("@[").Try().Or(Span.EqualTo("@\\["))
            from content in Character.Except(']').Many()
            from end in Character.EqualTo(']')
            select (IValue)new PdxExpression(new string(content));

        internal static TextParser<IValue> Variable =>
            from open in Character.EqualTo('@')
            from content in Character.LetterOrDigit.Or(Character.EqualTo('_')).Many()
            select (IValue)new PdxVariable(new string(content));
    }
}
