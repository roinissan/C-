using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class BitwiseNotGate : Gate
    {
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        public int Size { get; private set; }

        private NotGate not_gate_operation;

        //iterating each bit of the input and using the "NOT" gate to reverse the bit.
        public BitwiseNotGate(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);

            for (int bit = 0; bit < iSize; bit++)
            {
                not_gate_operation = new NotGate();
                not_gate_operation.ConnectInput(Input[bit]);
                Output[bit].ConnectInput(not_gate_operation.Output);
            }
        }

        public void ConnectInput(WireSet ws)
        {
            Input.ConnectInput(ws);
        }


        public override string ToString()
        {
            return "Not " + Input + " -> " + Output;
        }

        //checks all 3 possible input - full 1, full 0 and mixed
        public override bool TestGate()
        {
            String result = "[";
            for (int i = 0; i < Size; i++)
            {
                Input[i].Value = 1;
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
                Input[i].Value = 0;
                result = result + "1";
            }
            result = result + "]";
            if (!Output.ToString().Equals(result)) { 

                return false;
                }

            result = "[";
            for (int i = 0; i < Size; i++)
            {
                if (i % 2 == 0)
                {
                    Input[i].Value = 0;
                    result = result + "1";
                }
                else
                {
                    Input[i].Value = 1;
                    result = result + "0";
                }
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
