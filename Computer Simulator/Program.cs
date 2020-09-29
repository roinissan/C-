using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class Program
    {
        static void Main(string[] args)
        {

            Memory test = new Memory(4,4);
            if (test.TestGate())
                Console.WriteLine("yes");
            else
                Console.WriteLine("false");


        }
    }
}
