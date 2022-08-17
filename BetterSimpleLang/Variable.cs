using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterSimpleLang
{
    public class Variable
    {
        public string Name;
        public Type Type;
        public object Value;

        public Variable(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public Variable(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Name} => {Type} : '{Value}'";
        }

        //public Variable Copy() => new Variable(Name, Type, Value);
        public Variable Copy()
        {
            if (Type == Type.Arr || Type == Type.Struct)
            {
                List<Variable> new_vars = new List<Variable>();
                foreach (var v in (List<Variable>)Value)
                {
                    new_vars.Add(v.Copy());
                }
                return new Variable(Name, Type, new_vars);
            }

            return new Variable(Name, Type, Value);
        }

        public static Variable NewEmpty() => new Variable("", Null.Type);
    }
}
