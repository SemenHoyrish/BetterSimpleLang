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
                    return 0;
                }
            }
        }

        public static int DefaultValue() => 0;
    }
}
