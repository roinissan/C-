using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class Counter : Gate
    {
        private int m_iValue;
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        public Wire Load { get; private set; }
        public int Size { get; private set; }

        public MultiBitRegister register;

        public BitwiseMux mux_op;

        public MultiBitAdder adder;

        public WireSet val;

        // creating counter by using one multibit register and 1 full addr to increment by 1
        // using mux to decide if to increment or to get new input
        public Counter(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);
            Load = new Wire();
            val = new WireSet(Size);
            val.SetValue(1);

            mux_op = new BitwiseMux(Size);
            adder = new MultiBitAdder(Size);
            register = new MultiBitRegister(Size);

            register.Load.Value = 1;

            mux_op.ConnectControl(Load);
            mux_op.ConnectInput1(adder.Output);
            mux_op.ConnectInput2(Input);
            register.ConnectInput(mux_op.Output);
            Output.ConnectInput(register.Output);

            adder.ConnectInput1(register.Output);
            adder.ConnectInput2(val);


        }

        public void ConnectInput(WireSet ws)
        {
            Input.ConnectInput(ws);
        }
        public void ConnectLoad(Wire w)
        {
            Load.ConnectInput(w);
        }
        

        public override string ToString()
        {
            return Output.ToString();
        }

        
        // checking increment 3 times and then changing the input
        public override bool TestGate()
        {
            Load.Value = 1;
            Input.SetValue(0);
            Clock.ClockDown();
            Clock.ClockUp();
            Load.Value = 0;
            Clock.ClockDown();
            Clock.ClockUp();
            Input.SetValue(0);
            Clock.ClockDown();
            Clock.ClockUp();
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.GetValue() != 3)
                return false;
            Load.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.GetValue() != 0)
                return false;
            return true;
        }
    }
}
