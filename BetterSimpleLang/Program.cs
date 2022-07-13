using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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

    public enum ExpressionKind
    {
        Calc,
        VarDeclaration,
        VarSet,
        FuncDeclaration,
        FuncArg,
        Return,
        FuncExecution
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
        public IExpression Value;

        public ExpressionKind Kind() => ExpressionKind.VarSet;
    }

    public class FuncArgExpression : IExpression
    {
        public Token Type;
        public Token Name;

        public ExpressionKind Kind() => ExpressionKind.FuncArg;
    }

    public class FuncDeclarationExpression : IExpression
    {
        public Token Name;
        public FuncArgExpression[] Args;
        public IExpression[] Body;
        public Token Type;

        public ExpressionKind Kind() => ExpressionKind.FuncDeclaration;
    }

    public class FuncExecutionExpression : IExpression
    {
        public Token Name;
        public IExpression[] Args;

        public ExpressionKind Kind() => ExpressionKind.FuncExecution;
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
            if (v == null) return 0;
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

                    case '$':
                        result.Add(new Token(TokenKind.Dollar, "$"));
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

            bool func = false;
            int cb = 0;
            while (it.Next() != null)
            {
                if (it.Current().text == "func") func = true;
                if (it.Current().kind == TokenKind.OpenCurlyBracket) cb++;
                if (it.Current().kind == TokenKind.CloseCurlyBracket) cb--;
                if (it.Current().kind == TokenKind.Semicolon && func && cb == 0) func = false;

                if (!func && it.Current().kind == TokenKind.Semicolon)
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
                        case var t when ts[0].text == "func":
                            expr = ParseFuncDeclarationExpression(ts.ToArray());
                            break;
                        case var t when ts[0].text == "$":
                            expr = ParseFuncExecutionExpression(ts.ToArray());
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
            if (ts.Count > 0)
            {
                exprs.Add(ParseCalcExpression(ts.ToArray()));
            }
            
            return exprs.ToArray();
        }

        public CalcExpression ParseCalcExpression(Token[] tokens)
        {
            if (tokens.Length == 1)
                return new CalcExpression() { Value = tokens[0] };

            bool[] is_token_used = new bool[tokens.Length];
            // Token, { index, add_value };
            List<KeyValuePair<Token, int[]>> sign_tokens = new List<KeyValuePair<Token, int[]>>();
            int add = 0;
            //bool func = false;
            for(int i = 0; i < tokens.Length; i++)
            {
                //if (tokens[i].kind == TokenKind.Dollar)
                //{
                //    func = true;
                //}

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
            if (tokens.Length == 3)
                return new VarSetExpression() { Name = tokens[0], Value = new CalcExpression() { Value = tokens[2] } };

            List<Token> ts = new List<Token>();
            for (int i = 2; i < tokens.Length; i++)
            {
                ts.Add(tokens[i]);
            }
            ts.Add(new Token(TokenKind.Semicolon, ";"));
            return new VarSetExpression() { Name = tokens[0], Value = Parse(ts.ToArray())[0] };
        }

        public FuncDeclarationExpression ParseFuncDeclarationExpression(Token[] tokens)
        {
            Iterator<Token> it = new Iterator<Token>(tokens, null);
            it.Next();
            Token name = it.Next();

            List<FuncArgExpression> args_list = new List<FuncArgExpression>();

            bool args = false;
            bool args_parsed = false;

            bool body = false;
            bool body_parsed = false;

            List<Token> body_tokens = new List<Token>();

            Token type = null;
            while(it.Next() != null)
            {
                //if (it.Current().kind == TokenKind.OpenParenthesis)
                switch(it.Current().kind)
                {
                    case TokenKind.OpenParenthesis:
                        args = true;
                        break;
                    case TokenKind.CloseParenthesis when args && !args_parsed:
                        args = false;
                        args_parsed = true;
                        break;
                    case TokenKind.OpenCurlyBracket:
                        body = true;
                        break;
                    case TokenKind.CloseCurlyBracket when body && !body_parsed:
                        body = false;
                        body_parsed = true;
                        break;
                    case TokenKind.ColonColon:
                        type = it.Next();
                        break;
                    default:
                        if (args)
                        {
                            if (it.Current().kind == TokenKind.Comma) it.Next();
                            Token n = it.Current();
                            it.Next();
                            args_list.Add(new FuncArgExpression() { Name = it.Next(), Type = n });
                        }
                        if (body)
                        {
                            body_tokens.Add(it.Current());
                        }
                        break;
                }
            }

            return new FuncDeclarationExpression() { Name = name, Type = type, Args = args_list.ToArray(), Body = Parse(body_tokens.ToArray()) };
        }

        public FuncExecutionExpression ParseFuncExecutionExpression(Token[] tokens)
        {
            Iterator<Token> it = new Iterator<Token>(tokens, null);
            it.Next();
            Token name = it.Next();

            FuncExecutionExpression e = new FuncExecutionExpression();
            e.Name = name;

            List<IExpression> exprs = new List<IExpression>();
            List<Token> ts = new List<Token>();

            int open_par = 0;
            while(it.Next() != null)
            {
                if (it.Current().kind == TokenKind.OpenParenthesis)
                {
                    open_par++;
                    continue;
                }
                if (it.Current().kind == TokenKind.CloseParenthesis)
                {
                    open_par--;
                    continue;
                }
                if (open_par == 1 && it.Current().kind == TokenKind.Comma)
                {
                    exprs.Add(Parse(ts.ToArray())[0]);
                    ts.Clear();
                    continue;
                }
                ts.Add(it.Current());
            }
            if (ts.Count > 0)
            {
                exprs.Add(Parse(ts.ToArray())[0]);
                ts.Clear();
            }
            e.Args = exprs.ToArray();

            return e;
        }
    }

    public class Evaluator
    {
        private Dictionary<string, IType> _types = new Dictionary<string, IType>()
        {
            { "int", Integer.Type },
        };

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
                case ExpressionKind.FuncDeclaration:
                    return EvaluateFuncDeclarationExpression((FuncDeclarationExpression)expr, env);
                case ExpressionKind.FuncArg:
                    return EvaluateFuncArgExpression((FuncArgExpression)expr, env);
                case ExpressionKind.FuncExecution:
                    return EvaluateFuncExecutionExpression((FuncExecutionExpression)expr, env);
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
                    v.Value = Integer.ParseValue( Evaluate(expr.Value, env).Value );
                }
            }

            return Variable.NewEmpty();
        }

        public Variable EvaluateFuncDeclarationExpression(FuncDeclarationExpression expr, Env env)
        {
            if (env.Functions.FirstOrDefault(a => a.Name == expr.Name.text) == null)
            {
                List<KeyValuePair<string, IType>> args = new List<KeyValuePair<string, IType>>();
                foreach(var e in expr.Args)
                {
                    Variable arg = Evaluate(e, new Env());
                    args.Add( new KeyValuePair<string, IType>(arg.Name, arg.Type) );
                }
                Function f = new Function(
                    expr.Name.text,
                    _types[expr.Type.text],
                    args.ToArray(),
                    expr.Body
                    );
                env.Functions.Add(f);
            }

            return Variable.NewEmpty();
        }

        public Variable EvaluateFuncArgExpression(FuncArgExpression expr, Env env)
        {
            return new Variable(expr.Name.text, _types[expr.Type.text]);
        }

        public Variable EvaluateFuncExecutionExpression(FuncExecutionExpression expr, Env env)
        {
            Function f = env.Functions.FirstOrDefault(a => a.Name == expr.Name.text);
            if (f != null)
            {
                List<Variable> vars = new List<Variable>();
                foreach(var e in expr.Args)
                {
                    vars.Add(Evaluate(e, env));
                }
                return f.Execute(vars.ToArray());
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

    public class Function
    {
        public string Name;
        public IType Type;
        public KeyValuePair<string, IType>[] Args;
        public IExpression[] Body;

        public Function(string name, IType type, KeyValuePair<string, IType>[] args, IExpression[] body)
        {
            Name = name;
            Type = type;
            Args = args;
            Body = body;
        }

        public Variable Execute(Variable[] args)
        {

            Iterator<Variable> args_it = new Iterator<Variable>(args, null);
            var n = new KeyValuePair<string, IType>("", Null.Type);
            Iterator<KeyValuePair<string, IType>> Args_it = new Iterator<KeyValuePair<string, IType>>(Args, n);
            bool kvp_eq(KeyValuePair<string, IType> a, KeyValuePair<string, IType> b)
            {
                return a.Key == b.Key && a.Value == b.Value;
            }
            while(!kvp_eq(Args_it.Next(), n))
            {
                args_it.Next();

                var A_c = Args_it.Current();
                var a_c = args_it.Current();

                //if (A_c.Key != a_c.Name || A_c.Value != a_c.Type) throw new Exception();
                if (A_c.Value != a_c.Type) throw new Exception();
                a_c.Name = A_c.Key;
            }
            if(args_it.Next() != null) throw new Exception();

            Env env = new Env();
            env.Variables.AddRange(args);

            Evaluator evaluator = new Evaluator();

            object result = null;

            Variable r = Variable.NewEmpty();
            foreach (var e in Body)
            {
                r = evaluator.Evaluate(e, env);
                if (e.Kind() == ExpressionKind.Return) result = r.Value;
            }


            // Change this to Parse Return Expression
            if (result == null && r.Value != null)
                result = r.Value;

            return new Variable("", Type, result);
        }

    }

    public class Env
    {
        public List<Variable> Variables;
        public List<Function> Functions;

        public Env()
        {
            Variables = new List<Variable>();
            Functions = new List<Function>();
        }
    }

    // TODO: add GetTypeByToken function;

    class Program
    {

        static void Main(string[] args)
        {

            Env env = new Env();

            Lexer lexer = new Lexer();

            //string input = "var a :: int;" +
            //                "var b :: int;" +
            //                "" +
            //                "a = 12;" +
            //                "b = 3;" +
            //                "" +
            //                "a / b + 0;" +
            //                "";

            string input = "func test (int: a, int: b) {" +
                                "return a + b;" +
                            "} :: int;" +
                            "" +
                            "var sum :: int;" +
                            "sum = $test(2, 5) + 3;";
            //"sum = $test(a, 5) + 3 * 2;";

            var tokens = lexer.GetTokens(input);
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

            foreach (var v in env.Variables)
            {
                Console.WriteLine(v);
            }



        }
    }
}
