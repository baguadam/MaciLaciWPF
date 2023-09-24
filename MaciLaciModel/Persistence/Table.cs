using MaciLaci.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace MaciLaci.Persistence

{
    #region Enumerators
    public enum FieldOptions { FREE, PLAYER, COP, TREE, BASKET };
    public enum TableSize { SMALL = 6, MEDIUM = 8, LARGE = 9 };
    #endregion
    public class Table
    {
        #region Fields
        private FieldOptions[,] _fieldValue;
        private int _size;

        public FieldOptions[,] FieldValues { get { return _fieldValue; } }
        public Int32 Size { get { return _size;  } }

        #endregion

        #region Constructors
        public Table(Int32 tableSize)
        {
            if (tableSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tableSize), "The size of table must be at least 5");
            }

            _fieldValue = new FieldOptions[tableSize, tableSize];
            _size = tableSize;

        }
        #endregion

        #region Public methods
        public FieldOptions GetValue(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fieldValue.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "X is out of range!");
            }
            if (y < 0 || y >= _fieldValue.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "Y is out of range!");
            }

            return _fieldValue[x, y];
        }

        public void SetValue(Int32 x, Int32 y, FieldOptions value)
        {
            if (x < 0 || x >= _fieldValue.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "X is out of range!");
            }
            if (y < 0 || y >= _fieldValue.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "Y is out of range!");
            }

            _fieldValue[x, y] = value;
        }

        public Boolean OutOfTable(Int32 x, Int32 y)
        {
            if (x < 0
                || y < 0
                || x >= _fieldValue.GetLength(0)
                || y >= _fieldValue.GetLength(1))
            {
                return true;
            }
            return false;
        }

        public Boolean IsReturnStep(Int32 x, Int32 y)
        {
            if (GetValue(x, y) != FieldOptions.FREE
                || x < 0
                || y < 0
                || x >= _fieldValue.GetLength(0)
                || y >= _fieldValue.GetLength(1))
            {
                return true;
            }

            return false;
        }

        public Boolean IsFree(Int32 x, Int32 y)
        {
            if (!OutOfTable(x,y) 
                && GetValue(x, y) == FieldOptions.FREE)
            {
                return true;
            }
            return false;
        }

        public Boolean IsFreeForCop(Int32 x, Int32 y)
        {
            for (Int32 i = 0; i < 3; i++)
            {
                for (Int32 j = 0; j < 3; j++)
                {
                    if (!OutOfTable(x + (i - 1), y + (j - 1)))
                    {
                        if (GetValue(x + (i - 1), y + (j - 1)) != FieldOptions.FREE)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public Boolean IsFreeOrBasket(Int32 x, Int32 y)
        {
            if (!OutOfTable(x, y) && 
                (GetValue(x,y) == FieldOptions.FREE || GetValue(x,y) == FieldOptions.BASKET))
            {
                return true;
            }
            return false;
        }

        public Boolean CloseToStartingPlayer(Int32 radius, Int32 x, Int32 y)
        {
            if ((x <= radius) && (y <= radius))
            {
                return true;
            }
            return false;
        }
        #endregion

    }   
    
}
