using MakItE.Core.Models.Common;
using MakItE.Core.Parser;
using MakItE.Core.Tokenizer;
using Superpower;
using Superpower.Model;

namespace UnitTest.ParadoxParser
{
    public class ParserUnitTest
    {
        static void CheckAnyNode<T>(Result<TokenList<ParadoxToken>> tokens)
        {
            var parsed = TokenListParser.TPAnyNode.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is T);
        }

        [Theory]
        [InlineData(@"""my string""", "my string")]
        [InlineData(@"'my string'", "my string")]
        public void String(string text, string value)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.TPString.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PString v && v == value);

            CheckAnyNode<PString>(tokens);
        }
        
        [Theory]
        [InlineData(@"05", 5, PdxNumberKind.None)]
        [InlineData(@"5", 5, PdxNumberKind.None)]
        [InlineData(@"5.7", 5.7, PdxNumberKind.None)]
        [InlineData(@"5.", 5, PdxNumberKind.None)]
        [InlineData(@".7", 0.7, PdxNumberKind.None)]
        [InlineData(@"8%", 8, PdxNumberKind.Percent)]
        [InlineData(@"8%%", 8, PdxNumberKind.Percent)]
        public void Numeric(string text, decimal value, PdxNumberKind kind)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.TPNumber.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PNumber v && v == value && v == kind);

            CheckAnyNode<PNumber>(tokens);
        }
        
        [Theory]
        [InlineData(@"101.5.21", 101, 5, 21)]
        [InlineData(@"101.05.21", 101, 5, 21)]
        public void Date(string text, int year, int month, int day)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.TPDate.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PDate v && v == new DateOnly(year, month, day));

            CheckAnyNode<PDate>(tokens);
        }
        
        [Theory]
        [InlineData(@"namespace")]
        [InlineData("scope:key")]
        [InlineData("_tag_&_q'tag")]
        [InlineData("$VARIABLE$")]
        [InlineData("-$VAR$")]
        [InlineData("tag_$VAR|5$_suffix")]
        [InlineData("*-1")]
        [InlineData(".5.10%")]
        [InlineData(@"/SFX/Events/Themes/sfx_event_theme_type_corruption")]
        [InlineData(@"patron_nbg/p_ae_syrabane_nbg.dds")]
        public void Label(string text)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.TPLabel.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PLabel v && v == text);

            CheckAnyNode<PLabel>(tokens);
        }
        
        [Theory]
        [InlineData("@[value + 20]", "value + 20")]
        [InlineData("@\\[value + 20]", "value + 20")]
        public void Expression(string text, string value)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.TPExpression.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PExpression v && v == value);

            CheckAnyNode<PExpression>(tokens);
        }
        
        [Theory]
        [InlineData(@"@my_var", "my_var")]
        public void Variable(string text, string value)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.TPVariable.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PVariable v && v == value);

            CheckAnyNode<PVariable>(tokens);
        }

        [Theory]
        [InlineData("#1", "1", 1)]
        [InlineData("#1\n#2", "1\n2", 2)]
        public void Comment(string text, string value, int size)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.TPComment.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PComment v && v.Count == size && string.Join('\n', v) == value);

            CheckAnyNode<PComment>(tokens);
        }

        [Theory]
        [InlineData("@x = 5", typeof(PVariable), typeof(PNumber))]
        [InlineData("5 = { }", typeof(PNumber), typeof(PList))]
        [InlineData("999.5.4 = { }", typeof(PDate), typeof(PList))]
        [InlineData("abcd = { 1 2 }", typeof(PLabel), typeof(PList))]
        [InlineData("child_education = { 1 2 }", typeof(PLabel), typeof(PList))]
        [InlineData("child = { add = 0.1 }", typeof(PLabel), typeof(PList))]
        [InlineData("child = { add = 0.1 if = { } }", typeof(PLabel), typeof(PList))]
        [InlineData("child = { add = 0.1 if = { age == 14 } }", typeof(PLabel), typeof(PList))]
        [InlineData("child = { add = 0.1 if = { age > 14 } }", typeof(PLabel), typeof(PList))]
        [InlineData("color = hsv{ 0.58 0.94 0.7 }", typeof(PLabel), typeof(PColor))]
        [InlineData("color = { { } }", typeof(PLabel), typeof(PList))]
        public void Node(string text, Type tkey, Type tvalue)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.TPNode.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PNode v && v.Key.GetType() == tkey && v.Value.GetType() == tvalue);

            CheckAnyNode<PNode>(tokens);
        }

        [Theory]
        [InlineData(@"abcd <= ""text""", typeof(PLabel), typeof(PString), PdxCompareKind.LessOrEqual)]
        [InlineData(@"2hh$ > 6", typeof(PLabel), typeof(PNumber), PdxCompareKind.Greater)]
        public void Matching(string text, Type tvaluel, Type tvaluer, PdxCompareKind opt)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.TPMatch.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PMatch v && v.ValueL.GetType() == tvaluel && v.ValueR.GetType() == tvaluer && v.Operator == opt);

            CheckAnyNode<PMatch>(tokens);
        }

        [Theory]
        [InlineData(@"hsv { 0.58 0.94 0.7 }", PdxColorKind.HSV)]
        [InlineData(@"rgb { 174 169 166 }", PdxColorKind.RGB)]
        public void Color(string text, PdxColorKind ckind)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.TPColor.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PColor v && v.Kind == ckind);

            //CheckAnyNode<PdxColor>(tokens);
        }
    }
}
