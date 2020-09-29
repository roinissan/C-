using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class Demux : Gate
    {
        public Wire Output1 { get; private set; }
        public Wire Output2 { get; private set; }
        public Wire Input { get; private set; }
        public Wire Control { get; private set; }

        private AndGate and_output1;

        private AndGate and_output2;

        private NotGate not_contolinput_input1;


        // accoplish using 2 "AND" and "NOT" gates,
        // first connect input and "NOT" control bit to "AND" gate 1, then connect input and control bit to "AND" gate 2
        // lastly connect the outputs 
        public Demux()
        {
            
            Input = new Wire();
            Control = new Wire();
            Output1 = new Wire();
            Output2 = new Wire();

            // initilaize the gates        
            and_output1 = new AndGate();
            and_output2 = new AndGate();
            not_contolinput_input1 = new NotGate();

            //step 1
            not_contolinput_input1.ConnectInput(Control);
            and_output1.ConnectInput1(Input);
            and_output1.ConnectInput2(not_contolinput_input1.Output);

            //step 2
            and_output2.ConnectInput1(Input);
            and_output2.ConnectInput2(Control);

            //step 3
            Output1 = and_output1.Output;
            Output2 = and_output2.Output;

        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }
        public void ConnectInput(Wire wInput)
        {
            Input.ConnectInput(wInput);
        }


        //checks inputs
        public override bool TestGate()
        {
            Control.Value = 0;
            Input.Value = 0;
            if (Output1.Value != 0 || Output2.Value != 0)
            {
                return false;
            }

            Control.Value = 0;
            Input.Value = 1;
            if (Output1.Value != 1 || Output2.Value != 0)
            {
                return false;
            }
            Control.Value = 1;
            Input.Value = 0;
            if (Output1.Value != 0 || Output2.Value != 0)
            {
                Console.WriteLine("demux 3 failed");
                return false;
            }
            Control.Value = 1;
            Input.Value = 1;

            if (Output1.Value != 0 || Output2.Value != 1)
            {
                return false;
            }

            return true;
        }
    }
}
