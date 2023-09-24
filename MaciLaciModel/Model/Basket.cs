using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaciLaci.Model
{
    public class Basket : Characters
    {
        public Basket(int x, int y) : base(new Position(x, y)) { }
    }
}
