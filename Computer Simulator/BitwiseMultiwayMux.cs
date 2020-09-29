using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class BitwiseMultiwayMux : Gate
    {
        public int Size { get; private set; }
        public int ControlBits { get; private set; }
        public WireSet Output { get; private set; }
        public WireSet Control { get; private set; }
        public WireSet[] Inputs { get; private set; }

        private BitwiseMux[] bitwise_mux_operation;


        // in order to find the right input, for each level of inputs making mux operation with the corresponding control bit
        // for all inputs in that level, after that repeat the procedure for the results of the muxes.
        // to accomplish that I made array of bitwise mux , in the size of Inputs -1 , because there always be 1 less mux than the inputs

        public BitwiseMultiwayMux(int iSize, int cControlBits)
        {
            //initilaize 
            Size = iSize;
            ControlBits = cControlBits;
            Output = new WireSet(Size);
            Control = new WireSet(cControlBits);
            Inputs = new WireSet[(int)Math.Pow(2, cControlBits)];
            bitwise_mux_operation = new BitwiseMux[Inputs.Length - 1];

            for (int i = 0; i < Inputs.Length; i++)
            {
                Inputs[i] = new WireSet(Size);
            }

            for (int i = 0; i < bitwise_mux_operation.Length; i++)
            {
                bitwise_mux_operation[i] = new BitwiseMux(Size);
            }

            // make mux operation on the first level of inputs with the first control bit
            // so that the results will be stored in the first half of bitwise gates
            for (int i = 0; i < bitwise_mux_operation.Length/2+1; i++)
            {
                bitwise_mux_operation[i].ConnectControl(Control[0]);
                bitwise_mux_operation[i].ConnectInput1(Inputs[i * 2]);
                bitwise_mux_operation[i].ConnectInput2(Inputs[(i * 2) + 1]);
            }
             // iterates for each control bit (starting from the second) and saves each pair of outputs in inputs of bitwise mux
             // so that the end result is in the last bitwisemux
            int start_of_input = 0,end_of_input= bitwise_mux_operation.Length / 2 + 1,counter = 0;
            for (int contol_bit = 1; contol_bit < cControlBits; contol_bit++)
            {
                for (int out_bit = start_of_input; out_bit < end_of_input; out_bit = out_bit +2)
                {
                    bitwise_mux_operation[counter + end_of_input].ConnectControl(Control[contol_bit]);
                    bitwise_mux_operation[counter + end_of_input].ConnectInput1(bitwise_mux_operation[out_bit].Output);
                    bitwise_mux_operation[counter + end_of_input].ConnectInput2(bitwise_mux_operation[out_bit+1].Output);
                    counter++;
                }
                counter = 0;
                start_of_input = end_of_input;
                end_of_input = (end_of_input + (bitwise_mux_operation.Length - end_of_input) / 2) +1;
            }

            // connect the output
            Output.ConnectInput(bitwise_mux_operation[bitwise_mux_operation.Length-1].Output);
        }


        public void ConnectInput(int i, WireSet wsInput)
        {
            Inputs[i].ConnectInput(wsInput);
        }
        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }


        //checks each input from 1 to the len of Inputs
        public override bool TestGate()
        {
            Random ran = new Random();
            int value;
            for (int i = 0; i < (int)Math.Pow(2, ControlBits); i++)
            {
                value = ran.Next(0, (int)Math.Pow(2, ControlBits) - 1);
                Inputs[i].SetValue(value);
            }

            for (int i = 0; i < (int)Math.Pow(2, ControlBits); i++)
            {
                Control.SetValue(i);
                if (Output.GetValue() != Inputs[i].GetValue())
                    return false;
            }
            //Console.WriteLine("succes");
            return true;
            


        }
    }
}
