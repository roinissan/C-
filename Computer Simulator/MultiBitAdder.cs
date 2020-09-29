using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class MultiBitAdder : Gate
    {
        public int Size { get; private set; }
        public WireSet Input1 { get; private set; }
        public WireSet Input2 { get; private set; }
        public WireSet Output { get; private set; }
        public Wire Overflow { get; private set; }

        public FullAdder[] full_addr_op_array;


        // adding 2 binary numbers by using full adder for each bit
        public MultiBitAdder(int iSize)
        {
            //initilaize
            Size = iSize;
            Input1 = new WireSet(Size);
            Input2 = new WireSet(Size);
            Output = new WireSet(Size);
            Overflow = new Wire();
            // one extra full addr for the overflow bit
            full_addr_op_array = new FullAdder[Size+1];

            for (int i = 0; i < Size+1; i++) {
                full_addr_op_array[i] = new FullAdder();
            }

            //carry in the first bit is 0
            full_addr_op_array[0].CarryInput.Value = 0;

            for (int bit = 0; bit < Size; bit++) {
                full_addr_op_array[bit].ConnectInput1(Input1[bit]);
                full_addr_op_array[bit].ConnectInput2(Input2[bit]);

                Output[bit].ConnectInput(full_addr_op_array[bit].Output);
                full_addr_op_array[bit+1].CarryInput.ConnectInput(full_addr_op_array[bit].CarryOutput);        
            }
            Overflow.ConnectInput(full_addr_op_array[Size].CarryInput);

        }

        public override string ToString()
        {
            return Input1 + "(" + Input1.Get2sComplement() + ")" + " + " + Input2 + "(" + Input2.Get2sComplement() + ")" + " = " + Output + "(" + Output.Get2sComplement() + ")";
        }

        public void ConnectInput1(WireSet wInput)
        {
            Input1.ConnectInput(wInput);
        }
        public void ConnectInput2(WireSet wInput)
        {
            Input2.ConnectInput(wInput);
        }


        public override bool TestGate()
        {
            int value1;
            int value2;
            Random ran = new Random();
            for (int i = 0; i < 4; i++)
            {
                value1 = ran.Next(0, ((int)Math.Pow(2, Size)-1)/2);
                value2 = ran.Next(0, ((int)Math.Pow(2, Size) - 1) / 2);
                Input1.SetValue(value1);
                Input2.SetValue(value2);
                //Console.WriteLine(ToString());
                if (Output.GetValue() != value1 + value2)
                    return false;
            }
        return true;
        }
    }
}
