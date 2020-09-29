using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class LetStatement : StatetmentBase
    {
        public string Variable { get; set; }
        public Expression Value { get; set; }

        public override string ToString()
        {
            return "let " + Variable + " = " + Value + ";";
        }

        //parsing by the format let variableName=expression
        public override void Parse(TokensStack sTokens)
        {
            if (sTokens.Count < 5)
                throw new SyntaxErrorException("Early termination ", sTokens.LastPop);
            Token tLet = sTokens.Pop();
            if (!(tLet is Statement) || ((Statement)tLet).Name != "let")
                throw new SyntaxErrorException("Expected let statment , received " + tLet, tLet);
            Token tName = sTokens.Pop();
            if (!(tName is Identifier))
                throw new SyntaxErrorException("Expected identifier name, received " + tName, tName);
            Variable = ((Identifier)tName).Name;
            Token tEqual = sTokens.Pop();
            if (!(tEqual is Operator) || ((Operator)tEqual).Name != '=')
                throw new SyntaxErrorException("Expected = , received " + tEqual, tEqual);
            Value = Expression.Create(sTokens);
            Value.Parse(sTokens);
            if (!(sTokens.Peek() is Separator) || ((Separator)sTokens.Peek()).Name != ';')
                throw new SyntaxErrorException("Expected ; , received ", sTokens.Peek());           
            sTokens.Pop();
        }
    }
}
