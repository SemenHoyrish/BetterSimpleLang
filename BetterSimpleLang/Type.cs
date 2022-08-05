using System;
using System.Collections.Generic;
using System.Text;

namespace BetterSimpleLang
{
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
        public static Null Type = new Null();

        public static object DefaultValue() => null;

        public static object ParseValue(object v) => null;
    }

    public class Integer : IType<int>
    {
        public static Integer Type = new Integer();

        public static int ParseValue(object v)
        {
            // Report error
            if (v == null) return 0;
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
                    // Report error
                    return 0;
                }
            }
        }

        public static int DefaultValue() => 0;
    }

    public class Boolean : IType<bool>
    {
        public static Boolean Type = new Boolean();

        public static bool ParseValue(object v)
        {
            // Report error
            if (v == null) return false;
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
                    // Report error
                    return false;
                }
            }
        }

        public static bool DefaultValue() => false;
    }

    public class String : IType<string>
    {
        public static String Type = new String();

        public static string ParseValue(object v)
        {
            // Report error
            if (v == null) return "";
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
                        // Report error
                        return "";
                    }
                }
            }
        }

        public static string DefaultValue() => "";
    }

    public class Double : IType<double>
    {
        public static Double Type = new Double();

        public static double ParseValue(object v)
        {
            // Report error
            if (v == null) return 0;
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
                    // Report error
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
        public static IType ItemType = Null.Type;

        public static Arr Type = new Arr();

        public static List<Variable> ParseValue(object v)
        {
            // Report error
            if (v == null) return null;
            try
            {
                return (List<Variable>)v;
            }
            catch
            {
                // Report error
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
        public static IType ItemType = Integer.Type;
        //public static IntArr Type = new IntArr();
    }
    public class DoubleArr : Arr
    {
        public static IType ItemType = Double.Type;
        //public static DoubleArr Type = new DoubleArr();
    }
    public class BoolArr : Arr
    {
        public static IType ItemType = Boolean.Type;
        //public static BoolArr Type = new BoolArr();
    }
    public class StrArr : Arr
    {
        public static IType ItemType = String.Type;
        //public static StrArr Type = new StrArr();
    }


    public class Struct : IType<List<Variable>>
    {
        public string Name;

        //public static Struct Type = new Struct();

        public static object DefaultValue() => null;

        public static List<Variable> ParseValue(object v)
        {
            // Report error
            if (v == null) return null;
            try
            {
                List<Variable> fields = (List<Variable>)v;
                return fields;
            }
            catch
            {
                // Report error
                return null;
            }
        }
    }

}
