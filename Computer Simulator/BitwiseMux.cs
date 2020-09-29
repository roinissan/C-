using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class BitwiseMux : BitwiseTwoInputGate
    {
        public Wire ControlInput { get; private set; }

        private MuxGate mux_operation;

        // checks each corisponding bit of the 2 inputs and making mux operation on them
        // and connects the output
        public BitwiseMux(int iSize)
            : base(iSize)
        {
            ControlInput = new Wire();

            for (int bit = 0; bit < Size; bit++)
            {
                mux_operation = new MuxGate();
                mux_operation.ConnectControl(ControlInput);
                mux_operation.ConnectInput1(Input1[bit]);
                mux_operation.ConnectInput2(Input2[bit]);

                Output[bit].ConnectInput(mux_operation.Output);

            }
        }

        public void ConnectControl(Wire wControl)
        {
            ControlInput.ConnectInput(wControl);
        }



        public override string ToString()
        {
            return "Mux " + Input1 + "," + Input2 + ",C" + ControlInput.Value + " -> " + Output;
        }



        // checks input
        public override bool TestGate()
        {
            ControlInput.Value = 0;
            String first_string = "[";
            String second_string = "[";
            for (int i = 0; i < Size; i++)
            {
                Input2[i].Value = 1;
                Input1[i].Value = 0;
                first_string = first_string + "0";
                second_string = second_string + "1";
            }
            first_string = first_string + "]";
            second_string = second_string + "]";
            if (!Output.ToString().Equals(first_string))
                return false;
            ControlInput.Value = 1;
            if (! Output.ToString().Equals(second_string))
                return false;

            return true;
        }
    }
}
