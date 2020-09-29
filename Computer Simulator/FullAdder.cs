using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class FullAdder : TwoInputGate
    {
        public Wire CarryInput { get; private set; }
        public Wire CarryOutput { get; private set; }

        public HalfAdder first_half_addr;

        public HalfAdder second_half_addr;

        public OrGate or_op;

        // full addr operation is accomplished by using 2 half addrs and or gate for the output carrier
        public FullAdder()
        {
            // initilaize the gates
            CarryInput = new Wire();
            CarryOutput = new Wire();
            first_half_addr = new HalfAdder();
            second_half_addr = new HalfAdder();
            or_op = new OrGate();

            //connecting the first addr
            first_half_addr.ConnectInput1(Input1);
            first_half_addr.ConnectInput2(Input2);

            //connecting the second addr
            second_half_addr.ConnectInput1(first_half_addr.Output);
            second_half_addr.ConnectInput2(CarryInput);

            //or_op for the carry output
            or_op.ConnectInput1(second_half_addr.CarryOutput);
            or_op.ConnectInput2(first_half_addr.CarryOutput);

            //connecting the outputs
            Output.ConnectInput(second_half_addr.Output);
            CarryOutput.ConnectInput(or_op.Output);

        }


        public override string ToString()
        {
            return Input1.Value + "+" + Input2.Value + " (C" + CarryInput.Value + ") = " + Output.Value + " (C" + CarryOutput.Value + ")";
        }

        public override bool TestGate()
        {
            Input1.Value = 0;
            Input2.Value = 0;
            CarryInput.Value = 0;

            if (Output.Value != 0 || CarryOutput.Value != 0)
            {
                return false;
            }
            Input1.Value = 0;
            Input2.Value = 0;
            CarryInput.Value = 1;

            if (Output.Value != 1 || CarryOutput.Value != 0)
            {
                return false;
            }
            Input1.Value = 0;
            Input2.Value = 1;
            CarryInput.Value = 0;

            if (Output.Value != 1 || CarryOutput.Value != 0)
            {
                return false;
            }
            Input1.Value = 0;
            Input2.Value = 1;
            CarryInput.Value = 1;

            if (Output.Value != 0 || CarryOutput.Value != 1)
            {
                return false;
            }

            Input1.Value = 1;
            Input2.Value = 0;
            CarryInput.Value = 0;

            if (Output.Value != 1 || CarryOutput.Value != 0)
            {
                return false;
            }
            Input1.Value = 1;
            Input2.Value = 0;
            CarryInput.Value = 1;

            if (Output.Value != 0 || CarryOutput.Value != 1)
            {
                return false;
            }

            Input1.Value = 1;
            Input2.Value = 1;
            CarryInput.Value = 0;

            if (Output.Value != 0 || CarryOutput.Value != 1)
            {
                return false;
            }

            Input1.Value = 1;
            Input2.Value = 1;
            CarryInput.Value = 1;

            if (Output.Value != 1 || CarryOutput.Value != 1)
            {
                return false;
            }
            return true;
        }
    }
}
