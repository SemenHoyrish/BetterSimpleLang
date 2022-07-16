using System;
using System.Collections.Generic;
using System.Text;

namespace BetterSimpleLang
{
    public class Parser
    {
        public IExpression[] Parse(Token[] tokens)
        {
            List<IExpression> exprs = new List<IExpression>();

            Iterator<Token> it = new Iterator<Token>(tokens, null);

            List<Token> ts = new List<Token>();

            bool func = false;
            int cb = 0;
            while (it.Next() != null)
            {
                if (it.Current().text == "func") func = true;
                if (it.Current().kind == TokenKind.OpenCurlyBracket) cb++;
                if (it.Current().kind == TokenKind.CloseCurlyBracket) cb--;
                if (it.Current().kind == TokenKind.Semicolon && func && cb == 0) func = false;

                if (!func && it.Current().kind == TokenKind.Semicolon)
                {
                    IExpression expr;
                    switch (ts[0])
                    {
                        case var t when ts[0].text == "var":
                            expr = ParseVarDeclarationExpression(ts.ToArray());
                            break;
                        case var t when ts[0].kind == TokenKind.Name && ts[1].kind == TokenKind.Equals:
                            expr = ParseVarSetExpression(ts.ToArray());
                            break;
                        case var t when ts[0].text == "func":
                            expr = ParseFuncDeclarationExpression(ts.ToArray());
                            break;
                        //case var t when ts[0].text == "$":
                        //    expr = ParseFuncExecutionExpression(ts.ToArray());
                        //    break;
                        default:
                            expr = ParseCalcExpression(ts.ToArray());
                            break;
                    }
                    exprs.Add(expr);
                    ts.Clear();
                }
                else
                {
                    ts.Add(it.Current());
                }
            }
            if (ts.Count > 0)
            {
                exprs.Add(ParseCalcExpression(ts.ToArray()));
            }

            return exprs.ToArray();
        }

