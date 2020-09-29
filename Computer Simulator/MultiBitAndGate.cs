using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class MultiBitAndGate : MultiBitGate
    {
        private AndGate and_operator;
        private Wire result;
        private int size_of_wire;

        //making and operation for  2 consecutive bits(wires) until the  last bit. first bit is constant - 1
        public MultiBitAndGate(int iInputCount)
            : base(iInputCount)
        {
            size_of_wire = iInputCount;
            result = new Wire();
            result.Value = 1;
            
            for(int bit =0; bit< iInputCount; bit++)
            {
                and_operator = new AndGate();
                and_operator.ConnectInput1(result);
                and_operator.ConnectInput2(m_wsInput[bit]);
                result = and_operator.Output;
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

            if ( Output.Value != 0)
                return false;

            return true;
        }
    }
}
