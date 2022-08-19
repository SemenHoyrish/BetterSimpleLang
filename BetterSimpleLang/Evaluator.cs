using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterSimpleLang
{
    public class Evaluator
    {
        private Dictionary<string, Type> _types = new Dictionary<string, Type>()
        {
            { "int", Integer.Type },
            { "bool", Boolean.Type },
            { "str", String.Type },
            { "double", Double.Type },

            { "list_int", IntArr.Type },
            { "list_double", DoubleArr.Type },
            { "list_bool", BoolArr.Type },
            { "list_str", StrArr.Type },
        };


        private Type Type(string s, Env env)
        {
            if (_types.ContainsKey(s)) return _types[s];
            else if (env.GetStructure(s) != null) return BetterSimpleLang.Type.Struct;
            //else if (env.Structures.FirstOrDefault(a => a.Name == s) != null)
            //{
            //    return new Struct() { Name = s };
            //}
            return Null.Type;
        }

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
                case ExpressionKind.If:
                    return EvaluateIfExpression((IfExpression)expr, env);
                case ExpressionKind.Loop:
                    return EvaluateLoopExpression((LoopExpression)expr, env);
                case ExpressionKind.StructDeclaration:
                    return EvaluateStructDeclarationExpression((StructDeclarationExpression)expr, env);
                case ExpressionKind.Return:
                    return EvaluateReturnExpression((ReturnExpression)expr, env);
                default:
                    Error.Evaluator_UnexpectedExpressionKind(expr.Kind(), expr.Line);
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
                //if (is_str_digits(expr.Value.text))
                //    return new Variable("", Integer.Type, expr.Value.text);
                if (expr.Value.kind == TokenKind.Number)
                {
                    if (expr.Value.text.Contains("."))
                        return new Variable("", Double.Type, expr.Value.text);
                    else
                        return new Variable("", Integer.Type, expr.Value.text);
                }
                if (expr.Value.kind == TokenKind.String)
                    return new Variable("", String.Type, expr.Value.text);
                //return new Variable("", String.Type, expr.Value.text.Substring(1, expr.Value.text.Length - 1));
                if (expr.Value.kind == TokenKind.Name && (expr.Value.text.ToLower() == "true" || expr.Value.text.ToLower() == "false"))
                    return new Variable("", Boolean.Type, Boolean.ParseValue(expr.Value.text));


                Variable v = env.GetVariable(expr.Value.text);
                if (v == null)
                    return Variable.NewEmpty();

                return v;
            }

            TokenKind op_kind = expr.Operator.kind;
            Variable left = Evaluate(expr.Left, env);
            Variable right = Evaluate(expr.Right, env);

            Type left_type = left.Type;
            Type right_type = right.Type;

            if (op_kind == TokenKind.Arrow)
            {
                var sss = Struct.ParseValue(left.Value);
                return sss.First(a => a.Name == ((CalcExpression)expr.Right).Value.text);
            }

            //if (left.Type != Integer.Type || right.Type != Integer.Type)
            //{
            //    return new Variable("", Null.Type);
            //}

            // TODO: type checking for operations

            if (left_type != right_type)
            {
                Error.Evaluator_DifferentTypes(left_type, right_type, expr.Line);
            }

            if (op_kind == TokenKind.Equals)
            {
                left.Value = right.Value;
                return Variable.NewEmpty();
            }

            if (left_type == Integer.Type)
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
                    case TokenKind.EqualsEquals:
                        return new Variable("", Boolean.Type, Integer.ParseValue(left.Value) == Integer.ParseValue(right.Value));
                    case TokenKind.Bigger:
                        return new Variable("", Boolean.Type, Integer.ParseValue(left.Value) > Integer.ParseValue(right.Value));
                    case TokenKind.Less:
                        return new Variable("", Boolean.Type, Integer.ParseValue(left.Value) < Integer.ParseValue(right.Value));
                    default:
                        Error.Evaluator_UnexpectedOperatorForInteger(op_kind, expr.Line);
                        break;
                }
            else if (left_type == Double.Type)
                switch (op_kind)
                {
                    case TokenKind.Plus:
                        return new Variable("", Double.Type, Double.ParseValue(left.Value) + Double.ParseValue(right.Value));
                    case TokenKind.Minus:
                        return new Variable("", Double.Type, Double.ParseValue(left.Value) - Double.ParseValue(right.Value));
                    case TokenKind.Star:
                        return new Variable("", Double.Type, Double.ParseValue(left.Value) * Double.ParseValue(right.Value));
                    case TokenKind.Slash:
                        return new Variable("", Double.Type, Double.ParseValue(left.Value) / Double.ParseValue(right.Value));
                    case TokenKind.EqualsEquals:
                        return new Variable("", Boolean.Type, Double.ParseValue(left.Value) == Double.ParseValue(right.Value));
                    case TokenKind.Bigger:
                        return new Variable("", Boolean.Type, Double.ParseValue(left.Value) > Double.ParseValue(right.Value));
                    case TokenKind.Less:
                        return new Variable("", Boolean.Type, Double.ParseValue(left.Value) < Double.ParseValue(right.Value));
                    default:
                        Error.Evaluator_UnexpectedOperatorForDouble(op_kind, expr.Line);
                        break;
                }
            else if (left_type == Boolean.Type)
                switch (op_kind)
                {
                    case TokenKind.EqualsEquals:
                        return new Variable("", Boolean.Type, Boolean.ParseValue(left.Value) == Boolean.ParseValue(right.Value));
                    default:
                        Error.Evaluator_UnexpectedOperatorForBoolean(op_kind, expr.Line);
                        break;
                }
            else if (left_type == String.Type)
                switch (op_kind)
                {
                    case TokenKind.EqualsEquals:
                        return new Variable("", Boolean.Type, String.ParseValue(left.Value) == String.ParseValue(right.Value));
                    default:
                        Error.Evaluator_UnexpectedOperatorForString(op_kind, expr.Line);
                        break;
                }

            return new Variable("", Null.Type);
        }

        public Variable EvaluateVarDeclarationExpression(VarDeclarationExpression expr, Env env)
        {
            object default_value(Type t, string typeName)
            {
                if (t == Integer.Type) return Integer.DefaultValue();
                if (t == Boolean.Type) return Boolean.DefaultValue();
                if (t == String.Type) return String.DefaultValue();
                if (t == Double.Type) return Double.DefaultValue();
                if (t == Struct.Type) return Struct.DefaultValue(typeName, env);
                if (t == Arr.Type) return Arr.DefaultValue();
                return Null.DefaultValue();
            }

            if (env.GetVariable(expr.Name.text) == null)
            {
                Type t = Null.Type;
                //switch (expr.Type.text)
                //{
                //    case "int":
                //        t = Integer.Type;
                //        break;
                //}
                t = Type(expr.Type.text, env);
                if (t == Arr.Type)
                    env.Variables.Add(new Variable(expr.Name.text, t, Arr.DefaultValue()));
                else if (t == Struct.Type)
                {
                    //Structure st = env.Structures.First(a => a.Name == ((Struct)t).Name);
                    Structure st = env.GetStructure(expr.Type.text);
                    List<Variable> vars = new List<Variable>();
                    foreach(var sf in st.Fields)
                    {
                        vars.Add( new Variable(sf.Name, sf.Type, default_value(sf.Type, expr.Type.text)));
                    }
                    env.Variables.Add(new Variable(expr.Name.text, t, vars));
                }
                else
                    env.Variables.Add(new Variable(expr.Name.text, t));
            }

            return Variable.NewEmpty();
        }

        public Variable EvaluateVarSetExpression(VarSetExpression expr, Env env)
        {
            Variable v = env.GetVariable(expr.Name.text);
            if (v != null)
            {
                if (v.Type == Integer.Type)
                {
                    v.Value = Integer.ParseValue(Evaluate(expr.Value, env).Value);
                }
                if (v.Type == Double.Type)
                {
                    v.Value = Double.ParseValue(Evaluate(expr.Value, env).Value);
                }
                else if (v.Type == Boolean.Type)
                {
                    v.Value = Boolean.ParseValue(Evaluate(expr.Value, env).Value);
                }
                else if (v.Type == String.Type)
                {
                    v.Value = String.ParseValue(Evaluate(expr.Value, env).Value);
                }
                return v;
            }
            return Variable.NewEmpty();
        }

        public Variable EvaluateStructDeclarationExpression(StructDeclarationExpression expr, Env env)
        {
            if (env.GetStructure(expr.Name.text) == null)
            {
                List<StructureField> fields = new List<StructureField>();
                foreach(var fe in expr.Fields)
                {
                    fields.Add( new StructureField() { Name = fe.Name.text, Type = Type(fe.Type.text, env), TypeName = fe.Type.text } );
                }
                env.Structures.Add( new Structure() { Name = expr.Name.text, Fields = fields.ToArray() } );
            }

            return Variable.NewEmpty();
        }

        public Variable EvaluateFuncDeclarationExpression(FuncDeclarationExpression expr, Env env)
        {
            if (env.GetFunction(expr.Name.text) == null)
            {
                List<FunctionArgument> args = new List<FunctionArgument>();
                foreach (var e in expr.Args)
                {
                    Variable arg = Evaluate(e, env);
                    args.Add(new FunctionArgument(arg.Name, arg.Type, (bool)arg.Value));
                }
                Function f = new Function(
                    expr.Name.text,
                    Type(expr.Type.text, env),
                    args.ToArray(),
                    expr.Body
                    );
                env.Functions.Add(f);
            }

            return Variable.NewEmpty();
        }

        public Variable EvaluateFuncArgExpression(FuncArgExpression expr, Env env)
        {
            return new Variable(expr.Name.text, Type(expr.Type.text, env), expr.IsReference);
        }

        public Variable EvaluateFuncExecutionExpression(FuncExecutionExpression expr, Env env)
        {
            Function f = env.GetFunction(expr.Name.text);
            if (f != null)
            {
                List<Variable> vars = new List<Variable>();
                foreach (var e in expr.Args)
                {
                    var ttt = e.Kind();
                    vars.Add(Evaluate(e, env));
                }
                return f.Execute(vars.ToArray(), env, expr.Line);
            }

            return Variable.NewEmpty();
        }

        public Variable EvaluateIfExpression(IfExpression expr, Env env)
        {
            IExpression cond = expr.Condition;
            IExpression[] body = expr.Body;
            Variable cond_ev = Evaluate(cond, env);
            if ( cond_ev.Type == Boolean.Type && Boolean.ParseValue(cond_ev.Value) == true  )
            {
                foreach(var e in body)
                {
                    var r = Evaluate(e, env);
                    if (e.Kind() == ExpressionKind.Return) return r;
                }
            }

            return Variable.NewEmpty();
        }

        public Variable EvaluateLoopExpression(LoopExpression expr, Env env)
        {
            IExpression cond = expr.Condition;
            IExpression[] body = expr.Body;
            Variable cond_ev = Evaluate(cond, env);
            while (cond_ev.Type == Boolean.Type && Boolean.ParseValue(cond_ev.Value) == true)
            {
                foreach (var e in body)
                {
                    var r = Evaluate(e, env);
                    if (e.Kind() == ExpressionKind.Return) return r;
                }
                cond_ev = Evaluate(cond, env);
            }

            return Variable.NewEmpty();
        }

        public Variable EvaluateReturnExpression(ReturnExpression expr, Env env)
        {
            return Evaluate(expr.ForReturn, env);
        }

    }
}
