using System;
using System.Collections.Generic;

namespace SimpleCompiler
{
    public class FunctionCallExpression : Expression
    {
        public string FunctionName { get; private set; }
        public List<Expression> Args { get; private set; }

        //parsing by the format functionname-(arguments){}
        public override void Parse(TokensStack sTokens)
        {
            Args = new List<Expression>();
            Token tName = sTokens.Pop();
            if (!(tName is Identifier))
                throw new SyntaxErrorException("Expected identifier name, received " + tName, tName);
            FunctionName = ((Identifier)tName).Name;
            Token t = sTokens.Pop(); //(
            while (sTokens.Count > 0)//)
            {
                if (sTokens.Peek() is Parentheses && ((Parentheses)sTokens.Peek()).Name == ')')
                    break;
                if (sTokens.Count < 1)
                    throw new SyntaxErrorException("Early termination ", t);
                Expression tExpression = Expression.Create(sTokens);
                tExpression.Parse(sTokens);
                Args.Add(tExpression);
                if (sTokens.Count > 0 && sTokens.Peek() is Separator)//,
                    sTokens.Pop();
            }
            Token tEnd = sTokens.Pop();//}
        }

        public override string ToString()
        {
            string sFunction = FunctionName + "(";
            for (int i = 0; i < Args.Count - 1; i++)
                sFunction += Args[i] + ",";
            if (Args.Count > 0)
                sFunction += Args[Args.Count - 1];
            sFunction += ")";
            return sFunction;
        }
    }
}