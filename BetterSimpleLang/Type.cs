using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterSimpleLang
{
    // Guid Type field;

    public enum Type
    {
        Null,
        Integer,
        Boolean,
        String,
        Double,
        
        Arr,

        Struct
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
        //public static Null Type = new Null();
        public static Type Type = Type.Null;

        public static object DefaultValue() => null;

        public static object ParseValue(object v) => null;
    }

    public class Integer : IType<int>
    {
        //public static Integer Type = new Integer();
        public static Type Type = Type.Integer;

        public static int ParseValue(object v)
        {
            if (v == null) Error.Type_IncorrectValueToParseInteger(v);
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
                    Error.Type_IncorrectValueToParseInteger(v);
                    return 0;
                }
            }
        }

        public static int DefaultValue() => 0;
    }

    public class Boolean : IType<bool>
    {
        public static Type Type = Type.Boolean;

        public static bool ParseValue(object v)
        {
            if (v == null) Error.Type_IncorrectValueToParseBoolean(v);
            try
            {
                return (bool)v;
            }
            catch
            {
                try
                {
                    return bool.Parse((string)v);
                }
                catch
                {
                    Error.Type_IncorrectValueToParseBoolean(v);
                    return false;
                }
            }
        }

        public static bool DefaultValue() => false;
    }

    public class String : IType<string>
    {
        public static Type Type = Type.String;

        public static string ParseValue(object v)
        {
            if (v == null) Error.Type_IncorrectValueToParseString(v);
            try
            {
                return (string)v;
            }
            catch
            {
                try
                {
                    return ((int)v).ToString();
                }
                catch
                {
                    try
                    {
                        return ((bool)v).ToString();
                    }
                    catch
                    {
                        try
                        {
                            return ((double)v).ToString();
                        }
                        catch
                        {
                            Error.Type_IncorrectValueToParseString(v);
                            return "";
                        }
                    }
                }
            }
        }

        public static string DefaultValue() => "";
    }

    public class Double : IType<double>
    {
        public static Type Type = Type.Double;

        public static double ParseValue(object v)
        {
            if (v == null) Error.Type_IncorrectValueToParseDouble(v);
            try
            {
                return (double)v;
            }
            catch
            {
                try
                {
                    return double.Parse(((string)v).Replace(".", ","));
                }
                catch
                {
                    Error.Type_IncorrectValueToParseDouble(v);
                    return 0;
                }
            }
        }

        public static double DefaultValue() => 0;
    }


    public interface IArr : IType<List<Variable>>
    {
        public bool Add(Variable item);
        public Variable Get(int index);
        public bool Remove(int index);
        public int Len();
    }

    public class Arr : IArr
    {
        private List<Variable> _list;
        public static Type ItemType = Null.Type;

        //public static Arr Type = new Arr();
        public static Type Type = Type.Arr;

        public static List<Variable> ParseValue(object v)
        {
            if (v == null) Error.Type_IncorrectValueToParseArray(v);
            try
            {
                return (List<Variable>)v;
            }
            catch
            {
                Error.Type_IncorrectValueToParseArray(v);
                return null;
            }
        }

        public static List<Variable> DefaultValue() => new List<Variable>();

        public Arr()
        {
            _list = new List<Variable>();
        }

        public bool Add(Variable item)
        {
            if (item.Type != ItemType) return false;
            _list.Add(item);
            return true;
        }

        public Variable Get(int index)
        {
            return _list[index];
        }

        public bool Remove(int index)
        {
            _list.RemoveAt(index);
            return true;
        }

        public int Len()
        {
            return _list.Count;
        }

    }

    public class IntArr : Arr
    {
        public static Type ItemType = Integer.Type;
        //public static IntArr Type = new IntArr();
    }
    public class DoubleArr : Arr
    {
        public static Type ItemType = Double.Type;
        //public static DoubleArr Type = new DoubleArr();
    }
    public class BoolArr : Arr
    {
        public static Type ItemType = Boolean.Type;
        //public static BoolArr Type = new BoolArr();
    }
    public class StrArr : Arr
    {
        public static Type ItemType = String.Type;
        //public static StrArr Type = new StrArr();
    }


    public class Struct : IType<List<Variable>>
    {
        public string Name;

        public static Type Type = Type.Struct;
        
        public static object DefaultValue(string typeName, Env env)
        {
            //Structure st = env.Structures.First(a => a.Name == typeName);
            Structure st = env.GetStructure(typeName);
            List<Variable> vars = new List<Variable>();
            foreach (var sf in st.Fields)
            {
                if (sf.Type == Arr.Type)
                    vars.Add(new Variable(sf.Name, sf.Type, Arr.DefaultValue()));
                else if (sf.Type == Struct.Type)
                    vars.Add(new Variable(sf.Name, sf.Type, Struct.DefaultValue(sf.TypeName, env)));
                else
                    vars.Add(new Variable(sf.Name, sf.Type, sf.Value, sf.IsConstant));
            }
            return vars;
        }

        public static List<Variable> ParseValue(object v)
        {
            if (v == null) Error.Type_IncorrectValueToParseStruct(v);
            try
            {
                List<Variable> fields = (List<Variable>)v;
                return fields;
            }
            catch (Exception e)
            {
                Error.Type_IncorrectValueToParseStruct(v);
                return null;
            }
        }
    }

}
