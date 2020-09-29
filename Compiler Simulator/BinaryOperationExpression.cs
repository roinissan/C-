using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{

    public class BinaryOperationExpression : Expression
    {
        public string Operator { get;  set; }
        public Expression Operand1 { get;  set; }
        public Expression Operand2 { get;  set; }

        public override string ToString()
        {
            return "(" + Operator + " " + Operand1 + " " + Operand2 + ")";
        }

        //parsing by the format operand-expression-expression 
        public override void Parse(TokensStack sTokens)
        {
            Token t = sTokens.Pop(); //(
            if (sTokens.Count < 3 || !(sTokens.Peek(0) is Operator))
                throw new SyntaxErrorException("Expected Operator, recieved ", sTokens.Peek(0));
            Operator = ((Operator)sTokens.Pop()).Name.ToString();
            Operand1 = Expression.Create(sTokens);
            Operand1.Parse(sTokens);
            Operand2 = Expression.Create(sTokens);
            Operand2.Parse(sTokens);
            t = sTokens.Pop(); //)
        }
    }
}