        public CalcExpression ParseCalcExpression(Token[] tokens)
        {
            if (tokens.Length == 1)
                return new CalcExpression() { Value = tokens[0] };

            List<IExpression> exprs_list = new List<IExpression>();

            bool func = false;
            int open_par = 0;
            List<Token> func_ts = new List<Token>();
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].kind == TokenKind.Dollar) func = true;
                if (tokens[i].kind == TokenKind.OpenParenthesis && func) open_par++;
                if (func)
                {
                    func_ts.Add(tokens[i]);
                }
                else
                {
                    exprs_list.Add(new CalcExpression() { Value = tokens[i] });
                }
                if (tokens[i].kind == TokenKind.CloseParenthesis && func && open_par == 1)
                {
                    exprs_list.Add(ParseFuncExecutionExpression(func_ts.ToArray()));
                    func_ts.Clear();
                    func = false;
                }
                if (tokens[i].kind == TokenKind.CloseParenthesis) open_par--;

            }

            IExpression[] exprs = exprs_list.ToArray();

            bool[] is_token_used = new bool[exprs.Length];
            // Token, { index, add_value };
            List<KeyValuePair<Token, int[]>> sign_tokens = new List<KeyValuePair<Token, int[]>>();
            int add = 0;
            //bool func = false;
            for (int i = 0; i < exprs.Length; i++)
            {
                //if (tokens[i].kind == TokenKind.Dollar)
                //{
                //    func = true;
                //}

                if (exprs[i].Kind() == ExpressionKind.Calc && ((CalcExpression)exprs[i]).Value.kind == TokenKind.OpenParenthesis)
                {
                    add++;
                    is_token_used[i] = true;
                }
                if (exprs[i].Kind() == ExpressionKind.Calc && ((CalcExpression)exprs[i]).Value.kind == TokenKind.CloseParenthesis)
                {
                    add--;
                    is_token_used[i] = true;
                }

                if (exprs[i].Kind() == ExpressionKind.Calc && Token.OperatorsTokenKinds.Contains(((CalcExpression)exprs[i]).Value.kind))
                    sign_tokens.Add(new KeyValuePair<Token, int[]>(((CalcExpression)exprs[i]).Value, new int[2] { i, add }));
            }
            sign_tokens.Sort(new TokenComparer());
            CalcExpression[] expressions = new CalcExpression[exprs.Length];

            for (int i = 0; i < sign_tokens.Count; i++)
            {
                int ind = sign_tokens[i].Value[0];
                CalcExpression expr = new CalcExpression();
                expr.Operator = ((CalcExpression)exprs[ind]).Value;
                int k = 0;
                while (is_token_used.Length > ind + k + 1 && is_token_used[ind + k + 1])
                    k++;
                IExpression t = exprs[ind + k + 1];
                if (t.Kind() == ExpressionKind.Calc && Token.OperatorsTokenKinds.Contains(((CalcExpression)t).Value.kind))
                    expr.Right = expressions[ind + k + 1];
                else if (t.Kind() == ExpressionKind.FuncExecution)
                    expr.Right = t;
                else
                    expr.Right = new CalcExpression() { Value = ((CalcExpression)t).Value };
                is_token_used[ind + k + 1] = true;

                k = 0;
                while (ind - k - 1 > 0 && is_token_used[ind - k - 1])
                    k++;

                t = exprs[ind - k - 1];
                if (t.Kind() == ExpressionKind.Calc && Token.OperatorsTokenKinds.Contains(((CalcExpression)t).Value.kind))
                    expr.Left = expressions[ind - k - 1];
                else if (t.Kind() == ExpressionKind.FuncExecution)
                    expr.Left = t;
                else
                    expr.Left = new CalcExpression() { Value = ((CalcExpression)t).Value };
                is_token_used[ind - k - 1] = true;

                expressions[ind] = expr;
            }

            if (sign_tokens.Count == 0)
            {
                return new CalcExpression() { Left = exprs[0], Operator = new Token() { kind = TokenKind.Plus, text = "+" }, Right = new CalcExpression() { Value = new Token() { kind = TokenKind.Number, text = "0" } } };
            }

            return expressions[sign_tokens[sign_tokens.Count - 1].Value[0]];
        }

        public VarDeclarationExpression ParseVarDeclarationExpression(Token[] tokens)
        {
            return new VarDeclarationExpression() { Name = tokens[1], Type = tokens[3] };
        }

        public VarSetExpression ParseVarSetExpression(Token[] tokens)
        {
            if (tokens.Length == 3)
                return new VarSetExpression() { Name = tokens[0], Value = new CalcExpression() { Value = tokens[2] } };

            List<Token> ts = new List<Token>();
            for (int i = 2; i < tokens.Length; i++)
            {
                ts.Add(tokens[i]);
            }
            ts.Add(new Token(TokenKind.Semicolon, ";"));
            return new VarSetExpression() { Name = tokens[0], Value = Parse(ts.ToArray())[0] };
        }

        public FuncDeclarationExpression ParseFuncDeclarationExpression(Token[] tokens)
        {
            Iterator<Token> it = new Iterator<Token>(tokens, null);
            it.Next();
            Token name = it.Next();

            List<FuncArgExpression> args_list = new List<FuncArgExpression>();

            bool args = false;
            bool args_parsed = false;

            bool body = false;
            bool body_parsed = false;

            List<Token> body_tokens = new List<Token>();

            Token type = null;
            while (it.Next() != null)
            {
                //if (it.Current().kind == TokenKind.OpenParenthesis)
                switch (it.Current().kind)
                {
                    case TokenKind.OpenParenthesis:
                        args = true;
                        break;
                    case TokenKind.CloseParenthesis when args && !args_parsed:
                        args = false;
                        args_parsed = true;
                        break;
                    case TokenKind.OpenCurlyBracket:
                        body = true;
                        break;
                    case TokenKind.CloseCurlyBracket when body && !body_parsed:
                        body = false;
                        body_parsed = true;
                        break;
                    case TokenKind.ColonColon:
                        type = it.Next();
                        break;
                    default:
                        if (args)
                        {
                            if (it.Current().kind == TokenKind.Comma) it.Next();
                            Token n = it.Current();
                            it.Next();
                            args_list.Add(new FuncArgExpression() { Name = it.Next(), Type = n });
                        }
                        if (body)
                        {
                            body_tokens.Add(it.Current());
                        }
                        break;
                }
            }

            return new FuncDeclarationExpression() { Name = name, Type = type, Args = args_list.ToArray(), Body = Parse(body_tokens.ToArray()) };
        }

        public FuncExecutionExpression ParseFuncExecutionExpression(Token[] tokens)
        {
            Iterator<Token> it = new Iterator<Token>(tokens, null);
            it.Next();
            Token name = it.Next();

            FuncExecutionExpression e = new FuncExecutionExpression();
            e.Name = name;

            List<IExpression> exprs = new List<IExpression>();
            List<Token> ts = new List<Token>();

            int open_par = 0;
            while (it.Next() != null)
            {
                if (it.Current().kind == TokenKind.OpenParenthesis)
                {
                    if (open_par > 0) ts.Add(it.Current());
                    open_par++;
                    continue;
                }
                if (it.Current().kind == TokenKind.CloseParenthesis)
                {
                    if (open_par > 0) ts.Add(it.Current());
                    open_par--;
                    continue;
                }
                if (open_par == 1 && it.Current().kind == TokenKind.Comma)
                {
                    exprs.Add(Parse(ts.ToArray())[0]);
                    ts.Clear();
                    continue;
                }
                ts.Add(it.Current());
            }
            if (ts.Count > 0)
            {
                exprs.Add(Parse(ts.ToArray())[0]);
                ts.Clear();
            }
            e.Args = exprs.ToArray();

            return e;
        }
    }
}
