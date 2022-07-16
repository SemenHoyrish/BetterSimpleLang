using System;
using System.Collections.Generic;
using System.Text;

namespace BetterSimpleLang
{
    public class Lexer
    {
        public Token[] GetTokens(string input)
        {
            List<Token> result = new List<Token>();

            //var chars = input.ToCharArray().GetEnumerator();
            var chars = input.ToCharArray();

            //while (chars.MoveNext())
            int i = -1;

            char current()
            {
                if (i >= 0 && i < chars.Length)
                    return chars[i];
                return Char.MinValue;
            }
            char? next()
            {
                char? r = null;
                if (i < chars.Length - 1)
                {
                    i++;
                    r = current();
                }
                return r;
            }
            char? look_next()
            {
                char? r = null;
                if (i + 1 <= chars.Length - 1)
                    r = chars[i + 1];
                return r;
            }
            while (next() != null)
            {
                switch (current())
                {
                    case ':':
                        if (look_next() == ':')
                        {
                            next();
                            result.Add(new Token(TokenKind.ColonColon, "::"));
                        }
                        else
                            result.Add(new Token(TokenKind.Colon, ":"));
                        break;

                    case ';':
                        result.Add(new Token(TokenKind.Semicolon, ";"));
                        break;

                    case '+':
                        result.Add(new Token(TokenKind.Plus, "+"));
                        break;

                    case '-':
                        result.Add(new Token(TokenKind.Minus, "-"));
                        break;

                    case '*':
                        result.Add(new Token(TokenKind.Star, "*"));
                        break;

                    case '/':
                        result.Add(new Token(TokenKind.Slash, "/"));
                        break;

                    case '=':
                        if (look_next() == '=')
                        {
                            next();
                            result.Add(new Token(TokenKind.EqualsEquals, "=="));
                        }
                        else
                            result.Add(new Token(TokenKind.Equals, "="));
                        break;

                    case '>':
                        result.Add(new Token(TokenKind.Bigger, ">"));
                        break;

                    case '<':
                        result.Add(new Token(TokenKind.Less, "<"));
                        break;

                    case ',':
                        result.Add(new Token(TokenKind.Comma, ","));
                        break;

                    case '(':
                        result.Add(new Token(TokenKind.OpenParenthesis, "("));
                        break;

                    case ')':
                        result.Add(new Token(TokenKind.CloseParenthesis, ")"));
                        break;

                    case '{':
                        result.Add(new Token(TokenKind.OpenCurlyBracket, "{"));
                        break;

                    case '}':
                        result.Add(new Token(TokenKind.CloseCurlyBracket, "}"));
                        break;

                    case '[':
                        result.Add(new Token(TokenKind.OpenSquareBracket, "["));
                        break;

                    case ']':
                        result.Add(new Token(TokenKind.CloseSquareBracket, "]"));
                        break;

                    case '$':
                        result.Add(new Token(TokenKind.Dollar, "$"));
                        break;

                    default:
                        if (char.IsWhiteSpace(current()))
                        {
                            break;
                        }
                        if (char.IsLetter(current()))
                        {
                            string name = "";
                            name += current();
                            while (look_next() != null && char.IsLetter((char)look_next()))
                            {
                                name += next();
                            }
                            result.Add(new Token(TokenKind.Name, name));
                            break;
                        }
                        if (char.IsDigit(current()))
                        {
                            string number = "";
                            number += current();
                            while (look_next() != null && char.IsDigit((char)look_next()))
                            {
                                number += next();
                            }
                            result.Add(new Token(TokenKind.Number, number));
                            break;
                        }
                        throw new Exception("Unexpected character '" + current() + "'");
                }
            }

            return result.ToArray();
        }
    }
}
