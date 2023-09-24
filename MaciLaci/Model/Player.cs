using MaciLaci.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MaciLaci.Model
{
    public class Player : Characters
    {

        private int _collectedBaskets;

        public int CollectedBaskets { get { return _collectedBaskets; } set { _collectedBaskets = value; } }

        public Player() : base(new Position(0, 0))
        {
            _collectedBaskets = 0;
        }

        public Player(int x, int y) : base(new Position(x, y)) { }

        public Boolean CollectBasket(Table table)
        {
            if (table.GetValue(Position.PositionX, Position.PositionY) == FieldOptions.BASKET)
            {
                _collectedBaskets++;
                return true;
            }
            return false;
        }

        public void TakeStep(Table table, Direction direction)
        {
            switch (direction)
            {
                case Direction.UP:
                    if (table.IsFreeOrBasket(_position.PositionX - 1, _position.PositionY))
                    {
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.FREE);
                        _position.PositionX -= 1;
                        CollectBasket(table);
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.PLAYER);
                    }
                    break;
                case Direction.DOWN:
                    if (table.IsFreeOrBasket(_position.PositionX + 1, _position.PositionY))
                    {
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.FREE);
                        _position.PositionX += 1;
                        CollectBasket(table);
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.PLAYER);
                    }
                    break;
                case Direction.LEFT:
                    if (table.IsFreeOrBasket(_position.PositionX, _position.PositionY - 1))
                    {
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.FREE);
                        _position.PositionY -= 1;
                        CollectBasket(table);
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.PLAYER);
                    }
                    break;
                case Direction.RIGHT:
                    if (table.IsFreeOrBasket(_position.PositionX, _position.PositionY + 1))
                    {
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.FREE);
                        _position.PositionY += 1;
                        CollectBasket(table);
                        table.SetValue(_position.PositionX, _position.PositionY, FieldOptions.PLAYER);
                    }
                    break;
            }
        }

    }
}
