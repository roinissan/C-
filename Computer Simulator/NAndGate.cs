using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //this gate is given to you fully implemented - you only have to use it in your code
    class NAndGate : TwoInputGate
    {
        private AndGate and_input;
        private NotGate not_output;

        //nand gate is defines as not-AND gate, the procedure is completed using the AND,NOT gates.
        public NAndGate()
        {
            and_input = new AndGate();
            not_output = new NotGate();

            and_input.ConnectInput1(Input1);
            and_input.ConnectInput2(Input2);

            not_output.ConnectInput(and_input.Output);

            Output = not_output.Output;
        }

        public override string ToString()
        {
            return "NAnd " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }

        //checks all possible combinations
        public override bool TestGate()
        {
            Input1.Value = 0;
            Input2.Value = 0;

            if (Output.Value != 1)
            {

                return false;
            }
            Input1.Value = 0;
            Input2.Value = 1;

            if (Output.Value != 1)
            {

                return false;
            }
            Input1.Value = 1;
            Input2.Value = 0;

            if (Output.Value != 1)
            {

                return false;
            }
            Input1.Value = 1;
            Input2.Value = 1;

            if(Output.Value != 0)
            {

                return false;
            }
            return true;


        }

        
    }
}
