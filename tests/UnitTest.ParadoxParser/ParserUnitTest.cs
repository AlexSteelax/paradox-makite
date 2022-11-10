using MakItE.Core.Models;
using MakItE.Core.Parser;
using MakItE.Core.Tokenizer;
using Superpower;

namespace UnitTest.ParadoxParser
{
    public class ParserUnitTest
    {
        [Theory]
        [InlineData(@"""my string""")]
        [InlineData(@"'my string'")]
        public void String(string text)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.PdxValue.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PdxString);
        }

        [Theory]
        [InlineData(@"5", 5, PdxNumberKind.Default)]
        [InlineData(@"5.7", 5.7, PdxNumberKind.Default)]
        [InlineData(@"5.", 5, PdxNumberKind.Default)]
        [InlineData(@".7", 0.7, PdxNumberKind.Default)]
        [InlineData(@"8%", 8, PdxNumberKind.Percent)]
        [InlineData(@"8%%", 8, PdxNumberKind.Percent)]
        public void Numeric(string text, decimal value, PdxNumberKind kind)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.PdxValue.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.True(parsed.Value is PdxNumber);
            Assert.Equal(value, (parsed.Value as PdxNumber?)!.Value.Value);
            Assert.Equal(kind, (parsed.Value as PdxNumber?)!.Value.Kind);
        }

        [Theory]
        [InlineData(@"101.5.21")]
        public void Date(string text)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.PdxValue.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.NotNull(parsed.Value as PdxDate?);
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

            var parsed = TokenListParser.PdxValue.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.NotNull(parsed.Value as PdxLabel?);
        }

        [Theory]
        [InlineData("@[value + 20]")]
        [InlineData("@\\[value + 20]")]
        public void Expression(string text)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.PdxValue.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.NotNull(parsed.Value as PdxExpression?);
        }

        [Theory]
        [InlineData(@"@my_var")]
        public void Variable(string text)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);

            var parsed = TokenListParser.PdxValue.TryParse(tokens.Value);

            Assert.True(parsed.HasValue);

            Assert.NotNull(parsed.Value as PdxVariable?);
        }
    }
}
