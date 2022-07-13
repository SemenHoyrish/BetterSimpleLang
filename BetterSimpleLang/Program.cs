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
        VarDeclaration,
        VarSet,
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

    public class VarDeclarationExpression : IExpression
    {

        public Token Name;
        public Token Type;

        public ExpressionKind Kind() => ExpressionKind.VarDeclaration;
    }

    public class VarSetExpression : IExpression
    {

        public Token Name;
        public Token Value;

        public ExpressionKind Kind() => ExpressionKind.VarSet;
    }

    public interface IType
    {
        public static IType Type;
    }

    public interface IType<T> : IType
    {
        //public T GetValue();
        //public bool SetValue(object v);
        //public static T ParseValue(object v) => default(T);
        //public static T DefaultValue() => default(T);
    }

    public class Null : IType<object>
    {
        public static Null Type = new Null();

        public static object DefaultValue() => null;

        public static object ParseValue(object v) => null;
    }

    public class Integer : IType<int>
    {
        public static Integer Type = new Integer();

        public static int ParseValue(object v)
        {
            try
            {
                return (int)v;
            }
            catch
            {
                try
                {
                    return int.Parse((string)v);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public static int DefaultValue() => 0;
    }

    //public class Value<T>
    //{
    //    public IType<T> Type;
    //    //private object _data;

    //    public Value(IType type)
    //    {
    //        Type = type;
    //        Type.SetValue(Type.DefaultValue());
    //    }
    //    public Value(IType type, object data)
    //    {

    //    }
    //}

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

    public class Iterator<T>
    {
        private T _NULL;
        private T[] _data;
        private int _index = -1;

        public Iterator (T[] data, T NULL)
        {
            _data = data;
            _NULL = NULL;
        }

        public T Current()
        {
            if (_index >= 0 && _index < _data.Length)
                return _data[_index];
            return _NULL;
        }

        public T Next()
        {
            if (_index < _data.Length - 1)
            {
                _index++;
                return Current();
            }
            return _NULL;
        }

        public T LookNext()
        {
            if (_index < _data.Length - 1)
            {
                _index++;
                return _data[_index + 1];
            }
            return _NULL;
        }
    }

    public class Parser
    {
        public IExpression[] Parse(Token[] tokens)
        {
            List<IExpression> exprs = new List<IExpression>();

            Iterator<Token> it = new Iterator<Token>(tokens, null);

            List<Token> ts = new List<Token>();
            while (it.Next() != null)
            {
                if (it.Current().kind == TokenKind.Semicolon)
                {
                    IExpression expr;
                    switch (ts[0])
                    {
                        case var t when ts[0].text == "var":
                            expr = ParseVarDeclarationExpression(ts.ToArray());
                            break;
                        case var t when ts[0].kind == TokenKind.Name && ts[1].kind == TokenKind.Equals:
                            expr = ParseVarSetExpression(ts.ToArray());
                            break;
                        default:
                            expr = ParseCalcExpression(ts.ToArray());
                            break;
                    }
                    exprs.Add(expr);
                    ts.Clear();
                }
                else
                {
                    ts.Add(it.Current());
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

        public VarDeclarationExpression ParseVarDeclarationExpression(Token[] tokens)
        {
            return new VarDeclarationExpression() { Name = tokens[1], Type = tokens[3] };
        }

        public VarSetExpression ParseVarSetExpression(Token[] tokens)
        {
            return new VarSetExpression() { Name = tokens[0], Value = tokens[2] };
        }
    }

    public class Evaluator
    {
        public Variable Evaluate(IExpression expr, Env env)
        {
            switch (expr.Kind())
            {
                case ExpressionKind.Calc:
                    return EvaluateCalcExpression((CalcExpression)expr, env);
                case ExpressionKind.VarDeclaration:
                    return EvaluateVarDeclarationExpression((VarDeclarationExpression)expr, env);
                case ExpressionKind.VarSet:
                    return EvaluateVarSetExpression((VarSetExpression)expr, env);
                default:
                    throw new Exception("Unexpected expression kind '" + expr.Kind() + "'");
            }
        }

        public Variable EvaluateCalcExpression(CalcExpression expr, Env env)
        {

            bool is_str_digits(string s)
            {
                foreach(var c in s)
                {
                    if (!char.IsDigit(c)) return false;
                }
                return true;
            }

            if (expr.Value != null)
            {
                if (is_str_digits(expr.Value.text))
                    return new Variable("", Integer.Type, expr.Value.text);

                Variable v = env.Variables.FirstOrDefault(a => a.Name == expr.Value.text);
                if (v == null)
                    return Variable.NewEmpty();

                return v;
            }

            TokenKind op_kind = expr.Operator.kind;
            Variable left = Evaluate(expr.Left, env);
            Variable right = Evaluate(expr.Right, env);

            if (left.Type != Integer.Type || right.Type !=  Integer.Type)
            {
                return new Variable("", Null.Type);
            }

            // TODO: type checking for operations

            switch (op_kind)
            {
                case TokenKind.Plus:
                    return new Variable("", Integer.Type, Integer.ParseValue(left.Value) + Integer.ParseValue(right.Value));
                case TokenKind.Minus:
                    return new Variable("", Integer.Type, Integer.ParseValue(left.Value) - Integer.ParseValue(right.Value));
                case TokenKind.Star:
                    return new Variable("", Integer.Type, Integer.ParseValue(left.Value) * Integer.ParseValue(right.Value));
                case TokenKind.Slash:
                    return new Variable("", Integer.Type, Integer.ParseValue(left.Value) / Integer.ParseValue(right.Value));
            }

            return new Variable("", Null.Type);
        }

        public Variable EvaluateVarDeclarationExpression(VarDeclarationExpression expr, Env env)
        {
            if (env.Variables.FirstOrDefault(a => a.Name == expr.Name.text) == null)
            {
                IType t = Null.Type;
                switch (expr.Type.text)
                {
                    case "int":
                        t = Integer.Type;
                        break;
                }
                env.Variables.Add(new Variable(expr.Name.text, t));
            }

            return Variable.NewEmpty();
        }

        public Variable EvaluateVarSetExpression(VarSetExpression expr, Env env)
        {
            if (env.Variables.FirstOrDefault(a => a.Name == expr.Name.text) != null)
            {
                Variable v = env.Variables.First(a => a.Name == expr.Name.text);
                if (v.Type == Integer.Type)
                {
                    v.Value = Integer.ParseValue(expr.Value.text);
                }
            }

            return Variable.NewEmpty();
        }
    }

    public class Variable
    {
        public string Name;
        public IType Type;
        public object Value;

        public Variable(string name, IType type)
        {
            Name = name;
            Type = type;
        }

        public Variable(string name, IType type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Name} => {Type} : '{Value}'";
        }

        public static Variable NewEmpty() => new Variable("", Null.Type);
    }

    public class Env
    {
        public List<Variable> Variables;

        public Env()
        {
            Variables = new List<Variable>();
        }
    }

    class Program
    {

        static void Main(string[] args)
        {

            Env env = new Env();

            Lexer lexer = new Lexer();
            //string input = "func test (int: a, int: b) {" +
            //               "return a + b;" +
            //               "} :: int;";
            //string input = "((1 + 2) * 6) / 2;";
            string input = "var a :: int; a = 2; a + 5;";
            //string input = "1 + 3;";
            var tokens = lexer.GetTokens(input);
            //foreach (Token t in tokens)
            //{
            //    Console.Write(t.kind);
            //    Console.Write("  ->  ");
            //    Console.WriteLine(t.text);
            //}

            Parser parser = new Parser();
            IExpression[] exprs = parser.Parse(tokens);

            Evaluator evaluator = new Evaluator();
            //Variable res = evaluator.Evaluate(e[0], env);

            foreach (var e in exprs)
            {
                Console.Write(e.Kind());
                Console.Write("> ");
                var r = evaluator.Evaluate(e, env);
                Console.WriteLine(r.Value);
            }

            //Console.WriteLine(res.Value);

            foreach(var v in env.Variables)
            {
                Console.WriteLine(v);
            }

        }
    }
}
