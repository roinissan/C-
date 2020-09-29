using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //this class represents a set of wires (a cable)
    class WireSet
    {
        private Wire[] m_aWires;
        public int Size { get; private set; }
        public Boolean InputConected { get; private set; }
        public Wire this[int i]
        {
            get
            {
                return m_aWires[i];
            }
        }
        
        public WireSet(int iSize)
        {
            Size = iSize;
            InputConected = false;
            m_aWires = new Wire[iSize];
            for (int i = 0; i < m_aWires.Length; i++)
                m_aWires[i] = new Wire();
        }
        public override string ToString()
        {
            string s = "[";
            for (int i = m_aWires.Length - 1; i >= 0; i--)
                s += m_aWires[i].Value;
            s += "]";
            return s;
        }

        //transform a positive integer value into binary and set the wires accordingly, with 0 being the LSB
        public void SetValue(int iValue)
        {
            for (int i = 0; i < m_aWires.Length; i++)
            {
                m_aWires[i].Value = iValue % 2;
                iValue = iValue / 2;
            }
            //Console.WriteLine(ToString());
        }

        //transform the binary code into a positive integer
        public int GetValue()
        {
            int result = 0;
            for (int i = 0; i < m_aWires.Length; i++)
            {
                result = result + (m_aWires[i].Value * (int)Math.Pow(2, i));
            }
           // Console.WriteLine(result);
            return result;

        }

        //transform an integer value into binary using 2`s complement and set the wires accordingly, with 0 being the LSB
        public void Set2sComplement(int iValue)
        {
            if (iValue >= 0)
                SetValue(iValue);
            else {
                iValue = iValue * -1;
                int[] bits_arr = new int[Size];
                int remainder;
                for(int i = 0; i < Size; i++)
                {
                    remainder = iValue % 2;
                    if (remainder == 1)
                        remainder = 0;
                    else
                        remainder = 1;
                    bits_arr[i] = remainder;
                    iValue = iValue / 2;
                }

                bits_arr = transform_to_2s_c(bits_arr);
                for (int i = 0; i < Size; i++)
                {
                    m_aWires[i].Value = bits_arr[i];
                }
            }
        //  Console.WriteLine(ToString());
        }

        //transform the binary code in 2`s complement into an integer
        public int Get2sComplement()
        {
            int result = 0;
            if (m_aWires[Size - 1].Value == 0)
                result = GetValue();
            else
            {
                
                int[] bits_arr = transform_values();
                bits_arr = transform_to_2s_c(bits_arr);
                for (int i = 0; i < Size; i++)
                {
                    result = result + (bits_arr[i] * (int)Math.Pow(2, i));
                }
                result = result* -1;
            }
           // Console.WriteLine(result);
            return result;
        }

        public void ConnectInput(WireSet wIn)
        {
            if (InputConected)
                throw new InvalidOperationException("Cannot connect a wire to more than one inputs");
            if(wIn.Size != Size)
                throw new InvalidOperationException("Cannot connect two wiresets of different sizes.");
            for (int i = 0; i < m_aWires.Length; i++)
                m_aWires[i].ConnectInput(wIn[i]);

            InputConected = true;
            
        }

        // saves the reverse the bit of the wire in new array and return it
        private int[] transform_values()
        {
            int[] array_of_bits = new int[Size];
            for (int i = 0; i < Size; i++)
            {
                if (m_aWires[i].Value == 1)
                    array_of_bits[i] = 0;
                else
                    array_of_bits[i] = 1;
            }
            return array_of_bits;
        }


        // complete the 2s complement procedure by adding 1 to the reverse bits
        private int[] transform_to_2s_c(int[] array_of_bits)
        {
            int add_1 = 1, carry = 0,sum;

            for (int i = 0; i < Size; i++)
            {
                if (i != 0)
                    add_1 = 0;
                sum = array_of_bits[i] + add_1 + carry;
                if (sum < 2)
                {
                    array_of_bits[i] = sum;
                    carry = 0;
                }
                else if (sum == 2)
                {
                    array_of_bits[i] = 0;
                    carry = 1;
                }
                else
                {
                    array_of_bits[i] = 1;
                    carry = 1;
                }
                sum = 0;
            }
            return array_of_bits;
        }

    }
}
