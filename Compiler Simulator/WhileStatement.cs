using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    //parsing by the format while-(condition){-statments} 
    public class WhileStatement : StatetmentBase
    {
        public Expression Term { get; private set; }
        public List<StatetmentBase> Body { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            Body = new List<StatetmentBase>();
            if (sTokens.Count < 6)
                throw new SyntaxErrorException("Early termination ", sTokens.LastPop);
            Token tWhile = sTokens.Pop();
            if (!(tWhile is Statement) || ((Statement)tWhile).Name != "while")
                throw new SyntaxErrorException("Expected while statment , received " + tWhile, tWhile);
            Token t = sTokens.Pop(); //(
            if (!(t is Parentheses) || ((Parentheses)t).Name != '(')
                throw new SyntaxErrorException("Expected (  , received " , t);
            Expression pTerm = (Expression.Create(sTokens));
            pTerm.Parse(sTokens);
            Term = pTerm;
            t = sTokens.Pop(); //(
            if (!(t is Parentheses) || ((Parentheses)t).Name != ')')
                throw new SyntaxErrorException("Expected )  , received ", t);
            t = sTokens.Pop(); //(
            if (!(t is Parentheses) || ((Parentheses)t).Name != '{')
                throw new SyntaxErrorException("Expected {  , received ", t);

            while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
            {
                StatetmentBase s = StatetmentBase.Create(sTokens.Peek());
                s.Parse(sTokens);
                Body.Add(s);
            }
            Token tEnd = sTokens.Pop();//}
        }

        public override string ToString()
        {
            string sWhile = "while(" + Term + "){\n";
            foreach (StatetmentBase s in Body)
                sWhile += "\t\t\t" + s + "\n";
            sWhile += "\t\t}";
            return sWhile;
        }

    }
}
