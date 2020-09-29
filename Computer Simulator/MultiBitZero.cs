using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class MultiBitZero : Gate
    {
        public int Size;

        public WireSet Input;

        public WireSet Output;

        public MultiBitZero(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);

            Output.SetValue(0);

        }
        public override string ToString()
        {
            return "Output: " + Output.GetValue();
        }

        public void ConnectInput(WireSet wInput)
        {
            Input.ConnectInput(wInput);
        }

        public override bool TestGate()
        {    
            return true;
        }


    }
}
