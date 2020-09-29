using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class BitwiseAndGate : BitwiseTwoInputGate
    {

        private AndGate and_gate_per_bit;

        //iterates each bit of the 2 inputs and making or operation using "OR" gate - saves the output in Output
        public BitwiseAndGate(int iSize)
            : base(iSize)
        {
            
            for(int bit = 0; bit< iSize; bit++)
            {
                and_gate_per_bit = new AndGate();
                and_gate_per_bit.ConnectInput1(Input1[bit]);
                and_gate_per_bit.ConnectInput2(Input2[bit]);
                Output[bit].ConnectInput(and_gate_per_bit.Output);
            }
        }


        public override string ToString()
        {
            return "And " + Input1 + ", " + Input2 + " -> " + Output;
        }


        //checks all 3 possible input - full 1, full 0 and mixed
        public override bool TestGate()
        {
            String result = "[";
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = 1;
                Input2[i].Value = 1;
                result = result + "1";
            }
            result = result + "]";
            if (!Output.ToString().Equals(result))
            {
                return false;
            }

            result = "[";
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = 0;
                Input2[i].Value = 0;
                result = result + "0";
            }
            result = result + "]";
            if (!Output.ToString().Equals(result))
            {
                return false;
            }

            result = "[";
            for (int i = 0; i < Size; i++)
            {

                if (i % 2 == 0)
                {
                    Input1[i].Value = 0;
                    Input2[i].Value = 1;
                }
                else
                {
                    Input1[i].Value = 1;
                    Input2[i].Value = 0;
                }
                result = result + "0";

            }
            result = result + "]";
            if (!Output.ToString().Equals(result))
            {
                return false;
            }
            return true;
        }
    }
}
