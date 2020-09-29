using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class MultiBitOrGate : MultiBitGate
    {
        private OrGate or_operator;
        private Wire result;
        private int size_of_wire;

        //making or operation for  2 consecutive bits(wires) until the  last bit. first bit is constant - 0
        public MultiBitOrGate(int iInputCount)
            : base(iInputCount)
        {
            size_of_wire = iInputCount;
            result = new Wire();
            result.Value = 0;

            for (int bit = 0; bit < iInputCount; bit++)
            {
                or_operator = new OrGate();
                or_operator.ConnectInput1(result);
                or_operator.ConnectInput2(m_wsInput[bit]);
                result = or_operator.Output;
            }
            Output = result;

        }


        //checks all 3 possible input - full 1, full 0 and mixed
        public override bool TestGate()
        {
            for (int i = 0; i < size_of_wire; i++)
            {
                m_wsInput[i].Value = 1;
            }
            if (Output.Value != 1)
                return false;

            for (int i = 0; i < size_of_wire; i++)
            {
                m_wsInput[i].Value = 0;
            }

            if (Output.Value != 0)
                return false;

            for (int i = 0; i < size_of_wire; i++)
            {
                if (i % 2 == 0)
                    m_wsInput[i].Value = 0;
                else
                    m_wsInput[i].Value = 1;
            }

            if (size_of_wire > 1 && Output.Value != 1)
                return false;

            return true;
        }
    }
}
