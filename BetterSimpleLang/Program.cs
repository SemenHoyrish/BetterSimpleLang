using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace BetterSimpleLang
{
    //public class Value<T>
    //{
    //    public IType<T> Type;
    //    //private object _data;

    //    public Value(IType type)
    //    {
    //        Type = type;
    //        Type.SetValue(Type.DefaultValue());
    //    }
    //    public Value(IType type, object data)
    //    {

    //    }
    //}

    

    // TODO: add GetTypeByToken function;

    class Program
    {

        static void Main(string[] args)
        {

            Env env = new Env();

            Lexer lexer = new Lexer();

            //string input = "var a :: int;" +
            //                "var b :: int;" +
            //                "" +
            //                "a = 12;" +
            //                "b = 3;" +
            //                "" +
            //                "a / b + 0;" +
            //                "";

            //string input = "func test (int: a, int: b) {" +
            //                    "return a + b;" +
            //                "} :: int;" +
            //                "" +
            //                "var sum :: int;" +
            //                "sum = $test(2, 5) + $test(4, 6);";
            //"sum = $test(a, 5) + 3 * 2;";

            string FILENAME = "test6.bsl";
            if (args.Length == 1)
                FILENAME = args[0];

            //string input = File.ReadAllText(FILENAME);
            string input = "";
            foreach(var l in File.ReadAllLines(FILENAME))
            {
                if (!l.Trim().StartsWith("//"))
                    input += l + '\n';
            }
            


            var tokens = lexer.GetTokens(input);
            Parser parser = new Parser();
            IExpression[] exprs = parser.Parse(tokens);

            Evaluator evaluator = new Evaluator();
            //Variable res = evaluator.Evaluate(e[0], env);

            foreach (var e in exprs)
            {
                //Console.Write(e.Kind());
                //Console.Write("> ");
                var r = evaluator.Evaluate(e, env);
                //Console.WriteLine(r.Value);
            }

            //Console.WriteLine(res.Value);


            foreach (var v in env.Variables)
            {
                //Console.WriteLine(v);
            }



        }
    }
}
