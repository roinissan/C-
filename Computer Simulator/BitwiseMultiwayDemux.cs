using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class BitwiseMultiwayDemux : Gate
    {
        public int Size { get; private set; }
        public int ControlBits { get; private set; }
        public WireSet Input { get; private set; }
        public WireSet Control { get; private set; }
        public WireSet[] Outputs { get; private set; }

        private BitwiseDemux[] bitwise_demux_operation;


        // similar procudre to the multimux just the opposite - creates bitwise demux array
        // saves the 2 outputs as input of new bitwise demux
        // the cotrol bit are checking in reverse order
        public BitwiseMultiwayDemux(int iSize, int cControlBits)
        {
            //initilaize 
            Size = iSize;
            Input = new WireSet(Size);
            Control = new WireSet(cControlBits);
            Outputs = new WireSet[(int)Math.Pow(2, cControlBits)];
            bitwise_demux_operation = new BitwiseDemux[(int)Math.Pow(2, cControlBits) - 1];

            for (int i = 0; i < Outputs.Length; i++)
            {
                Outputs[i] = new WireSet(Size);
            }

            for (int i = 0; i < bitwise_demux_operation.Length; i++)
            {
                bitwise_demux_operation[i] = new BitwiseDemux(Size);
            }

            // making demux on the input with the last contol bit - to save in the array 
            bitwise_demux_operation[0].ConnectControl(Control[cControlBits - 1]);
            bitwise_demux_operation[0].ConnectInput(Input);

            // for each control bit making bitwise demux on all the outputs in that level
            // the outputs in last half of the bitwise demux array are the final outputs
            int start_of_input = 0, end_of_input = 1, counter = 0, iterator = 1;
            for (int contol_bit = cControlBits - 2; contol_bit > -1 ; contol_bit--)
            {
                for (int out_bit = start_of_input; out_bit < end_of_input; out_bit++)
                {
                    bitwise_demux_operation[counter + end_of_input].ConnectControl(Control[contol_bit]);
                    bitwise_demux_operation[counter + end_of_input + 1].ConnectControl(Control[contol_bit]);
                    bitwise_demux_operation[counter + end_of_input].ConnectInput(bitwise_demux_operation[out_bit].Output1);
                    bitwise_demux_operation[counter + end_of_input + 1].ConnectInput(bitwise_demux_operation[out_bit].Output2);
                    counter= counter +2;
                }
                counter = 0;
                start_of_input = end_of_input;
                end_of_input = end_of_input + 2* iterator;
                iterator++;
        
            }

            // connects the outputs
            counter = 0;
            for (int start = bitwise_demux_operation.Length/2; start < bitwise_demux_operation.Length; start++)
            {
                Outputs[counter].ConnectInput(bitwise_demux_operation[start].Output1);
                Outputs[counter+1].ConnectInput(bitwise_demux_operation[start].Output2);
                counter = counter + 2;
            }


        }


        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }
        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }

        // checks for each value in range if its in right out when changing the control bit
        public override bool TestGate()
        {
            Random ran = new Random();
            int value;
            for (int i = 0; i < (int)Math.Pow(2, ControlBits); i++)
            {
                value = ran.Next(0, (int)Math.Pow(2, ControlBits) - 1);
                Input.SetValue(value);
                Control.SetValue(i);
                for (int j = 0; j < (int)Math.Pow(2, ControlBits); j++)
                {
                    if (j != i)
                    {
                        if (Outputs[j].GetValue() != 0)
                            return false;
                    }
                    else
                    {       
                        if (Outputs[j].GetValue() != Input.GetValue())
                            return false;
                    }
                }
            }
            //Console.WriteLine("success");
            return true;
        }
    }
}
