using System;
using System.Collections.Generic;
using System.Text;

namespace BetterSimpleLang
{
    public class Env
    {
        public List<Variable> Variables;
        public List<Function> Functions;

        public Env()
        {
            Variables = new List<Variable>();
            Functions = new List<Function>();

            Functions.Add(new Function("printi", Integer.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", Integer.Type) }, new IExpression[0]));
            Functions.Add(new Function("printd", Integer.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", Double.Type) }, new IExpression[0]));
            Functions.Add(new Function("prints", Integer.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", String.Type) }, new IExpression[0]));
            Functions.Add(new Function("printb", Integer.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", Boolean.Type) }, new IExpression[0]));

            Functions.Add(new Function("readi", Integer.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", Integer.Type) }, new IExpression[0]));
            Functions.Add(new Function("readd", Double.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", Integer.Type) }, new IExpression[0]));
            Functions.Add(new Function("reads", String.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", Integer.Type) }, new IExpression[0]));

            Functions.Add(new Function("arr_add", Boolean.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", Arr.Type), new KeyValuePair<string, IType>("index", Integer.Type) }, new IExpression[0]));
            Functions.Add(new Function("arr_get", Arr.ItemType, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", Arr.Type), new KeyValuePair<string, IType>("index", Integer.Type) }, new IExpression[0]));
            Functions.Add(new Function("arr_del", Boolean.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", Arr.Type), new KeyValuePair<string, IType>("index", Integer.Type) }, new IExpression[0]));
            Functions.Add(new Function("arr_len", Integer.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", Arr.Type) }, new IExpression[0]));

            Functions.Add(new Function("read_file", Boolean.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", String.Type) }, new IExpression[0]));
            Functions.Add(new Function("write_file", Boolean.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("value", String.Type), new KeyValuePair<string, IType>("content", String.Type) }, new IExpression[0]));

            Functions.Add(new Function("concat_str", String.Type, new KeyValuePair<string, IType>[] { new KeyValuePair<string, IType>("str1", String.Type), new KeyValuePair<string, IType>("str2", String.Type) }, new IExpression[0]));
        }
    }
}
