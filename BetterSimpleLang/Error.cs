using System;
using System.Collections.Generic;
using System.Text;

namespace BetterSimpleLang
{
    public static class Error
    {

        // LEXER
        public static void Lexer_UnexpectedCharacter(char character, int line, int column)
        {
            Console.WriteLine($"Unexpected character '{character}' at ({line};{column})");
            System.Environment.Exit(-1);
        }

        // TYPE
        public static void Type_IncorrectValueToParseInteger(object value)
        {
            Console.WriteLine($"Incorrect value '{value}' to parse as Integer");
            System.Environment.Exit(-1);
        }

        public static void Type_IncorrectValueToParseBoolean(object value)
        {
            Console.WriteLine($"Incorrect value '{value}' to parse as Boolean");
            System.Environment.Exit(-1);
        }

        public static void Type_IncorrectValueToParseString(object value)
        {
            Console.WriteLine($"Incorrect value '{value}' to parse as String");
            System.Environment.Exit(-1);
        }

        public static void Type_IncorrectValueToParseDouble(object value)
        {
            Console.WriteLine($"Incorrect value '{value}' to parse as Double");
            System.Environment.Exit(-1);
        }

        public static void Type_IncorrectValueToParseArray(object value)
        {
            Console.WriteLine($"Incorrect value '{value}' to parse as Array");
            System.Environment.Exit(-1);
        }

        public static void Type_IncorrectValueToParseStruct(object value)
        {
            Console.WriteLine($"Incorrect value '{value}' to parse as Struct");
            System.Environment.Exit(-1);
        }

        // EVALUATOR
        public static void Evaluator_UnexpectedExpressionKind(ExpressionKind kind, int line)
        {
            Console.WriteLine($"Unexpected expression kind '{kind}' on line {line}");
            System.Environment.Exit(-1);
        }

        public static void Evaluator_DifferentTypes(Type left, Type right, int line)
        {
            Console.WriteLine($"Different types: '{left}' and '{right}' on line {line}");
            System.Environment.Exit(-1);
        }

        public static void Evaluator_UnexpectedOperatorForInteger(TokenKind kind, int line)
        {
            Console.WriteLine($"Unexpected operator for Integer '{kind}' on line {line}");
            System.Environment.Exit(-1);
        }

        public static void Evaluator_UnexpectedOperatorForDouble(TokenKind kind, int line)
        {
            Console.WriteLine($"Unexpected operator for Double'{kind}' on line {line}");
            System.Environment.Exit(-1);
        }

        public static void Evaluator_UnexpectedOperatorForBoolean(TokenKind kind, int line)
        {
            Console.WriteLine($"Unexpected operator for Boolean '{kind}' on line {line}");
            System.Environment.Exit(-1);
        }

        public static void Evaluator_UnexpectedOperatorForString(TokenKind kind, int line)
        {
            Console.WriteLine($"Unexpected operator for String '{kind}' on line {line}");
            System.Environment.Exit(-1);
        }

        // PARSER
        public static void Parser_ErrorWhileParsingIfExpression(int line)
        {
            Console.WriteLine($"Error while parsing if expression on line {line}");
            System.Environment.Exit(-1);
        }

        public static void Parser_ErrorWhileParsingLoopExpression(int line)
        {
            Console.WriteLine($"Error while parsing loop expression on line {line}");
            System.Environment.Exit(-1);
        }

        // FUNCTION
        public static void Function_WrongTypeForArgument(string functionName, Type expected, Type real, int line)
        {
            Console.WriteLine($"Wrong type for argument for function '{functionName}'. Expected: '{expected}', got: '{real}' on line {line}");
            System.Environment.Exit(-1);
        }

        public static void Function_NotEnoughArguments(string functionName, int expected, int real, int line)
        {
            Console.WriteLine($"Not enough arguments for function '{functionName}'. Expected: '{expected}', got: '{real}' on line {line}");
            System.Environment.Exit(-1);
        }

        public static void Function_TooManyArguments(string functionName, int expected, int real, int line)
        {
            Console.WriteLine($"Too many arguments for function '{functionName}'. Expected: '{expected}', got: '{real}' on line {line}");
            System.Environment.Exit(-1);
        }

        public static void Function_ReturnedNothing(string functionName, int line)
        {
            Console.WriteLine($"Function '{functionName}' returned nothing on line {line}");
            System.Environment.Exit(-1);
        }

    }
}
