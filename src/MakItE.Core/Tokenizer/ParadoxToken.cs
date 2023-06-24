using Superpower.Display;

namespace MakItE.Core.Tokenizer
{
    public enum ParadoxToken
    {
        [Token(Category = "object", Description = "Comment")]
        Comment,
        [Token(Category = "object", Description = "Expression")]
        Expression,
        [Token(Category = "object", Description = "Variable")]
        Variable,
        [Token(Category = "object", Description = "String")]
        String,
        [Token(Category = "object", Description = "Number")]
        Number,
        [Token(Category = "object", Description = "Date")]
        Date,
        [Token(Category = "object", Description = "Label")]
        Label,

        [Token(Category = @"syntax", Description = "Value comparison operator")]
        ComparisonSign,
        [Token(Category = @"syntax", Description = "Value set operator")]
        SetSign,
        [Token(Category = @"syntax", Description = "Symbol {")]
        LBracket,
        [Token(Category = @"syntax", Description = "Symbol }")]
        RBracket,
        [Token(Category = @"syntax", Description = "Symbol [")]
        LSquareBracket,
        [Token(Category = @"syntax", Description = "Symbol ]")]
        RSquareBracket
    }
}
