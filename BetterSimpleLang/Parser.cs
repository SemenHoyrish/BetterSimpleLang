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
            bool loop = false;
            int cb_loop = 0;
            bool if_expr = false;
            int cb_if = 0;
            bool define = false;
            int cb_define = 0;
            while (it.Next() != null)
            {
                if (it.Current().text == "define") define = true;
                if (it.Current().kind == TokenKind.OpenCurlyBracket) cb_define++;
                if (it.Current().kind == TokenKind.CloseCurlyBracket) cb_define--;
                if (it.Current().kind == TokenKind.Semicolon && define && cb_define == 0) define = false;

                if (!define && it.Current().text == "func") func = true;
                if (!define && it.Current().kind == TokenKind.OpenCurlyBracket) cb++;
                if (!define && it.Current().kind == TokenKind.CloseCurlyBracket) cb--;
                if (!define && it.Current().kind == TokenKind.Semicolon && func && cb == 0) func = false;

                if (!define && !func && it.Current().text == "while") loop = true;
                if (!define && it.Current().kind == TokenKind.OpenCurlyBracket && loop) cb_loop++;
                if (!define && it.Current().kind == TokenKind.CloseCurlyBracket && loop) cb_loop--;
                if (!define && it.Current().kind == TokenKind.Semicolon && loop && cb_loop == 0) loop = false;

                if (!define && !func && !loop && it.Current().text == "if") if_expr= true;
                if (!define && !loop && it.Current().kind == TokenKind.OpenCurlyBracket && if_expr) cb_if++;
                if (!define && !loop && it.Current().kind == TokenKind.CloseCurlyBracket && if_expr) cb_if--;
                if (!define && !loop && it.Current().kind == TokenKind.Semicolon && if_expr && cb_if == 0) if_expr = false;

                if (!define && !func && !loop && !if_expr && it.Current().kind == TokenKind.Semicolon)
                {
                    IExpression expr;
                    
                    switch (ts[0])
                    {
                        case var t when ts[0].text == "var":
                            expr = ParseVarDeclarationExpression(ts.ToArray());
                            break;
                        //case var t when ts[0].kind == TokenKind.Name && ts[1].kind == TokenKind.Equals:
                        //    expr = ParseVarSetExpression(ts.ToArray());
                        //    break;
                        case var t when ts[0].text == "define":
                            expr = ParseStructDeclarationExpression(ts.ToArray());
                            break;
                        case var t when ts[0].text == "func":
                            expr = ParseFuncDeclarationExpression(ts.ToArray());
                            break;
                        //case var t when ts[0].text == "return":
                        //    expr = ParseFuncDeclarationExpression(ts.ToArray());
                            //break;
                        case var t when ts[0].text == "while":
                            expr = ParseLoopExpression(ts.ToArray());
                            break;
                        case var t when ts[0].text == "if":
                            expr = ParseIfExpression(ts.ToArray());
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

        public IExpression ParseCalcExpression(Token[] tokens)
        {
            if (tokens.Length == 1)
                return new CalcExpression() { Value = tokens[0], Line = tokens[0].Line };

            if (tokens[0].text == "return")
            {
                Token[] new_tokens = new Token[tokens.Length - 1];
                for(int i = 0; i < tokens.Length - 1; ++i)
                {
                    new_tokens[i] = tokens[i + 1];
                }
                return new ReturnExpression() { ForReturn = Parse(new_tokens)[0], Line = tokens[0].Line };
            }

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
                    exprs_list.Add(new CalcExpression() { Value = tokens[i], Line = tokens[0].Line });
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
                CalcExpression expr = new CalcExpression() { Line = tokens[0].Line };
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
                    expr.Right = new CalcExpression() { Value = ((CalcExpression)t).Value, Line = tokens[0].Line };
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
                    expr.Left = new CalcExpression() { Value = ((CalcExpression)t).Value, Line = tokens[0].Line };
                is_token_used[ind - k - 1] = true;

                expressions[ind] = expr;
            }

            if (sign_tokens.Count == 0)
            {
                //return new CalcExpression() { Left = exprs[0], Operator = new Token() { kind = TokenKind.Plus, text = "+" }, Right = new CalcExpression() { Value = new Token() { kind = TokenKind.Number, text = "0" } } };
                return exprs[0];
            }

            return expressions[sign_tokens[sign_tokens.Count - 1].Value[0]];
        }

        public VarDeclarationExpression ParseVarDeclarationExpression(Token[] tokens)
        {
            int add = 0;
            if (tokens[1].text == "const") add = 1;

            IExpression value = null;
            List<Token> tkns = new List<Token>();
            if (tokens.Length > 4 + add && tokens[4 + add].kind == TokenKind.Equals)
            {
                for(int i = 5 + add; i < tokens.Length; ++i)
                {
                    tkns.Add(tokens[i]);
                }
                value = Parse(tkns.ToArray())[0];
            }


            return new VarDeclarationExpression() { Name = tokens[1 + add], Type = tokens[3 + add], Value = value, isConstant = add == 1, Line = tokens[0].Line };
        }

        public VarSetExpression ParseVarSetExpression(Token[] tokens)
        {
            if (tokens.Length == 3)
                return new VarSetExpression() { Name = tokens[0], Value = new CalcExpression() { Value = tokens[2], Line = tokens[0].Line }, Line = tokens[0].Line };

            List<Token> ts = new List<Token>();
            for (int i = 2; i < tokens.Length; i++)
            {
                ts.Add(tokens[i]);
            }
            ts.Add(new Token(TokenKind.Semicolon, ";", 0, 0));
            return new VarSetExpression() { Name = tokens[0], Value = Parse(ts.ToArray())[0], Line = tokens[0].Line };
        }

        public StructDeclarationExpression ParseStructDeclarationExpression(Token[] tokens)
        {
            Iterator<Token> it = new Iterator<Token>(tokens, null);
            it.Next();

            Token name = it.Next();
            List<StructFieldExpression> fields = new List<StructFieldExpression>();

            it.Next();

            while(it.Next() != null)
            {
                if (it.Current().kind != TokenKind.CloseCurlyBracket && it.Current().kind != TokenKind.Semicolon)
                {
                    bool isConst = false;
                    if (it.Current().text == "const")
                    {
                        isConst = true;
                        it.Next();
                    }
                    Token field_type = it.Current();
                    it.Next();
                    Token field_name = it.Next();

                    List<Token> tkns = new List<Token>();
                    if (it.LookNext() != null && it.LookNext().kind == TokenKind.Equals)
                    {
                        it.Next();
                        while (it.LookNext() != null && it.LookNext().kind != TokenKind.Semicolon)
                        {
                            tkns.Add(it.Next());
                        }
                    }

                    fields.Add(new StructFieldExpression() { Name = field_name, Type = field_type, isConstant = isConst, Value = tkns.Count > 0 ? Parse(tkns.ToArray())[0] : null, Line = tokens[0].Line });
                }
            }

            return new StructDeclarationExpression() { Name = name, Fields = fields.ToArray(), Line = tokens[0].Line };
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
            int open_cur = 0;

            List<Token> body_tokens = new List<Token>();

            Token type = null;
            while (it.Next() != null)
            {
                //if (it.Current().kind == TokenKind.OpenParenthesis)
                TokenKind cur = it.Current().kind;
                if (cur == TokenKind.OpenParenthesis && !args_parsed)
                {
                    args = true;
                }
                else if (cur == TokenKind.CloseParenthesis && !args_parsed)
                {
                    args = false;
                    args_parsed = true;
                }
                else if (cur == TokenKind.OpenCurlyBracket)
                {
                    if (!body)
                    {
                        body = true;
                    }
                    else
                    {
                        body_tokens.Add(it.Current());
                    }
                    open_cur++;
                }
                else if(cur == TokenKind.CloseCurlyBracket)
                {
                    if (open_cur == 1 && body)
                    {
                        body = false;
                        body_parsed = true;
                    }else
                    {
                        body_tokens.Add(it.Current());
                    }
                    open_cur--;
                }
                else if (cur == TokenKind.ColonColon && body_parsed)
                {
                    type = it.Next();
                    break;
                }

                else if (args)
                {
                    if (it.Current().kind == TokenKind.Comma) it.Next();
                    Token n = it.Current();
                    bool is_ref = false;
                    if (n.text == "ref")
                    {
                        is_ref = true;
                        n = it.Next();
                    }
                    it.Next();
                    args_list.Add(new FuncArgExpression() { Name = it.Next(), Type = n, IsReference = is_ref, Line = tokens[0].Line });
                }
                else if (body)
                {
                    body_tokens.Add(it.Current());
                }

                //switch (it.Current().kind)
                //{
                //    case TokenKind.OpenParenthesis when !args_parsed:
                //        args = true;
                //        break;
                //    case TokenKind.CloseParenthesis when args && !args_parsed:
                //        args = false;
                //        args_parsed = true;
                //        break;
                //    case TokenKind.OpenCurlyBracket when !body:
                //        body = true;
                //        break;
                //    case TokenKind.CloseCurlyBracket when body && !body_parsed:
                //        body = false;
                //        body_parsed = true;
                //        break;
                //    case TokenKind.ColonColon:
                //        type = it.Next();
                //        break;
                //    default:
                //        if (args)
                //        {
                //            if (it.Current().kind == TokenKind.Comma) it.Next();
                //            Token n = it.Current();
                //            it.Next();
                //            args_list.Add(new FuncArgExpression() { Name = it.Next(), Type = n });
                //        }
                //        if (body)
                //        {
                //            body_tokens.Add(it.Current());
                //        }
                //        break;
                //}
            }

            return new FuncDeclarationExpression() { Name = name, Type = type, Args = args_list.ToArray(), Body = Parse(body_tokens.ToArray()), Line = tokens[0].Line };
        }

        public FuncExecutionExpression ParseFuncExecutionExpression(Token[] tokens)
        {
            Iterator<Token> it = new Iterator<Token>(tokens, null);
            it.Next();
            Token name = it.Next();

            FuncExecutionExpression e = new FuncExecutionExpression() { Line = tokens[0].Line };
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
                    if (open_par > 1) ts.Add(it.Current());
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

        public IfExpression ParseIfExpression(Token[] tokens)
        {
            Iterator<Token> it = new Iterator<Token>(tokens, null);
            it.Next();

            bool condition_parsed = false;
            int open_par = 0;

            List<Token> condition = new List<Token>();
            IExpression condition_expr = null;

            bool body_parsed = false;
            int open_cur = 0;

            List<Token> body = new List<Token>();
            IExpression[] body_exprs = null;

            while(it.Next() != null)
            {
                if (!condition_parsed)
                {
                    if (it.Current().kind == TokenKind.OpenParenthesis)
                    {
                        if (open_par > 0) condition.Add(it.Current());

                        open_par++;
                    }
                    else if (it.Current().kind == TokenKind.CloseParenthesis)
                    {
                        if (open_par == 1)
                        {
                            condition_parsed = true;
                            condition_expr = condition.Count > 0 ? Parse(condition.ToArray())[0] : null;
                        }
                        else
                        {
                            condition.Add(it.Current());
                        }
                        open_par--;
                    }
                    else
                        condition.Add(it.Current());
                }
                else
                {
                    if (it.Current().kind == TokenKind.OpenCurlyBracket)
                    {
                        if(open_cur > 0)
                        {
                            body.Add(it.Current());
                        }
                        open_cur++;
                    }
                    else if(it.Current().kind == TokenKind.CloseCurlyBracket)
                    {
                        if (open_cur == 1)
                        {
                            body_parsed = true;
                            body_exprs = Parse(body.ToArray());
                        }
                        else
                        {
                            body.Add(it.Current());
                        }
                        open_cur--;
                    }
                    else
                    {
                        body.Add(it.Current());
                    }
                }
            }

            if (condition_expr == null || body_exprs == null)
            {
                Error.Parser_ErrorWhileParsingIfExpression(tokens[0].Line);
            }

            return new IfExpression() { Condition = condition_expr, Body = body_exprs, Line = tokens[0].Line };
        }

        public LoopExpression ParseLoopExpression(Token[] tokens)
        {
            Iterator<Token> it = new Iterator<Token>(tokens, null);
            it.Next();

            bool condition_parsed = false;
            int open_par = 0;

            List<Token> condition = new List<Token>();
            IExpression condition_expr = null;

            bool body_parsed = false;
            int open_cur = 0;

            List<Token> body = new List<Token>();
            IExpression[] body_exprs = null;

            while (it.Next() != null)
            {
                if (!condition_parsed)
                {
                    if (it.Current().kind == TokenKind.OpenParenthesis)
                    {
                        if (open_par > 0) condition.Add(it.Current());

                        open_par++;
                    }
                    else if (it.Current().kind == TokenKind.CloseParenthesis)
                    {
                        if (open_par == 1)
                        {
                            condition_parsed = true;
                            condition_expr = condition.Count > 0 ? Parse(condition.ToArray())[0] : null;
                        }
                        else
                        {
                            condition.Add(it.Current());
                        }
                        open_par--;
                    }
                    else
                        condition.Add(it.Current());
                }
                else
                {
                    if (it.Current().kind == TokenKind.OpenCurlyBracket)
                    {
                        if (open_cur > 0)
                        {
                            body.Add(it.Current());
                        }
                        open_cur++;
                    }
                    else if (it.Current().kind == TokenKind.CloseCurlyBracket)
                    {
                        if (open_cur == 1)
                        {
                            body_parsed = true;
                            body_exprs = Parse(body.ToArray());
                        }
                        else
                        {
                            body.Add(it.Current());
                        }
                        open_cur--;
                    }
                    else
                    {
                        body.Add(it.Current());
                    }
                }
            }

            if (condition_expr == null || body_exprs == null)
            {
                Error.Parser_ErrorWhileParsingLoopExpression(tokens[0].Line);
            }

            return new LoopExpression() { Condition = condition_expr, Body = body_exprs, Line = tokens[0].Line };
        }
    }
}
