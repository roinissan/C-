using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class SingleBitRegister : Gate
    {
 
        public Wire Input { get; private set; }
        public Wire Output { get; private set; }
        public Wire Load { get; private set; }

        public DFlipFlopGate flip_flop;

        public MuxGate mux_op;


        //creating single bit register by using DFF and a mux
        public SingleBitRegister()
        {

            Input = new Wire();
            Load = new Wire();
            Output = new Wire();
            mux_op = new MuxGate();
            flip_flop = new DFlipFlopGate();
            mux_op.ConnectControl(Load);
            mux_op.ConnectInput1(flip_flop.Output);
            mux_op.ConnectInput2(Input);      
            flip_flop.ConnectInput(mux_op.Output);  
            Output.ConnectInput(flip_flop.Output);


        }

        public void ConnectInput(Wire wInput)
        {
            Input.ConnectInput(wInput);
        }

      

        public void ConnectLoad(Wire wLoad)
        {
            Load.ConnectInput(wLoad);
        }

        //checking 2 possible outcomes
        public override bool TestGate()
        {

            Input.Value = 1;   
            Load.Value = 0;
            Input.Value = 0;
            if (Output.Value != 1)
                return false;

            Load.Value = 0;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.Value != 0)
                return false;

            return true;
        }
    }
}
