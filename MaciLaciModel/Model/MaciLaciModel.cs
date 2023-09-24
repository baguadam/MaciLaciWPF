using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaciLaci.Persistence;
using System.ComponentModel;
using MaciLaci.Model;
using ConsoleMaciLaci.Persistence;

namespace MaciLaci.Model
{
    public class MaciLaciModel
    {

        #region Size constants
        private const Int32 GeneratedFieldSmall = 6;
        private const Int32 GeneratedFieldMedium = 8;
        private const Int32 GeneratedFieldLarge = 9;
        #endregion

        #region Fields
        private IMaciLaciDataAccess _dataAccess;
        private List<Cop> _copPosition;
        private List<Tree> _treePosition;
        private List<Basket> _basketPosition;
        private Player _player;
        private Table _table;
        private Boolean _gamePaused;

        private Int32 _basketNumber;
        private TableSize _tableSize;
        private Int32 _actualSize;
        private Int32 _gameTime;
        #endregion

        #region Properties
        public List<Cop> CopPosition { get => _copPosition; set => _copPosition = value; }
        public List<Tree> TreePosition { get => _treePosition; set => _treePosition = value; }
        public List<Basket> BasketPosition { get => _basketPosition; set => _basketPosition = value;
        }
        public Player Player { get => _player; set => _player = value; }
        public Table GameTable { get { return _table; } set { _table = value;  } }
        public TableSize TableSize { get { return _tableSize; } set { _tableSize = value; } }

        public Int32 BasketNumber { get { return _basketNumber; } set => _basketNumber = value; }
        public Int32 ActualSize { get { return _actualSize; } }

        public Int32 GameTime { get { return _gameTime; } }

        public Boolean GamePaused { get => _gamePaused; private set => _gamePaused = value; }
        #endregion

        #region Constructors
        public MaciLaciModel(IMaciLaciDataAccess dataAccess)
        {
            _gameTime = 0;
            _dataAccess = dataAccess;
            _tableSize = TableSize.MEDIUM;
            _actualSize = (int)_tableSize;
            _table = new Table(_actualSize);
            _copPosition = new List<Cop>();
            _treePosition = new List<Tree>();
            _basketPosition = new List<Basket>();
            _player = new Player();
            _gamePaused = false;
        }
        #endregion

        #region Events
        public event EventHandler<MaciLaciEventArgs>? GameOver;
        public event EventHandler<MaciLaciEventArgs>? GameAdvancedPlayer;
        public event EventHandler<MaciLaciEventArgs>? GameAdvancedCops;
        public event EventHandler<MaciLaciEventArgs>? PlayerCollect;
        public event EventHandler<MaciLaciEventArgs>? GameAdvancedTime;
        #endregion

        #region Private methods
        private void SetStartingTable()
        {
            for (int i = 0; i < _actualSize; i++)
            {
                for (int j = 0; j < _actualSize; j++)
                {
                    _table.SetValue(i, j, FieldOptions.FREE);
                }
            }
            _table.SetValue(0, 0, FieldOptions.PLAYER);
        }

        private void Generate(Int32 cop, Int32 basket, Int32 tree)
        {
            Random random = new Random();

            for (Int32 i = 0; i < basket; i++)
            {
                Int32 x, y;

                do
                {
                    x = random.Next(_actualSize);
                    y = random.Next(_actualSize);
                } while (_table.CloseToStartingPlayer(1, x, y) || !_table.IsFree(x, y));

                Basket basketCharacter = new Basket(x, y);
                _basketPosition.Add(basketCharacter);
                _table.SetValue(x, y, FieldOptions.BASKET);
            }

            for (Int32 i = 0; i < tree; i++)
            {
                Int32 x, y;

                do
                {
                    x = random.Next(_actualSize);
                    y = random.Next(_actualSize);
                } while (_table.CloseToStartingPlayer(1, x, y) || !_table.IsFree(x, y));

                Tree treeCharacter = new Tree(x, y);
                _treePosition.Add(treeCharacter);
                _table.SetValue(x, y, FieldOptions.TREE);
            }

            for (Int32 i = 0; i < cop; i++)
            {
                Int32 x, y;

                do
                {
                    x = random.Next(_actualSize);
                    y = random.Next(_actualSize);
                } while (_table.CloseToStartingPlayer(2, x, y) || !_table.IsFree(x, y) || !_table.IsFreeForCop(x, y));

                Cop copCharacter = new Cop(x, y);
                _copPosition.Add(copCharacter);
                _table.SetValue(x, y, FieldOptions.COP);
            }
        }

        private Boolean AnyCopCathes()
        {
            foreach (Cop cop in _copPosition)
            {
                if (cop.CopCathes(_table))
                {
                    OnGameOver(false);
                    return true;
                }
            }
            return false;
        }

