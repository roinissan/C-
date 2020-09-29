using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    //parsing by the format if-(condition){statments}
    public class IfStatement : StatetmentBase
    {
        public Expression Term { get; private set; }
        public List<StatetmentBase> DoIfTrue { get; private set; }
        public List<StatetmentBase> DoIfFalse { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            DoIfTrue = new List<StatetmentBase>();
            DoIfFalse = new List<StatetmentBase>();
            if (sTokens.Count < 5)
                throw new SyntaxErrorException("Early termination ", sTokens.LastPop);
            Token tIF = sTokens.Pop();
            if (!(tIF is Statement) || ((Statement)tIF).Name != "if")
                throw new SyntaxErrorException("Expected while statment , received " + tIF, tIF);
            Token t = sTokens.Pop(); //(
            if (!(t is Parentheses) || ((Parentheses)t).Name != '(')
                throw new SyntaxErrorException("Expected (  , received ", t);

            Term = Expression.Create(sTokens);
            Term.Parse(sTokens);

            t = sTokens.Pop(); //)
            if (!(t is Parentheses) || ((Parentheses)t).Name != ')')
                throw new SyntaxErrorException("Expected )  , received ", t);
            t = sTokens.Pop(); //{
            if (!(t is Parentheses) || ((Parentheses)t).Name != '{')
                throw new SyntaxErrorException("Expected {  , received ", t);
            while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
            {
                StatetmentBase s = StatetmentBase.Create(sTokens.Peek());
                s.Parse(sTokens);
                DoIfTrue.Add(s);
            }
            t = sTokens.Pop(); //}
            if (sTokens.Count > 3 && sTokens.Peek() is Statement && ((Statement)sTokens.Peek()).Name == "else")
            {
                t = sTokens.Pop();
                if (!(sTokens.Peek() is Parentheses) )
                    throw new SyntaxErrorException("Expected {  , received ", sTokens.Peek());
                if(((Parentheses)sTokens.Peek()).Name != '{')
                     throw new SyntaxErrorException("Expected {  , received ", sTokens.Peek());
                t = sTokens.Pop(); //{
                while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
                {
                    StatetmentBase s = StatetmentBase.Create(sTokens.Peek());
                    s.Parse(sTokens);
                    DoIfFalse.Add(s);
                }
                Token tEnd = sTokens.Pop();
            }
        }

        public override string ToString()
        {
            string sIf = "if(" + Term + "){\n";
            foreach (StatetmentBase s in DoIfTrue)
                sIf += "\t\t\t" + s + "\n";
            sIf += "\t\t}";
            if (DoIfFalse.Count > 0)
            {
                sIf += "else{";
                foreach (StatetmentBase s in DoIfFalse)
                    sIf += "\t\t\t" + s + "\n";
                sIf += "\t\t}";
            }
            return sIf;
        }

    }
}
