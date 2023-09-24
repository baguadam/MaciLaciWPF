using MaciLaci.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaciLaci.Model
{
    public class MaciLaciEventArgs : EventArgs
    {
        private FieldOptions[,] _fieldValue;
        private Boolean _isWon;
        private Int32 _collectedBaskets;
        private Int32 _gameTime;
        public Boolean IsWon { get { return _isWon; } }

        public Int32 GameTime { get { return _gameTime; } }
        public FieldOptions[,] FieldValue { get { return _fieldValue; } }

        public Int32 Collected { get { return _collectedBaskets; } } 

        public MaciLaciEventArgs(Boolean isWon, FieldOptions[,] fieldOptions, Int32 collectedBaskets, Int32 gameTime)
        {
            _isWon = isWon;
            _fieldValue = fieldOptions;
            _collectedBaskets = collectedBaskets;
            _gameTime = gameTime;
        }

    }
}
