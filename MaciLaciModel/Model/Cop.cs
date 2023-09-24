using MaciLaci.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaciLaci.Model
{
    public class Cop : Characters
    {

        private Direction _direction;

        public Direction Direction { get => _direction; set => _direction = value; }

        public Cop(Int32 x, Int32 y) : base(new Position(x,y))
        {
            Random random = new Random();
            _direction = (Direction)random.Next(4);

        }

        public Boolean CopCathes(Table table)
        {
            for (Int32 i = 0; i < 3; i++)
            {
                for (Int32 j = 0; j < 3; j++)
                {
                    if (!table.OutOfTable(_position.PositionX + (i - 1), _position.PositionY + (j - 1)))
                    {
                        if (table.GetValue(_position.PositionX + (i - 1), _position.PositionY + (j - 1)) == FieldOptions.PLAYER)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void TakeStep(Table table)
        {
            switch(_direction)
            {
                case Direction.UP:
                    if (!Turn(table, Position.PositionX - 1, Position.PositionY))
                    {
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.FREE);
                        _position.PositionX -= 1;
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.COP);
                    }
                    break;
                case Direction.DOWN:
                    if (!Turn(table, Position.PositionX + 1, Position.PositionY))
                    {
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.FREE);
                        Position.PositionX += 1;
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.COP);
                    }
                    break;
                case Direction.LEFT:
                    if (!Turn(table, Position.PositionX, Position.PositionY - 1))
                    {
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.FREE);
                        Position.PositionY -= 1;
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.COP);
                    }
                    break;
                case Direction.RIGHT:
                    if (!Turn(table, Position.PositionX, Position.PositionY + 1))
                    {
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.FREE);
                        Position.PositionY += 1;
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.COP);
                    }
                    break;
            }
        }

        private Boolean Turn(Table table, Int32 x, Int32 y)
        {
            if (table.OutOfTable(x, y) || table.IsReturnStep(x, y))
            {
                switch(_direction)
                {
                    case Direction.UP:
                        _direction = Direction.DOWN;
                        return true;
                    case Direction.DOWN:
                        _direction = Direction.UP;
                        return true;
                    case Direction.LEFT:
                        _direction = Direction.RIGHT;
                        return true;
                    case Direction.RIGHT:
                        _direction = Direction.LEFT;
                        return true;
                }
            }
            return false;
        }
    }
}
