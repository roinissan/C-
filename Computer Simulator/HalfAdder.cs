using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class HalfAdder : TwoInputGate
    {
        public Wire CarryOutput { get; private set; }

        public XorGate xor_op;

        public AndGate and_op;
       
        // creating the halfadder by using the "xor" and "and gates" for the output and the carry
        public HalfAdder()
        {
            // initilaize the gates
            CarryOutput = new Wire();
            xor_op = new XorGate();
            and_op = new AndGate();

            //xor operation for the sum output
            xor_op.ConnectInput1(Input1);
            xor_op.ConnectInput2(Input2);

            //and operation for the carry output
            and_op.ConnectInput1(Input1);
            and_op.ConnectInput2(Input2);

            //connecting outputs
            Output.ConnectInput(xor_op.Output);
            CarryOutput.ConnectInput(and_op.Output);
        }


        public override string ToString()
        {
            return "HA " + Input1.Value + "," + Input2.Value + " -> " + Output.Value + " (C" + CarryOutput + ")";
        }

        public override bool TestGate()
        {
            Input1.Value = 0;
            Input2.Value = 0;

            if (Output.Value != 0 || CarryOutput.Value != 0)
            {
                return false;
            }
            Input1.Value = 0;
            Input2.Value = 1;

            if (Output.Value != 1 || CarryOutput.Value != 0)
            {
                return false;
            }
            Input1.Value = 1;
            Input2.Value = 0;

            if (Output.Value != 1 || CarryOutput.Value != 0)
            {
                return false;
            }
            Input1.Value = 1;
            Input2.Value = 1;

            if (Output.Value != 0 || CarryOutput.Value != 1)
            {
                return false;
            }
            return true;
        }
    }
}
