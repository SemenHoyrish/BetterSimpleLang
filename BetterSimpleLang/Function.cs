using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BetterSimpleLang
{
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

        public Variable Execute(Variable[] _args)
        {
            Variable[] args = new Variable[_args.Length];
            for(int i = 0; i < _args.Length; i++)
                args[i] = _args[i].Copy();

            Iterator<Variable> args_it = new Iterator<Variable>(args, null);
            var n = new KeyValuePair<string, IType>("", Null.Type);
            Iterator<KeyValuePair<string, IType>> Args_it = new Iterator<KeyValuePair<string, IType>>(Args, n);
            bool kvp_eq(KeyValuePair<string, IType> a, KeyValuePair<string, IType> b)
            {
                return a.Key == b.Key && a.Value == b.Value;
            }
            while (!kvp_eq(Args_it.Next(), n))
            {
                args_it.Next();

                var A_c = Args_it.Current();
                var a_c = args_it.Current();

                //if (A_c.Key != a_c.Name || A_c.Value != a_c.Type) throw new Exception();
                if (A_c.Value != a_c.Type) throw new Exception();
                a_c.Name = A_c.Key;
            }
            if (args_it.Next() != null) throw new Exception();

            Env env = new Env();
            env.Variables.AddRange(args);

            Evaluator evaluator = new Evaluator();

            object result = null;

            Variable r = Variable.NewEmpty();

            if(new List<string>() { "printi", "printd", "prints", "printb" }.Contains(Name))
            {
                Console.Write( env.Variables[0].Value.ToString().Replace("\\n", "\n") );
                return r;
            }

            if (Name == "readi")
            {
                string s = Console.ReadLine();
                int i = 0;
                if (int.TryParse(s, out i))
                {
                    return new Variable("", Integer.Type, i);
                }
                else
                {
                    return r;
                }
            }
            if (Name == "readd")
            {
                string s = Console.ReadLine();
                double d = 0;
                if (double.TryParse(s, out d))
                {
                    return new Variable("", Double.Type, d);
                }
                else
                {
                    return r;
                }
            }
            if (Name == "reads")
            {
                string s = Console.ReadLine();
                return new Variable("", String.Type, s);
            }

            if (Name == "arr_add")
            {
                if (_args[0].Type != Arr.Type) return new Variable("", Boolean.Type, false);
                ((List<Variable>)_args[0].Value).Add(_args[1]);
                return new Variable("", Boolean.Type, true);
            }
            if (Name == "arr_get")
            {
                if (_args[0].Type != Arr.Type) return r;
                return ((List<Variable>)_args[0].Value)[(Integer.ParseValue(_args[1].Value))];
            }
            if (Name == "arr_del")
            {
                if (_args[0].Type != Arr.Type) return new Variable("", Boolean.Type, false);
                ((List<Variable>)_args[0].Value).RemoveAt(Integer.ParseValue(_args[1].Value));
                return new Variable("", Boolean.Type, true);
            }
            if (Name == "arr_len")
            {
                return new Variable("", Integer.Type, ((List<Variable>)_args[0].Value).Count);
            }
            if (Name == "read_file")
            {
                string s = String.ParseValue(_args[0].Value);
                return new Variable("", String.Type, File.ReadAllText(s));
            }
            if (Name == "write_file")
            {
                if (_args[0].Type != String.Type || _args[1].Type != String.Type) return new Variable("", Boolean.Type, false);
                if (File.Exists(String.ParseValue(_args[0].Value))) return new Variable("", Boolean.Type, false);
                File.WriteAllText( String.ParseValue(_args[0].Value), String.ParseValue(_args[1].Value).Replace("\\n", "\n") );
                return new Variable("", Boolean.Type, true);
            }
            if (Name == "concat_str")
            {
                return new Variable("", String.Type, String.ParseValue(_args[0].Value) + String.ParseValue(_args[1].Value));
            }

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
}
