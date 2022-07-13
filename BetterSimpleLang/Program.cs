using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

    public enum ExpressionKind
    {
        Calc,
    }

    public interface IExpression
    {
        public ExpressionKind Kind();
    }

    public class CalcExpression : IExpression
    {
        public CalcExpression Left;
        public Token Operator;
        public CalcExpression Right;

        public Token Value;

        public ExpressionKind Kind() => ExpressionKind.Calc;
    }

    public class Value
    {
        public Type type;
        public object value;
    }

    public class Lexer
    {
        public Token[] GetTokens(string input)
        {
            List<Token> result = new List<Token>();

            //var chars = input.ToCharArray().GetEnumerator();
            var chars = input.ToCharArray();

            //while (chars.MoveNext())
            int i = -1;

            char current()
            {
                if(i >= 0 && i < chars.Length)
                    return chars[i];
                return Char.MinValue;
            }
            char? next()
            {
                char? r = null;
                if (i < chars.Length - 1)
                {
                    i++;
                    r = current();
                }
                return r;
            }
            char? look_next()
            {
                char? r = null;
                if (i + 1 <= chars.Length - 1)
                    r = chars[i + 1];
                return r;
            }
            while (next() != null)
            {
                switch (current())
                {
                    case ':':
                        if (look_next() == ':')
                        {
                            next();
                            result.Add(new Token(TokenKind.ColonColon, "::"));
                        }
                        else
                            result.Add(new Token(TokenKind.Colon, ":"));
                        break;

                    case ';':
                        result.Add(new Token(TokenKind.Semicolon, ";"));
                        break;

                    case '+':
                        result.Add(new Token(TokenKind.Plus, "+"));
                        break;

                    case '-':
                        result.Add(new Token(TokenKind.Minus, "-"));
                        break;

                    case '*':
                        result.Add(new Token(TokenKind.Star, "*"));
                        break;

                    case '/':
                        result.Add(new Token(TokenKind.Slash, "/"));
                        break;

                    case '=':
                        if (look_next() == '=')
                        {
                            next();
                            result.Add(new Token(TokenKind.EqualsEquals, "=="));
                        }
                        else
                            result.Add(new Token(TokenKind.Equals, "="));
                        break;

                    case ',':
                        result.Add(new Token(TokenKind.Comma, ","));
                        break;

                    case '(':
                        result.Add(new Token(TokenKind.OpenParenthesis, "("));
                        break;

                    case ')':
                        result.Add(new Token(TokenKind.CloseParenthesis, ")"));
                        break;

                    case '{':
                        result.Add(new Token(TokenKind.OpenCurlyBracket, "{"));
                        break;

                    case '}':
                        result.Add(new Token(TokenKind.CloseCurlyBracket, "}"));
                        break;

                    case '[':
                        result.Add(new Token(TokenKind.OpenSquareBracket, "["));
                        break;

                    case ']':
                        result.Add(new Token(TokenKind.CloseSquareBracket, "]"));
                        break;

                    default:
                        if (char.IsWhiteSpace(current()))
                        {
                            break;
                        }
                        if (char.IsLetter(current()))
                        {
                            string name = "";
                            name += current();
                            while(look_next() != null && char.IsLetter((char)look_next()))
                            {
                                name += next();
                            }
                            result.Add(new Token(TokenKind.Name, name));
                            break;
                        }
                        if (char.IsDigit(current()))
                        {
                            string number = "";
                            number += current();
                            while (look_next() != null && char.IsDigit((char)look_next()))
                            {
                                number += next();
                            }
                            result.Add(new Token(TokenKind.Number, number));
                            break;
                        }
                        throw new Exception("Unexpected character '" + current() + "'");
                }
            }

            return result.ToArray();
        }
    }

    public class Parser
    {
        public IExpression[] Parse(Token[] tokens)
        {
            List<IExpression> exprs = new List<IExpression>();

            List<Token> ts = new List<Token>();
            foreach(var t in tokens)
            {
                if (t.kind == TokenKind.Semicolon)
                {
                    exprs.Add(ParseCalcExpression(ts.ToArray()));
                    ts.Clear();
                }
                else
                {
                    ts.Add(t);
                }
            }

            return exprs.ToArray();
        }

        public CalcExpression ParseCalcExpression(Token[] tokens)
        {
            bool[] is_token_used = new bool[tokens.Length];
            // Token, { index, add_value };
            List<KeyValuePair<Token, int[]>> sign_tokens = new List<KeyValuePair<Token, int[]>>();
            int add = 0;
            for(int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].kind == TokenKind.OpenParenthesis)
                {
                    add++;
                    is_token_used[i] = true;
                }
                if (tokens[i].kind == TokenKind.CloseParenthesis)
                {
                    add--;
                    is_token_used[i] = true;
                }

                if (Token.OperatorsTokenKinds.Contains(tokens[i].kind))
                    sign_tokens.Add(new KeyValuePair<Token, int[]>(tokens[i], new int[2] { i, add }));
            }
            sign_tokens.Sort(new TokenComparer());
            CalcExpression[] expressions = new CalcExpression[tokens.Length];

            for (int i = 0; i < sign_tokens.Count; i++)
            {
                int ind = sign_tokens[i].Value[0];
                CalcExpression expr = new CalcExpression();
                expr.Operator = tokens[ind];
                int k = 0;
                while (is_token_used.Length > ind + k + 1 && is_token_used[ind + k + 1])
                    k++;
                Token t = tokens[ind + k + 1];
                if (Token.OperatorsTokenKinds.Contains(t.kind))
                    expr.Right = expressions[ind + k + 1];
                else
                    expr.Right = new CalcExpression() { Value = t };
                is_token_used[ind + k + 1] = true;
                
                k = 0;
                while (ind - k - 1 > 0 && is_token_used[ind - k - 1])
                    k++;

                t = tokens[ind - k - 1];
                if (Token.OperatorsTokenKinds.Contains(t.kind))
                    expr.Left = expressions[ind - k - 1];
                else
                    expr.Left = new CalcExpression() { Value = t };
                is_token_used[ind - k - 1] = true;

                expressions[ind] = expr;
            }

            return expressions[sign_tokens[sign_tokens.Count - 1].Value[0]];
        }
    }

    public class Evaluator
    {
        public Value Evaluate(IExpression expr)
        {
            switch (expr.Kind())
            {
                case ExpressionKind.Calc:
                    return EvaluateCalcExpression((CalcExpression)expr);
                    break;
                default:
                    throw new Exception("Unexpected expression kind '" + expr.Kind() + "'");
            }
        }

        public Value EvaluateCalcExpression(CalcExpression expr)
        {
            if (expr.Value != null)
            {
                return new Value() { type = typeof(int), value = int.Parse(expr.Value.text) };
            }

            TokenKind op_kind = expr.Operator.kind;
            Value left = Evaluate(expr.Left);
            Value right = Evaluate(expr.Right);

            // TODO: type checking for operations;

            switch (op_kind)
            {
                case TokenKind.Plus:
                    return new Value() { type = typeof(int), value = (int)left.value + (int)right.value };
                case TokenKind.Minus:
                    return new Value() { type = typeof(int), value = (int)left.value - (int)right.value };
                case TokenKind.Star:
                    return new Value() { type = typeof(int), value = (int)left.value * (int)right.value };
                case TokenKind.Slash:
                    return new Value() { type = typeof(int), value = (int)left.value / (int)right.value };
            }

            return new Value();
        }
    }

    class Program
    {

        static void Main(string[] args)
        {

            Lexer lexer = new Lexer();
            //string input = "func test (int: a, int: b) {" +
            //               "return a + b;" +
            //               "} :: int;";
            string input = "((1 + 2) * 6) / 2;";
            var tokens = lexer.GetTokens(input);
            //foreach (Token t in tokens)
            //{
            //    Console.Write(t.kind);
            //    Console.Write("  ->  ");
            //    Console.WriteLine(t.text);
            //}

            Parser parser = new Parser();
            IExpression[] e = parser.Parse(tokens);

            Evaluator evaluator = new Evaluator();
            Value res = evaluator.Evaluate(e[0]);

            Console.WriteLine(res.value);
        }
    }
}
