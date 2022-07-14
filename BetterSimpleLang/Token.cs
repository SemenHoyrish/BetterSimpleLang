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

        OpenParenthesis,
        CloseParenthesis,
        OpenCurlyBracket,
        CloseCurlyBracket,
        OpenSquareBracket,
        CloseSquareBracket,

        Dollar
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
        };

        public static Dictionary<TokenKind, int> OperatorsPriority = new Dictionary<TokenKind, int>
        {
            { TokenKind.Plus, 0 },
            { TokenKind.Minus, 0 },
            { TokenKind.Star, 1 },
            { TokenKind.Slash, 1 },
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
