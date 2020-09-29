using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class MuxGate : TwoInputGate
    {
        public Wire ControlInput { get; private set; }

        private OrGate or_output;

        private AndGate and_input1;

        private AndGate and_input2;

        private NotGate not_contolinput_input1;

        // accoplish using 2 "AND", "OR" and "NOT" gates,
        // first connect input 1 and "NOT" control bit to "AND" gate 1, then connect input 2 and control bit to "AND" gate 2
        // lastly connect the outputs of "AND" gates to "OR" gate
        public MuxGate()
        {

            ControlInput = new Wire();

            // initilaize the gates
            and_input1 = new AndGate();
            and_input2 = new AndGate();
            not_contolinput_input1 = new NotGate();
            or_output = new OrGate();

            //step 1
            not_contolinput_input1.ConnectInput(ControlInput);
            and_input1.ConnectInput1(Input1);
            and_input1.ConnectInput2(not_contolinput_input1.Output);

            //step 2
            and_input2.ConnectInput1(Input2);
            and_input2.ConnectInput2(ControlInput);

            //step 3
            or_output.ConnectInput1(and_input1.Output);
            or_output.ConnectInput2(and_input2.Output);
            Output = or_output.Output;

        }

        public void ConnectControl(Wire wControl)
        {
            ControlInput.ConnectInput(wControl);
        }


        public override string ToString()
        {
            return "Mux " + Input1.Value + "," + Input2.Value + ",C" + ControlInput.Value + " -> " + Output.Value;
        }


        // checks all possible inputs
        public override bool TestGate()
        {
            ControlInput.Value = 0;
            Input1.Value = 0;
            Input2.Value = 0;
            if (Output.Value != 0)
                return false;

            ControlInput.Value = 0;
            Input1.Value = 0;
            Input2.Value = 1;
            if (Output.Value != 0)
                return false;

            ControlInput.Value = 0;
            Input1.Value = 1;
            Input2.Value = 0;
            if (Output.Value != 1)
                return false;

            ControlInput.Value = 0;
            Input1.Value = 1;
            Input2.Value = 1;
            if (Output.Value != 1)
                return false;

            ControlInput.Value = 1;
            Input1.Value = 0;
            Input2.Value = 0;
            if (Output.Value != 0)
                return false;
            
            ControlInput.Value = 1;
            Input1.Value = 1;
            Input2.Value = 0;
            if (Output.Value != 0)
                return false;
      
            ControlInput.Value = 1;
            Input1.Value = 0;
            Input2.Value = 1;
            if (Output.Value != 1)
                return false;
            
            ControlInput.Value = 1;
            Input1.Value = 1;
            Input2.Value = 1;
            if (Output.Value != 1)
                return false;
            
            return true;
        }
    }
}
