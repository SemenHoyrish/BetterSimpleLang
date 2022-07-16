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

}
