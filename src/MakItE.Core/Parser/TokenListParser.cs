using MakItE.Core.Models;
using MakItE.Core.Tokenizer;
using Superpower;
using Superpower.Parsers;

namespace MakItE.Core.Parser
{
    public static class TokenListParser
    {
        static TokenListParser<ParadoxToken, IValue> PdxString { get; } =
            Token.EqualTo(ParadoxToken.String)
                .Apply(TextParser.DoubleQuotedString.Try().Or(TextParser.SingleQuotedString));

        static TokenListParser<ParadoxToken, IValue> PdxNumber { get; } =
            Token.EqualTo(ParadoxToken.Value)
                .Apply(TextParser.Number);

        static TokenListParser<ParadoxToken, IValue> PdxDate { get; } =
            Token.EqualTo(ParadoxToken.Value)
                .Apply(TextParser.Date);

        static TokenListParser<ParadoxToken, IValue> PdxLabel { get; } =
            Token.EqualTo(ParadoxToken.Value)
                .Apply(TextParser.Label);

        static TokenListParser<ParadoxToken, IValue> PdxExpression { get; } =
            Token.EqualTo(ParadoxToken.Expression)
                .Apply(TextParser.Expression);

        static TokenListParser<ParadoxToken, IValue> PdxVariable { get; } =
            Token.EqualTo(ParadoxToken.Variable)
                .Apply(TextParser.Variable);

        public static TokenListParser<ParadoxToken, IValue> PdxValue { get; } =
            PdxString
            .Or(PdxExpression)
            .Or(PdxVariable)
            .Or(PdxNumber).Try().Or(PdxDate).Try().Or(PdxLabel);
        
        public static TokenListParser<ParadoxToken, IValue> PdxNode { get; } =
            from key in Token.EqualTo(ParadoxToken.Value).Or(Token.EqualTo(ParadoxToken.Variable))
            //from opr in Token.EqualTo(ParadoxToken.SetSign)//.Or(Token.EqualTo(ParadoxToken.ComparisonSign))
            select (IValue)null;
        
    }
}