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
            int line = 1;
            int column = 0;

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
                column++;
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

            bool is_name_char(char c)
            {
                if (char.IsLetter(c)) return true;
                List<char> chars = new List<char>() { '_' };
                if (chars.Contains(c)) return true;
                return false;
            }

            bool str = false;
            //bool ignore_line = false;
            while (next() != null)
            {
                if (current() == Environment.NewLine.ToCharArray()[1])
                {
                    //if (ignore_line) ignore_line = false;
                    
                    line++;
                    column = 0;
                    //continue;
                }
                //else if (current() == '/' && look_next() == '/')
                //{
                //    next();
                //    ignore_line = true;
                //}
                //else if (ignore_line) continue;

                else if (current() == '"')
                {
                    str = !str;
                }
                else if (str)
                {
                    string name = "";
                    name += current();
                    while (look_next() != null && (char)look_next() != '"')
                    {
                        name += next();
                    }
                    result.Add(new Token(TokenKind.String, name, line, column));
                }
                else {
                    switch (current())
                    {
                        case ':':
                            if (look_next() == ':')
                            {
                                next();
                                result.Add(new Token(TokenKind.ColonColon, "::", line, column));
                            }
                            else
                                result.Add(new Token(TokenKind.Colon, ":", line, column));
                            break;

                        case ';':
                            result.Add(new Token(TokenKind.Semicolon, ";", line, column));
                            break;

                        case '+':
                            result.Add(new Token(TokenKind.Plus, "+", line, column));
                            break;

                        case '-':
                            if (look_next() == '>')
                            {
                                next();
                                result.Add(new Token(TokenKind.Arrow, "->", line, column));
                            }
                            else
                                result.Add(new Token(TokenKind.Minus, "-", line, column));
                            break;

                        case '*':
                            result.Add(new Token(TokenKind.Star, "*", line, column));
                            break;

                        case '/':
                            result.Add(new Token(TokenKind.Slash, "/", line, column));
                            break;

                        case '=':
                            if (look_next() == '=')
                            {
                                next();
                                result.Add(new Token(TokenKind.EqualsEquals, "==", line, column));
                            }
                            else
                                result.Add(new Token(TokenKind.Equals, "=", line, column));
                            break;

                        case '>':
                            result.Add(new Token(TokenKind.Bigger, ">", line, column));
                            break;

                        case '<':
                            result.Add(new Token(TokenKind.Less, "<", line, column));
                            break;

                        case ',':
                            result.Add(new Token(TokenKind.Comma, ",", line, column));
                            break;

                        case '(':
                            result.Add(new Token(TokenKind.OpenParenthesis, "(", line, column));
                            break;

                        case ')':
                            result.Add(new Token(TokenKind.CloseParenthesis, ")", line, column));
                            break;

                        case '{':
                            result.Add(new Token(TokenKind.OpenCurlyBracket, "{", line, column));
                            break;

                        case '}':
                            result.Add(new Token(TokenKind.CloseCurlyBracket, "}", line, column));
                            break;

                        case '[':
                            result.Add(new Token(TokenKind.OpenSquareBracket, "[", line, column));
                            break;

                        case ']':
                            result.Add(new Token(TokenKind.CloseSquareBracket, "]", line, column));
                            break;

                        case '$':
                            result.Add(new Token(TokenKind.Dollar, "$", line, column));
                            break;

                        default:
                            if (char.IsWhiteSpace(current()))
                            {
                                break;
                            }
                            if (is_name_char(current()))
                            {
                                string name = "";
                                name += current();
                                while (look_next() != null && is_name_char((char)look_next()))
                                {
                                    name += next();
                                }
                                result.Add(new Token(TokenKind.Name, name, line, column));
                                break;
                            }
                            if (char.IsDigit(current()) || current() == '.')
                            {
                                string number = "";
                                number += current();
                                while (look_next() != null && (char.IsDigit((char)look_next()) || (char)look_next() == '.'))
                                {
                                    number += next();
                                }
                                result.Add(new Token(TokenKind.Number, number, line, column));
                                break;
                            }
                            //throw new Exception("Unexpected character '" + current() + "'");
                            Error.Lexer_UnexpectedCharacter(current(), line, column);
                            break;
                    }
                }
            }

            return result.ToArray();
        }
    }
}
