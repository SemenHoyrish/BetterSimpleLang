using System;
using System.Collections.Generic;
using System.Text;

namespace BetterSimpleLang
{
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

        public Variable Copy() => new Variable(Name, Type, Value);

        public static Variable NewEmpty() => new Variable("", Null.Type);
    }
}
