using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterSimpleLang
{
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
                foreach (var c in s)
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

            if (left.Type != Integer.Type || right.Type != Integer.Type)
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
                    v.Value = Integer.ParseValue(Evaluate(expr.Value, env).Value);
                }
            }

            return Variable.NewEmpty();
        }

        public Variable EvaluateFuncDeclarationExpression(FuncDeclarationExpression expr, Env env)
        {
            if (env.Functions.FirstOrDefault(a => a.Name == expr.Name.text) == null)
            {
                List<KeyValuePair<string, IType>> args = new List<KeyValuePair<string, IType>>();
                foreach (var e in expr.Args)
                {
                    Variable arg = Evaluate(e, new Env());
                    args.Add(new KeyValuePair<string, IType>(arg.Name, arg.Type));
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
                foreach (var e in expr.Args)
                {
                    vars.Add(Evaluate(e, env));
                }
                return f.Execute(vars.ToArray());
            }

            return Variable.NewEmpty();
        }

    }
}
