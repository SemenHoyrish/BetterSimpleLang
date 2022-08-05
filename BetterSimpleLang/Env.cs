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

            Functions.Add(new Function("printi", Integer.Type, new FunctionArgument[] { new FunctionArgument("value", Integer.Type, false) }, new IExpression[0]));
            Functions.Add(new Function("printd", Integer.Type, new FunctionArgument[] { new FunctionArgument("value", Double.Type, false) }, new IExpression[0]));
            Functions.Add(new Function("prints", Integer.Type, new FunctionArgument[] { new FunctionArgument("value", String.Type, false) }, new IExpression[0]));
            Functions.Add(new Function("printb", Integer.Type, new FunctionArgument[] { new FunctionArgument("value", Boolean.Type, false) }, new IExpression[0]));

            Functions.Add(new Function("readi", Integer.Type, new FunctionArgument[] { new FunctionArgument("value", Integer.Type, false) }, new IExpression[0]));
            Functions.Add(new Function("readd", Double.Type, new FunctionArgument[] { new FunctionArgument("value", Integer.Type, false) }, new IExpression[0]));
            Functions.Add(new Function("reads", String.Type, new FunctionArgument[] { new FunctionArgument("value", Integer.Type, false) }, new IExpression[0]));

            Functions.Add(new Function("arr_add", Boolean.Type, new FunctionArgument[] { new FunctionArgument("value", Arr.Type, false), new FunctionArgument("index", Integer.Type, false) }, new IExpression[0]));
            Functions.Add(new Function("arr_get", Arr.ItemType, new FunctionArgument[] { new FunctionArgument("value", Arr.Type, false), new FunctionArgument("index", Integer.Type, false) }, new IExpression[0]));
            Functions.Add(new Function("arr_del", Boolean.Type, new FunctionArgument[] { new FunctionArgument("value", Arr.Type, false), new FunctionArgument("index", Integer.Type, false) }, new IExpression[0]));
            Functions.Add(new Function("arr_len", Integer.Type, new FunctionArgument[] { new FunctionArgument("value", Arr.Type, false) }, new IExpression[0]));

            Functions.Add(new Function("read_file", Boolean.Type, new FunctionArgument[] { new FunctionArgument("value", String.Type, false) }, new IExpression[0]));
            Functions.Add(new Function("write_file", Boolean.Type, new FunctionArgument[] { new FunctionArgument("value", String.Type, false), new FunctionArgument("content", String.Type, false) }, new IExpression[0]));

            Functions.Add(new Function("concat_str", String.Type, new FunctionArgument[] { new FunctionArgument("str1", String.Type, false), new FunctionArgument("str2", String.Type, false) }, new IExpression[0]));
        }
    }
}
