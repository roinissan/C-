using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCompiler
{
    public class Identifier : Token
    {
        public string Name { get; set; }
        public Identifier(string name, int line, int position)
        {
            Line = line;
            Position = position;
            Name = name;

            if (name[0] < 'A' || name[0] > 'z')
            {
                throw new SyntaxErrorException("Wrong syntax", this);

            }
        }
        public override bool Equals(object obj)
        {
            if (obj is Identifier)
            {
                Identifier t = (Identifier)obj;
                if (Name == t.Name)
                    return base.Equals(t);
            }
            return false;
        }
        public override string ToString()
        {
            return Name;
        }

    }
}
