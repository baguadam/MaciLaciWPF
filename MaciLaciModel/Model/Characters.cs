using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaciLaci.Model
{
    public enum Direction { UP, DOWN, LEFT, RIGHT };
    public abstract class Characters
    {
        protected Position _position;

        public Position Position { get { return _position; } set => _position = value; }

        public Characters(Position? actuelPosition)
        {
            if (actuelPosition == null)
            {
                throw new ArgumentNullException(nameof(actuelPosition));
            }
            _position = actuelPosition;
        }
    }
}
