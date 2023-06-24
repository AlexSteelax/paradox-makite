using MakItE.Core.Models;
using MakItE.Core.Models.Common;
using MakItE.Core.Tokenizer;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System.Diagnostics.CodeAnalysis;
using static MakItE.Core.Parser.TextParser;
using static MakItE.Core.Models.Common.IObject;

namespace MakItE.Core.Parser
{
    public static class TokenListParser
    {
        internal static TokenListParser<ParadoxToken, PString> TPString { get; } = Token.EqualTo(ParadoxToken.String).Apply(StringParser);
        internal static TokenListParser<ParadoxToken, PExpression> TPExpression { get; } = Token.EqualTo(ParadoxToken.Expression).Apply(ExpressionParser);
        internal static TokenListParser<ParadoxToken, PVariable> TPVariable { get; } = Token.EqualTo(ParadoxToken.Variable).Apply(VariableParser);
        internal static TokenListParser<ParadoxToken, PNumber> TPNumber { get; } = Token.EqualTo(ParadoxToken.Number).Apply(NumberParser);
        internal static TokenListParser<ParadoxToken, PLabel> TPLabel { get; } = Token.EqualTo(ParadoxToken.Label).Apply(LabelParser);

        internal static TokenListParser<ParadoxToken, PDate> TPDate { get; } = Token.EqualTo(ParadoxToken.Date).Apply(DateParser);

        internal static TokenListParser<ParadoxToken, PColor> TPColor { get; } =
            from ckind in Token.EqualTo(ParadoxToken.Label).Apply(ColorKindParser)
            from begin in Token.EqualTo(ParadoxToken.LBracket)
            from values in Parse.Ref(() => Token.EqualTo(ParadoxToken.Number).Apply(NumberParser).Where(v => v == PdxNumberKind.None).Repeat(3))
            from end in Token.EqualTo(ParadoxToken.RBracket)
            select NewColor(values[0].Value, values[1].Value, values[2].Value, ckind);

        internal static TokenListParser<ParadoxToken, PList> TPList { get; } =
            from begin in Token.EqualTo(ParadoxToken.LBracket)
            from items in Parse.Ref(() => TPAnyNode!.Many())
            from end in Token.EqualTo(ParadoxToken.RBracket)
            select NewList(items);

        internal static TokenListParser<ParadoxToken, IObject> TKeyNode { get; } =
            Parse.OneOf(
                //Fix quoted value used for key
                TPString.Where(s => s != string.Empty).Select(s => (IObject)NewLabel(s.Value)),

                TPVariable.Select(s => (IObject)s),
                TPNumber.Select(s => (IObject)s),
                TPDate.Select(s => (IObject)s),
                TPLabel.Select(s => (IObject)s));

        internal static TokenListParser<ParadoxToken, IObject> TValueNode { get; } =
            Parse.OneOf(
                TPString.Select(s => (IObject)s),
                TPExpression.Select(s => (IObject)s),
                TPVariable.Select(s => (IObject)s),
                TPNumber.Select(s => (IObject)s),
                TPDate.Select(s => (IObject)s),
                TPColor.Select(s => (IObject)s).Try(),
                TPLabel.Select(s => (IObject)s),
                TPList.Select(s => (IObject)s));

        internal static TokenListParser<ParadoxToken, IObject> TLeftMatchNode { get; } =
            TPLabel.Select(s => (IObject)s);

        internal static TokenListParser<ParadoxToken, IObject> TRightMatchNode { get; } =
            Parse.OneOf(
                TPString.Select(s => (IObject)s),
                TPExpression.Select(s => (IObject)s),
                TPVariable.Select(s => (IObject)s),
                TPNumber.Select(s => (IObject)s),
                TPDate.Select(s => (IObject)s),
                TPLabel.Select(s => (IObject)s),
                TPList.Select(s => (IObject)s));

        internal static TokenListParser<ParadoxToken, PComment> TPComment { get; } = Token
            .EqualTo(ParadoxToken.Comment)
            .Apply(CommentParser)
            .AtLeastOnce()
            .Select(v => NewComment(v));

        internal static TokenListParser<ParadoxToken, PMatch> TPMatch { get; } =
            from value1 in TLeftMatchNode
            from sign in Token.EqualTo(ParadoxToken.ComparisonSign).Apply(ComparisonSignParser)
            from value2 in TRightMatchNode
            select NewMatch(value1, value2, sign);

        internal static TokenListParser<ParadoxToken, PNode> TPNode { get; } =
            from key in TKeyNode
            from _ in Token.EqualTo(ParadoxToken.SetSign)
            from value in TValueNode
            select NewNode(key, value);

        internal static TokenListParser<ParadoxToken, IObject> TPAnyNode { get; } =
          Parse.OneOf(
              TPComment.Select(s => (IObject)s).Try(),
              TPNode.Select(s => (IObject)s).Try(),
              TPMatch.Select(s => (IObject)s).Try(),
              TPList.Select(s => (IObject)s),
              TPString.Select(s => (IObject)s),
              TPExpression.Select(s => (IObject)s),
              TPVariable.Select(s => (IObject)s),
              TPNumber.Select(s => (IObject)s),
              TPDate.Select(s => (IObject)s),
              TPLabel.Select(s => (IObject)s));

        internal static TokenListParser<ParadoxToken, IObject[]> TPDocument { get; } =
            TPAnyNode.Many().AtEnd();

        public static bool TryParse(string document, out IEnumerable<IObject>? result, [MaybeNullWhen(true)] out string error, out Position errorPosition)
        {
            var tokens = TokenParser.Instance.TryTokenize(document);
            if (!tokens.HasValue)
            {
                result = null;
                error = tokens.ToString();
                errorPosition = tokens.ErrorPosition;
                return false;
            }

            var parsed = TPDocument.TryParse(tokens.Value);
            if (!parsed.HasValue)
            {
                result = null;
                error = parsed.ToString();
                errorPosition = parsed.ErrorPosition;
                return false;
            }

            result = parsed.Value;
            error = null;
            errorPosition = Position.Empty;
            return true;
        }
    }
}