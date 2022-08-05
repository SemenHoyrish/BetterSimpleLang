using System;
using System.Collections.Generic;
using System.Text;

namespace BetterSimpleLang
{
    public enum ExpressionKind
    {
        Calc,
        VarDeclaration,
        VarSet,
        FuncDeclaration,
        FuncArg,
        Return,
        FuncExecution,
        If,
        Loop,
        StructDeclaration,
        StructField
    }

    public interface IExpression
    {
        public ExpressionKind Kind();
    }

    public class CalcExpression : IExpression
    {
        public IExpression Left;
        public Token Operator;
        public IExpression Right;

        //public Token Value;
        public Token Value;

        public ExpressionKind Kind() => ExpressionKind.Calc;
    }

    public class VarDeclarationExpression : IExpression
    {

        public Token Name;
        public Token Type;

        public ExpressionKind Kind() => ExpressionKind.VarDeclaration;
    }

    public class VarSetExpression : IExpression
    {

        public Token Name;
        public IExpression Value;

        public ExpressionKind Kind() => ExpressionKind.VarSet;
    }

    

    public class StructFieldExpression : IExpression
    {
        public Token Name;
        public Token Type;

        public ExpressionKind Kind() => ExpressionKind.StructField;
    }

    public class StructDeclarationExpression : IExpression
    {
        public Token Name;
        public StructFieldExpression[] Fields;

        public ExpressionKind Kind() => ExpressionKind.StructDeclaration;
    }

    public class FuncArgExpression : IExpression
    {
        public Token Type;
        public Token Name;
        public bool IsReference;

        public ExpressionKind Kind() => ExpressionKind.FuncArg;
    }

    public class FuncDeclarationExpression : IExpression
    {
        public Token Name;
        public FuncArgExpression[] Args;
        public IExpression[] Body;
        public Token Type;

        public ExpressionKind Kind() => ExpressionKind.FuncDeclaration;
    }

    public class FuncExecutionExpression : IExpression
    {
        public Token Name;
        public IExpression[] Args;

        public ExpressionKind Kind() => ExpressionKind.FuncExecution;
    }

    public class IfExpression : IExpression
    {
        public IExpression Condition;
        public IExpression[] Body;

        public ExpressionKind Kind() => ExpressionKind.If;
    }

    public class LoopExpression : IExpression
    {
        public IExpression Condition;
        public IExpression[] Body;

        public ExpressionKind Kind() => ExpressionKind.Loop;
    }
}
