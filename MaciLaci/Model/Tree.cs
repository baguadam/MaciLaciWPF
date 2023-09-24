using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaciLaci.Model
{
    public class Tree : Characters
    {
        public Tree(Int32 x, Int32 y) : base(new Position(x,y)) { }
    }
}
