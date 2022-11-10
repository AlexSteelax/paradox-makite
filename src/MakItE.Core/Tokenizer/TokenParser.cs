using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace MakItE.Core.Tokenizer
{
    //Fast and very dirty parsers
    public static class TokenParser
    {
        static TextParser<Unit> ExpressionToken =>
            from open in Span.EqualTo("@[").Try().Or(Span.EqualTo("@\\["))
            from content in Character.Except(']').IgnoreMany()
            from end in Character.EqualTo(']')
            select Unit.Value;

        static TextParser<Unit> VariableToken =>
            from open in Character.EqualTo('@')
            from content in Character.LetterOrDigit.Or(Character.EqualTo('_')).IgnoreMany()
            select Unit.Value;

        static TextParser<Unit> StringToken { get; } =
            from open in Character.In('"', '\'')
            from content in Character.EqualTo('\\').IgnoreThen(Character.AnyChar).Value(Unit.Value).Try()
                .Or(Character.Except(open).Value(Unit.Value))
                .IgnoreMany()
            from close in Character.EqualTo(open)
            select Unit.Value;

        static TextParser<Unit> ValueToken =>
            from context in Character.LetterOrDigit.Or(Character.In('_', '&', '-', '\'', '$', '.', '|', '*', ':', '%', '/')).IgnoreMany()
            select Unit.Value;

        static TextParser<Unit> ComparisonToken =>
            Span.EqualTo("==").Value(Unit.Value).Try().Or(Character.In('!', '>', '<').IgnoreThen(Character.EqualTo('=').Optional()).Value(Unit.Value));

        static TokenParser() => Instance = new TokenizerBuilder<ParadoxToken>()

            //Ignore UTF-8 BOM symbols
            .Ignore(Character.In('\uFEFF', '\u200B'))
            .Ignore(Span.WhiteSpace)
            .Ignore(Character.In(';', ','))

            .Match(Comment.ShellStyle, ParadoxToken.Comment)

            .Match(ComparisonToken, ParadoxToken.ComparisonSign)
            .Match(Character.EqualTo('='), ParadoxToken.SetSign)
            .Match(Character.EqualTo('{'), ParadoxToken.LBracket)
            .Match(Character.EqualTo('}'), ParadoxToken.RBracket)
            .Match(Character.EqualTo('['), ParadoxToken.LSquareBracket)
            .Match(Character.EqualTo(']'), ParadoxToken.RSquareBracket)

            .Match(StringToken, ParadoxToken.String)
            .Match(ExpressionToken, ParadoxToken.Expression, requireDelimiters: true)
            .Match(VariableToken, ParadoxToken.Variable, requireDelimiters: true)
            .Match(ValueToken, ParadoxToken.Value, requireDelimiters: true)

            .Build();

        public static Tokenizer<ParadoxToken> Instance { get; private set; }
    }
}
