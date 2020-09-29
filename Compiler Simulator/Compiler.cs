using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SimpleCompiler
{
    public class Compiler
    {

        private Dictionary<string, int> m_dSymbolTable;
        private int m_cLocals;

        public Compiler()
        {
            m_dSymbolTable = new Dictionary<string, int>();
            m_cLocals = 0;

        }

        public List<string> Compile(string sInputFile)
        {
            List<string> lCodeLines = ReadFile(sInputFile);
            List<Token> lTokens = Tokenize(lCodeLines);
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            JackProgram program = Parse(sTokens);
            return null;
        }

        private JackProgram Parse(TokensStack sTokens)
        {
            JackProgram program = new JackProgram();
            program.Parse(sTokens);
            return program;
        }

        public List<string> Compile(List<string> lLines)
        {

            List<string> lCompiledCode = new List<string>();
            foreach (string sExpression in lLines)
            {
                List<string> lAssembly = Compile(sExpression);
                lCompiledCode.Add("// " + sExpression);
                lCompiledCode.AddRange(lAssembly);
            }
            return lCompiledCode;
        }



        public List<string> ReadFile(string sFileName)
        {
            StreamReader sr = new StreamReader(sFileName);
            List<string> lCodeLines = new List<string>();
            while (!sr.EndOfStream)
            {
                lCodeLines.Add(sr.ReadLine());
            }
            sr.Close();
            return lCodeLines;
        }




        private bool Contains(string[] a, string s)
        {
            foreach (string s1 in a)
                if (s1 == s)
                    return true;
            return false;
        }
        private bool Contains(char[] a, char c)
        {
            foreach (char c1 in a)
                if (c1 == c)
                    return true;
            return false;
        }


        private string Next(string s, char[] aDelimiters, out string sToken, out int cChars)
        {
            cChars = 1;
            sToken = s[0] + "";
            if (Contains(aDelimiters, s[0]))
                return s.Substring(1);
            int i = 0;
            for (i = 1; i < s.Length; i++)
            {
                if (Contains(aDelimiters, s[i]))
                    return s.Substring(i);
                else
                    sToken += s[i];
                cChars++;
            }
            return null;
        }
        private List<string> Split(string s, char[] aDelimiters)
        {
            List<string> lTokens = new List<string>();
            while (s.Length > 0)
            {
                string sToken = "";
                int i = 0;
                for (i = 0; i < s.Length; i++)
                {
                    if (Contains(aDelimiters, s[i]))
                    {
                        if (sToken.Length > 0)
                            lTokens.Add(sToken);
                        lTokens.Add(s[i] + "");
                        break;
                    }
                    else
                        sToken += s[i];
                }
                if (i == s.Length)
                {
                    lTokens.Add(sToken);
                    s = "";
                }
                else
                    s = s.Substring(i + 1);
            }
            return lTokens;
        }

        // splits each line by the delimiters and then checks for each symbol in the splitted line which token is relevent
        public List<Token> Tokenize(List<string> lCodeLines)
        {
            List<string> string_tokens;
            List<Token> lTokens = new List<Token>();
            char[] delemiters = { '(', ')', '{', '}', ',', ';', '*', '+', '<', '>', '=', '-', '\t', ' ', '/', '[', ']', '!', '\n' };
            int line_index = 0, postion, n;
            string stripedLine;
            Token token;
            char symbolChar;
            // iterate through the line
            foreach (string sLine in lCodeLines)
            {
                stripedLine = Regex.Split(sLine, "//")[0];
                if (stripedLine.Length == 0 || stripedLine.StartsWith("//"))
                {
                    line_index++;
                    continue;
                }
                string_tokens = Split(stripedLine, delemiters);
                postion = 0;
                // iterate through the tokens 
                foreach (string symbol in string_tokens)
                {

                    if (symbol.StartsWith("\t") || symbol.StartsWith(" ") || symbol.StartsWith("\n"))
                    {
                        postion++;
                        continue;
                    }
                    if (Token.Statements.Contains(symbol))
                        token = new Statement(symbol, line_index, postion);
                    else if (Token.VarTypes.Contains(symbol))
                        token = new VarType(symbol, line_index, postion);
                    else if (Token.Constants.Contains(symbol))
                        token = new Constant(symbol, line_index, postion);
                    else if (symbol.Length == 1)
                    {
                        symbolChar = symbol[0];
                        if (Token.Operators.Contains(symbolChar))
                            token = new Operator(symbolChar, line_index, postion);
                        else if (Token.Parentheses.Contains(symbolChar))
                            token = new Parentheses(symbolChar, line_index, postion);
                        else if (Token.Separators.Contains(symbolChar))
                            token = new Separator(symbolChar, line_index, postion);
                        else if (int.TryParse(symbol, out n))
                            token = new Number(symbol, line_index, postion);
                        else
                            token = new Identifier(symbol, line_index, postion);
                    }
                    else if (int.TryParse(symbol, out n))
                        token = new Number(symbol, line_index, postion);
                    else
                        token = new Identifier(symbol, line_index, postion);
                    lTokens.Add(token);
                    n = symbol.Length;
                    postion = postion + symbol.Length;
                }
                line_index++;
            }
            return lTokens;
        }

    }
}
