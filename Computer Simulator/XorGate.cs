using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class XorGate : TwoInputGate
    {
        private NAndGate nand_input;
        private OrGate or_input;
        private AndGate and_output;

        //Xor gate is possible to get by combining with and gate the results of the nand and or gates.
        public XorGate()
        {
            nand_input = new NAndGate();
            or_input= new OrGate();
            and_output = new AndGate();

            nand_input.ConnectInput1(Input1);
            nand_input.ConnectInput2(Input2);

            or_input.ConnectInput1(Input1);
            or_input.ConnectInput2(Input2);


            and_output.ConnectInput1(nand_input.Output);
            and_output.ConnectInput2(or_input.Output);

            Output = and_output.Output;
        }

        public override string ToString()
        {
            return "Xor " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }


        //checks all possible combinations
        public override bool TestGate()
        {
            Input1.Value = 0;
            Input2.Value = 0;

            if (Output.Value != 0)
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

            if (Output.Value != 0)
            {

                return false;
            }

            return true;
        }
    }
}
