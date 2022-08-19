using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterSimpleLang
{
    public class Env
    {
        public Env Root;
        public List<Variable> Variables;
        public List<Function> Functions;
        public List<Structure> Structures;

        public Env()
        {
            Root = null;
            Variables = new List<Variable>();
            Functions = new List<Function>();
            Structures = new List<Structure>();


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
            Functions.Add(new Function("split_str", Arr.Type, new FunctionArgument[] { new FunctionArgument("str", String.Type, false), new FunctionArgument("splitby", String.Type, false), new FunctionArgument("arr", Arr.Type, true) }, new IExpression[0]));

            Functions.Add(new Function("system", Null.Type, new FunctionArgument[] { new FunctionArgument("file", String.Type, false) }, new IExpression[0]));
            Functions.Add(new Function("sleep", Null.Type, new FunctionArgument[] { new FunctionArgument("ms", Integer.Type, false) }, new IExpression[0]));

            //Functions.Add(new Function("copy", Struct.Type, new FunctionArgument[] { new FunctionArgument("source", Struct.Type, false) }, new IExpression[0]));

            //Functions.Add(new Function("clear", Struct.Type, new FunctionArgument[] { new FunctionArgument("st", Struct.Type, true) }, new IExpression[0]));
        }

        // TODO: Add GetVar GetFunc GetStruct methods;
        // TODO: NO? Add global vars;
        // TODO: NO? Make functions see only local vars;

        public Env(Env root)
        {
            Root = root;
            Variables = new List<Variable>();
            Functions = new List<Function>();
            Structures = new List<Structure>();

            /*
            var vars = new Variable[root.Variables.Count];
            root.Variables.CopyTo(vars);
            Variables = vars.ToList();

            var funcs = new Function[root.Functions.Count];
            root.Functions.CopyTo(funcs);
            Functions = funcs.ToList();

            var structs = new Structure[root.Structures.Count];
            root.Structures.CopyTo(structs);
            Structures = structs.ToList();

            if (Variables == null)
                Variables = new List<Variable>();
            if (Functions == null)
                Functions = new List<Function>();
            if (Structures == null)
                Structures = new List<Structure>();
            */
        }

        public Variable GetVariable(string name)
        {
            var r = Variables.FirstOrDefault(a => a.Name == name);
            if (r == null && Root != null)
                r = Root.GetVariable(name);
            return r;
        }

        public Function GetFunction(string name)
        {
            var r = Functions.FirstOrDefault(a => a.Name == name);
            if (r == null && Root != null)
                r = Root.GetFunction(name);
            return r;
        }

        public Structure GetStructure(string name)
        {
            var r = Structures.FirstOrDefault(a => a.Name == name);
            if (r == null && Root != null)
                r = Root.GetStructure(name);
            return r;
        }

    }
}
