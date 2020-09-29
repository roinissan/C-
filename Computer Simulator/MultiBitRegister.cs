using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class MultiBitRegister : Gate
    {
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        public Wire Load { get; private set; }
        public int Size { get; private set; }

        public SingleBitRegister[] registers;


        // creating multi bit register by using array of single bit register and a mux
        public MultiBitRegister(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);
            Load = new Wire();
            registers = new SingleBitRegister[Size];

            for (int i = 0; i < Size; i++)
            {
                registers[i] = new SingleBitRegister();
            }

            for (int bit = 0; bit < Size; bit++)
            {
                registers[bit].ConnectLoad(Load);
                registers[bit].ConnectInput(Input[bit]);
                Output[bit].ConnectInput(registers[bit].Output);
           }


        }

        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        
        public override string ToString()
        {
            return Output.ToString();
        }

        //checking 2 possible outcomes
        public override bool TestGate()
        {
            Random ran = new Random();
            int value1,value2;
            value1 = ran.Next(0, ((int)Math.Pow(2, Size) - 1));
            Input.SetValue(value1);
            Load.Value = 0;
            Clock.ClockDown();
            Clock.ClockUp();
            Load.Value = 1;
            value2 = ran.Next(0, ((int)Math.Pow(2, Size) - 1));
            Clock.ClockDown();
            Clock.ClockUp();
            Input.SetValue(value2);
            if (Output.GetValue() != value1)
                return false;
            return true;
        }
    }
}
