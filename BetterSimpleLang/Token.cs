using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BetterSimpleLang
{
    public enum TokenKind
    {
        Name,
        Number,
        String,

        Colon,
        ColonColon,
        Semicolon,
        Comma,

        Plus,
        Minus,
        Star,
        Slash,
        Equals,
        EqualsEquals,
        Bigger,
        Less,

        OpenParenthesis,
        CloseParenthesis,
        OpenCurlyBracket,
        CloseCurlyBracket,
        OpenSquareBracket,
        CloseSquareBracket,

        Dollar,
        Arrow
    }

    public class Token
    {
        public TokenKind kind;
        public string text;

        public Token() { }
        public Token(TokenKind k, string t)
        {
            kind = k;
            text = t;
        }

        public static List<TokenKind> OperatorsTokenKinds = new List<TokenKind>
        {
            TokenKind.Plus,
            TokenKind.Minus,
            TokenKind.Star,
            TokenKind.Slash,
            TokenKind.Equals,
            TokenKind.EqualsEquals,
            TokenKind.Bigger,
            TokenKind.Less,
            TokenKind.Arrow,
        };

        public static Dictionary<TokenKind, int> OperatorsPriority = new Dictionary<TokenKind, int>
        {
            { TokenKind.Equals, 0 },
            { TokenKind.Bigger, 1 },
            { TokenKind.Less, 1 },
            { TokenKind.EqualsEquals, 1 },
            { TokenKind.Plus, 2 },
            { TokenKind.Minus, 2 },
            { TokenKind.Star, 3 },
            { TokenKind.Slash, 3 },
            { TokenKind.Arrow, 4 },
            //{ TokenKind.Equals,
            //{ TokenKind.EqualsEquals,
        };
    }

    public class TokenComparer : IComparer<KeyValuePair<Token, int[]>>
    {
        public int Compare([AllowNull] KeyValuePair<Token, int[]> x, [AllowNull] KeyValuePair<Token, int[]> y) =>
            (Token.OperatorsPriority[y.Key.kind] + y.Value[1]) - (Token.OperatorsPriority[x.Key.kind] + x.Value[1]);
    }
}