        private Boolean IsGameOver()
        {
            foreach (Cop cop in _copPosition)
            {
                if (cop.CopCathes(_table))
                {
                    return true;
                }
            }
            if (_player.CollectedBaskets == _basketNumber)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Public methods
        public void NewGame()
        {
            _gameTime = 0;
            _player = new Player();
            _copPosition.Clear();
            _basketPosition.Clear();
            _treePosition.Clear();
            _actualSize = (int)_tableSize;
            _gamePaused = false;

            switch (_tableSize)
            {
                case TableSize.SMALL:
                    _table = new Table(GeneratedFieldSmall);
                    _basketNumber = 2;
                    SetStartingTable();
                    Generate(1, 2, 2);
                    break;
                case TableSize.MEDIUM:
                    _table = new Table(GeneratedFieldMedium);
                    _basketNumber = 3;
                    SetStartingTable();
                    Generate(2, 3, 3);
                    break;
                case TableSize.LARGE:
                    _table = new Table(GeneratedFieldLarge);
                    _basketNumber = 4;
                    SetStartingTable();
                    Generate(3, 4, 4);
                    break;
            }
        }

        public void PauseGame()
        {
            _gamePaused = true;
        }

        public void ContinueGame()
        {
            _gamePaused = false;
        }

        public void AdvanceTime()
        {
            if (IsGameOver() || _gamePaused)
            {
                return;
            }

            _gameTime++;
            OnGameTimeAdvanced();
        }

        #region Game loading and saving methods
        public void LoadGame(String path)
        {
            if (_dataAccess == null)
            {
                throw new InvalidOperationException("No data!");
            }

            _gameTime = 0;
            _table = _dataAccess.Load(path);
            _copPosition.Clear();
            _basketPosition.Clear();
            _treePosition.Clear();
            _actualSize = _table.Size;
            _tableSize = (TableSize)_actualSize;
            _gamePaused = false;
            int basketNum = 0;

            for (int i = 0; i < _actualSize; i++)
            {
                for (int j = 0; j < _actualSize; j++)
                {
                    switch(_table.GetValue(i, j))
                    {
                        case FieldOptions.PLAYER:
                            _player = new Player(i, j);
                            break;
                        case FieldOptions.COP:
                            _copPosition.Add(new Cop(i, j));
                            break;
                        case FieldOptions.TREE:
                            _treePosition.Add(new Tree(i, j));
                            break ;
                        case FieldOptions.BASKET:
                            basketNum++;
                            _basketPosition.Add(new Basket(i, j));
                            break;
                        default:
                            break;
                    }
                }
            }

            _basketNumber = basketNum;

        }

        public void SaveGame(String path)
        {
            if (_dataAccess == null)
            {
                throw new InvalidOperationException("No data access is provided!");
            }
            _dataAccess.Save(path, _table);
        }
        #endregion

        public void StepCops()
        {
            if (IsGameOver() || _gamePaused)
            {
                return;
            }
            
            foreach (Cop cop in _copPosition)
            {
                cop.TakeStep(_table);
            }
            OnGameAdvancedCops();
            AnyCopCathes();

        }

        public void StepPlayer(Direction direction)
        {
            if (IsGameOver() || _gamePaused)
            {
                return;
            } 

            _player.TakeStep(_table, direction);
            OnGameAdvancedPlayer();
            if (!AnyCopCathes())
            {
                if (_player.CollectBasket(_table))
                {
                    OnGameCollectBasket();
                }

                if (_player.CollectedBaskets == _basketNumber)
                {
                    OnGameOver(true);
                }
            }
        }
        #endregion

        #region Private event methods

        private void OnGameAdvancedPlayer()
        {
            GameAdvancedPlayer?.Invoke(this, new MaciLaciEventArgs(false, _table.FieldValues, _player.CollectedBaskets, _gameTime));
        }

        private void OnGameAdvancedCops()
        {
            GameAdvancedCops?.Invoke(this, new MaciLaciEventArgs(false, _table.FieldValues, _player.CollectedBaskets, _gameTime));
        }

        private void OnGameOver(Boolean isWon)
        {
            GameOver?.Invoke(this, new MaciLaciEventArgs(isWon, _table.FieldValues, _player.CollectedBaskets, _gameTime)); 
        }

        private void OnGameCollectBasket()
        {
            PlayerCollect?.Invoke(this, new MaciLaciEventArgs(false, _table.FieldValues, _player.CollectedBaskets, _gameTime));
        }

        private void OnGameTimeAdvanced()
        {
            GameAdvancedTime?.Invoke(this, new MaciLaciEventArgs(false, _table.FieldValues, _player.CollectedBaskets, _gameTime));
        }
        #endregion

    }

}
