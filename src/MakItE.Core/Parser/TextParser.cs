using MakItE.Core.Models.Common;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System.Globalization;
using static MakItE.Core.Models.Common.IObject;

namespace MakItE.Core.Parser
{
    //More accurate parsers
    internal static class TextParser
    {
        static TextParser<PString> DoubleQuotedStringParser { get; } =
            from open in Character.EqualTo('"').Named("open")
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
            from close in Character.EqualTo(open).Named("close")
            select NewString(new string(chars));

        static TextParser<PString> SingleQuotedStringParser { get; } =
            from open in Character.EqualTo('\'')
            from chars in Character.Except(open).Many()
            from close in Character.EqualTo(open)
            select NewString(new string(chars));

        internal static TextParser<PString> StringParser { get; } =
            Parse.OneOf(DoubleQuotedStringParser, SingleQuotedStringParser);

        internal static TextParser<PNumber> NumberParser { get; } =
            from sign in Character.EqualTo('-').Optional()
            from whole in Numerics.Natural.Optional().Select(n => n?.ToStringValue() ?? "0")
            from frac in Character.EqualTo('.').IgnoreThen(Numerics.Natural.Optional().Select(n => n?.ToStringValue() ?? "0")).OptionalOrDefault("0")
            from kind in Character.EqualTo('%').AtLeastOnce().Value(PdxNumberKind.Percent).OptionalOrDefault(PdxNumberKind.None)
            let value = decimal.Parse($"{sign}{whole}.{frac}", CultureInfo.InvariantCulture)
            select NewNumber(value, kind);

        internal static TextParser<PDate> DateParser { get; } =
            from year in Numerics.Natural.Select(v => int.Parse(v.ToStringValue()))
            from _ in Character.EqualTo('.')
            from month in Numerics.Natural.Select(v => int.Parse(v.ToStringValue()))
            from __ in Character.EqualTo('.')
            from day in Numerics.Natural.Select(v => int.Parse(v.ToStringValue()))
            select NewDate(year, month, day);

        internal static TextParser<PLabel> LabelParser { get; } =
            from content in Character.ExceptIn(' ', '\\', '"').AtLeastOnce()
            select NewLabel(new string(content));

        internal static TextParser<PExpression> ExpressionParser =>
            from open in Span.EqualTo("@[").Try().Or(Span.EqualTo("@\\["))
            from content in Character.Except(']').Many()
            from end in Character.EqualTo(']')
            select NewExpression(new string(content));

        internal static TextParser<PVariable> VariableParser =>
            from open in Character.EqualTo('@')
            from content in Character.LetterOrDigit.Or(Character.EqualTo('_')).Many()
            select NewVariable(new string(content));

        internal static TextParser<PdxCompareKind> ComparisonSignParser =>
            from sign in Character.In('=', '>', '<', '!').AtLeastOnce().Select(s => new string(s))
            let comparisonSign = sign switch
            {
                "==" => PdxCompareKind.Equal,
                "!=" => PdxCompareKind.NotEqual,
                ">=" => PdxCompareKind.GreaterOrEqual,
                "<=" => PdxCompareKind.LessOrEqual,
                ">" => PdxCompareKind.Greater,
                "<" => PdxCompareKind.Less,
                //Never call
                _ => throw new ArgumentException(nameof(PdxCompareKind), $"Not expected value: {sign}")
            }
            select comparisonSign;

        internal static TextParser<string> CommentParser =>
            from begin in Character.EqualTo('#')
            from msg in Character.AnyChar.Many()
            select new string(msg).TrimEnd();

        internal static TextParser<PdxColorKind> ColorKindParser =>
            Span.EqualToIgnoreCase("rgb").Value(PdxColorKind.RGB).Try()
            .Or(Span.EqualToIgnoreCase("hsv").Value(PdxColorKind.HSV));
    }
}
