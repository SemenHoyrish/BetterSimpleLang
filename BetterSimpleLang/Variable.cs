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
        private object _value;
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!IsConstant) _value = value;
                else
                {
                    //Error.Variable_ChangeConstantValue(Name);
                }
            }
        }
        public bool IsConstant;

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

        public Variable(string name, Type type, object value, bool isConstant)
        {
            Name = name;
            Type = type;
            Value = value;
            IsConstant = isConstant;
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
                return new Variable(Name, Type, new_vars, IsConstant);
            }

            return new Variable(Name, Type, Value, IsConstant);
        }

        public static Variable NewEmpty() => new Variable("", Null.Type);
    }
}
