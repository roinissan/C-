using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class Memory : SequentialGate
    {
        public int AddressSize { get; private set; }
        public int WordSize { get; private set; }

        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        public WireSet Address { get; private set; }
        public Wire Load { get; private set; }

        public MultiBitRegister[] memory_cells;

        public BitwiseMultiwayDemux input_demux;

        public BitwiseMultiwayDemux input_demux_for_load;

        public BitwiseMultiwayMux output_mux;

        // wireset for the load because the bitwisa multibit demux input has to be wireset and not wire 
        public WireSet load_wireset;


        // creating memory component by using array of multi bit register
        // using 2 demux - one for the date input and one for the load input(to know if read/write)
        // using mux for the output
        public Memory(int iAddressSize, int iWordSize)
        {
            AddressSize = iAddressSize;
            WordSize = iWordSize;

            Input = new WireSet(WordSize);
            Output = new WireSet(WordSize);
            Address = new WireSet(AddressSize);
            Load = new Wire();
            load_wireset = new WireSet(1);
            

            memory_cells = new MultiBitRegister[(int)Math.Pow(2, AddressSize)];
            input_demux = new BitwiseMultiwayDemux(WordSize, AddressSize);
            input_demux_for_load = new BitwiseMultiwayDemux(1, AddressSize);
            output_mux = new BitwiseMultiwayMux(WordSize, AddressSize);
            

            for (int cell = 0; cell < memory_cells.Length; cell++)
            {
                memory_cells[cell] = new MultiBitRegister(WordSize);
            }

            load_wireset[0].ConnectInput(Load);

            input_demux.ConnectControl(Address);
            input_demux_for_load.ConnectControl(Address);
            output_mux.ConnectControl(Address);

            input_demux.ConnectInput(Input);
            input_demux_for_load.ConnectInput(load_wireset);

            for (int cell = 0; cell < memory_cells.Length; cell++)
            {
                memory_cells[cell].Load.ConnectInput(input_demux_for_load.Outputs[cell][0]);
                memory_cells[cell].ConnectInput(input_demux.Outputs[cell]);
                output_mux.Inputs[cell].ConnectInput(memory_cells[cell].Output);
            }

            Output.ConnectInput(output_mux.Output);
        }


        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }
        public void ConnectAddress(WireSet wsAddress)
        {
            Address.ConnectInput(wsAddress);
        }


        public override void OnClockUp()
        {
        }

        public override void OnClockDown()
        {
        }

        public override string ToString()
        {
            return ("");
        }

        // checking 16 inputs and outputs.
        public override bool TestGate()
        {
            Random ran = new Random();
            int value_to_insert, chosen_address; 
            value_to_insert = ran.Next(0, ((int)Math.Pow(2, WordSize) - 1));
            chosen_address = ran.Next(0, ((int)Math.Pow(2, AddressSize) - 1));
            for (int i = 0; i < 16; i++) {

                Input.SetValue(value_to_insert);
                Address.SetValue(chosen_address);
                if (i % 2 == 0)
                    Load.Value = 1;
                else
                    Load.Value = 0;
                Clock.ClockDown();
                Clock.ClockUp();
                if (i % 2 == 0 && Output.GetValue() != value_to_insert)
                    return false;
                else if (i % 2 != 0 && Output.GetValue() == value_to_insert)
                    return false;
                value_to_insert = ran.Next(0, ((int)Math.Pow(2, WordSize) - 1));
            }


            return true;
        }
    }
}
