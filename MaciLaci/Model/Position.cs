using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaciLaci.Model
{
    public class Position
    {
        private int _x;
        private int _y;

        public int PositionX { get => _x; set { _x = value; } }
        public int PositionY { get => _y; set { _y = value; } }

        public Position() { }

        public Position(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }
}
