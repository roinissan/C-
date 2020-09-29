using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class BitwiseDemux : Gate
    {
        public int Size { get; private set; }
        public WireSet Output1 { get; private set; }
        public WireSet Output2 { get; private set; }
        public WireSet Input { get; private set; }
        public Wire Control { get; private set; }

        private Demux demux_op;


        // checks each bit of the input and making demux operation on them
        // and connects the outputs
        public BitwiseDemux(int iSize)
        {
            Size = iSize;
            Control = new Wire();
            Input = new WireSet(Size);
            Output1 = new WireSet(Size);
            Output2 = new WireSet(Size);

            for (int bit = 0; bit < Size; bit++)
            {
                demux_op = new Demux();
                demux_op.ConnectInput(Input[bit]);
                demux_op.ConnectControl(Control);
                Output1[bit].ConnectInput(demux_op.Output1);
                Output2[bit].ConnectInput(demux_op.Output2);
            }
        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }
        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        //checks output when changing control bit
        public override bool TestGate()
        {
            Control.Value = 1;
            String first_string = "[";
            String second_string = "[";
            for (int i = 0; i < Size; i++)
            {
                Input[i].Value = 1;
                first_string = first_string + "1";
                second_string = second_string + "0";
            }
            first_string = first_string + "]";
            second_string = second_string + "]";
            if (!Output1.ToString().Equals(second_string) || !Output2.ToString().Equals(first_string))
                return false;

            Control.Value = 0;
            if (!Output1.ToString().Equals(first_string) || !Output2.ToString().Equals(second_string))
                return false;

            return true;
        }
    }
}
