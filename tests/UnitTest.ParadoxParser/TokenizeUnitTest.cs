using MakItE.Core.Tokenizer;
using static System.Net.Mime.MediaTypeNames;

namespace UnitTest.ParadoxParser
{
    public class TokenizeUnitTest
    {
        [Theory]
        [InlineData(@"# My comment")]
        public void Comment(string text)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);
            Assert.Single(tokens.Value);

            var value = tokens.Value.ElementAt(0);

            Assert.Equal(ParadoxToken.Comment, value.Kind);
            Assert.Equal(text, value.ToStringValue());
        }

        [Theory]
        [InlineData("@[value + 20]")]
        [InlineData("@\\[value + 20]")]
        public void Expression(string text)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);
            Assert.Single(tokens.Value);

            var value = tokens.Value.ElementAt(0);
            Assert.Equal(ParadoxToken.Expression, value.Kind);
            Assert.Equal(text, value.ToStringValue());
        }

        [Theory]
        [InlineData(@"@my_var")]
        public void Variable(string text)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);
            Assert.Single(tokens.Value);

            var value = tokens.Value.ElementAt(0);
            Assert.Equal(ParadoxToken.Variable, value.Kind);
            Assert.Equal(text, value.ToStringValue());
        }

        [Theory]
        [InlineData(@"""my \"" text""")]
        [InlineData(@"'F'")]
        [InlineData(@"'my text'")]
        public void String(string text)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);
            Assert.Single(tokens.Value);

            var value = tokens.Value.ElementAt(0);

            Assert.Equal(ParadoxToken.String, value.Kind);
            Assert.Equal(text, value.ToStringValue());
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
            Assert.Single(tokens.Value);

            var value = tokens.Value.ElementAt(0);

            Assert.Equal(ParadoxToken.Label, value.Kind);
            Assert.Equal(text, value.ToStringValue());
        }

        [Theory]
        [InlineData("-0")]
        [InlineData("-0.5")]
        [InlineData("5.6")]
        [InlineData(".8")]
        public void Number(string text)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);
            Assert.Single(tokens.Value);

            var value = tokens.Value.ElementAt(0);

            Assert.Equal(ParadoxToken.Number, value.Kind);
            Assert.Equal(text, value.ToStringValue());
        }

        [Theory]
        [InlineData("999.5.6")]
        [InlineData("00.00.00")]
        public void Date(string text)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);
            Assert.Single(tokens.Value);

            var value = tokens.Value.ElementAt(0);

            Assert.Equal(ParadoxToken.Date, value.Kind);
            Assert.Equal(text, value.ToStringValue());
        }

        [Theory]
        [InlineData("==", ParadoxToken.ComparisonSign)]
        [InlineData("!=", ParadoxToken.ComparisonSign)]
        [InlineData(">", ParadoxToken.ComparisonSign)]
        [InlineData("<", ParadoxToken.ComparisonSign)]
        [InlineData(">=", ParadoxToken.ComparisonSign)]
        [InlineData("<=", ParadoxToken.ComparisonSign)]
        [InlineData("=", ParadoxToken.SetSign)]
        [InlineData("{", ParadoxToken.LBracket)]
        [InlineData("}", ParadoxToken.RBracket)]
        [InlineData("[", ParadoxToken.LSquareBracket)]
        [InlineData("]", ParadoxToken.RSquareBracket)]
        public void Syntax(string text, ParadoxToken kind)
        {
            var tokens = TokenParser.Instance.TryTokenize(text);

            Assert.True(tokens.HasValue);
            Assert.Single(tokens.Value);

            var value = tokens.Value.ElementAt(0);
            Assert.Equal(kind, value.Kind);
            Assert.Equal(text, value.ToStringValue());
        }
    }
}