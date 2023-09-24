using MaciLaci.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaciLaci.Persistence;
using System.Threading;
using System.Timers;
using System.Windows;

namespace MaciLaci.WPF.ViewModel
{
    public class MaciLaciViewModel : ViewModelBase
    {
        private MaciLaciModel _model;

        #region Properties 
        public DelegateCommand NewGameCommand { get; private set; }

        public DelegateCommand LoadGameCommand { get; private set; }

        public DelegateCommand SaveGameCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand StepPlayerCommand { get; private set; }

        public DelegateCommand PauseGameCommand { get; private set; }

        public DelegateCommand ContinueGameCommand { get; private set; }
        public ObservableCollection<MaciLaciField> GameField { get; set; }


        public Int32 TableSizeInt { get; private set; }

        public Int32 GameBasketCollected { get { return _model.Player.CollectedBaskets;  } }

        public Int32 GameTime { get { return _model.GameTime; } }

        public Boolean IsTableSmall
        {
            get { return _model.TableSize == TableSize.SMALL; }
            set
            {
                if (_model.TableSize == TableSize.SMALL)
                {
                    return;
                }

                _model.TableSize = TableSize.SMALL;
                OnPropertyChanged(nameof(IsTableSmall));
                OnPropertyChanged(nameof(IsTableMedium));
                OnPropertyChanged(nameof(IsTableLarge));
            }
        }

        public Boolean IsTableMedium
        {
            get { return _model.TableSize == TableSize.MEDIUM; }
            set
            {
                if (_model.TableSize == TableSize.MEDIUM)
                {
                    return;
                }

                _model.TableSize = TableSize.MEDIUM;
                OnPropertyChanged(nameof(IsTableSmall));
                OnPropertyChanged(nameof(IsTableMedium));
                OnPropertyChanged(nameof(IsTableLarge));
            }
        }

        public Boolean IsTableLarge
        {
            get { return _model.TableSize == TableSize.LARGE; }
            set
            {
                if (_model.TableSize == TableSize.LARGE)
                {
                    return;
                }

                _model.TableSize = TableSize.LARGE;
                OnPropertyChanged(nameof(IsTableSmall));
                OnPropertyChanged(nameof(IsTableMedium));
                OnPropertyChanged(nameof(IsTableLarge));
            }
        }

        #endregion

        #region Events
        public event EventHandler? NewGame;
        public event EventHandler? SaveGame;
        public event EventHandler? LoadGame;
        public event EventHandler? ExitGame;
        #endregion

        #region Constructors
        public MaciLaciViewModel(MaciLaciModel model)
        {
            _model = model;

            _model.GameAdvancedPlayer += new EventHandler<MaciLaciEventArgs>(Game_GameAdvanced);
            _model.GameAdvancedCops += new EventHandler<MaciLaciEventArgs>(Game_GameAdvanced);
            _model.PlayerCollect += new EventHandler<MaciLaciEventArgs>(Game_PlayerCollect);

            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());

            StepPlayerCommand = new DelegateCommand(param => StepPlayer(param?.ToString() ?? String.Empty));
            PauseGameCommand = new DelegateCommand(param => PauseGame());
            ContinueGameCommand = new DelegateCommand(param => ContinueGame());

            StartNewGame();
        }

        #endregion

        #region Public methods
        public void StartNewGame()
        {
            GameField = new ObservableCollection<MaciLaciField>();
            for (Int32 i = 0; i < _model.GameTable.Size; i++)
            {
                for (Int32 j = 0; j < _model.GameTable.Size; j++)
                {
                    GameField.Add(new MaciLaciField
                    {
                        FieldOptions = FieldOptions.FREE,
                        X = i,
                        Y = j,
                        Text = String.Empty,
                        Number = i * _model.GameTable.Size + j,
                    });
                }
            }

            switch(_model.TableSize)
            {
                case TableSize.SMALL:
                    TableSizeInt = 6;
                    break;
                case TableSize.MEDIUM:
                    TableSizeInt = 8;
                    break;
                case TableSize.LARGE:
                    TableSizeInt = 9;
                    break;
            }
            OnPropertyChanged(nameof(TableSizeInt));
            OnPropertyChanged(nameof(GameField));
            OnPropertyChanged(nameof(GameTime));
            UpdateTable();
        }

        public void UpdateTable()
        {
            foreach (var field in GameField)
            {
                field.FieldOptions = _model.GameTable.GetValue(field.X, field.Y);
                switch(field.FieldOptions)
                {
                    case FieldOptions.PLAYER:
                        field.Text = "🐻";
                        break;
                    case FieldOptions.COP:
                        field.Text = "🚔";
                        break;
                    case FieldOptions.TREE:
                        field.Text = "🌳";
                        break;
                    case FieldOptions.BASKET:
                        field.Text = "🧺";
                        break;
                    default:
                        field.Text = String.Empty;
                        break;
                }
                OnPropertyChanged(field.Text);
            }
        }
        #endregion

        #region Private methods
        private void StepPlayer(String direction)
        {
            switch (direction)
            {
                case "W":
                    _model.StepPlayer(Direction.UP);
                    break;
                case "S":
                    _model.StepPlayer(Direction.DOWN);
                    break;
                case "D":
                    _model.StepPlayer(Direction.RIGHT);
                    break;
                case "A":
                    _model.StepPlayer(Direction.LEFT);
                    break;
            }

            UpdateTable();
            OnPropertyChanged();
        }

        private void PauseGame()
        {
            _model.PauseGame();
        }

        private void ContinueGame()
        {
            _model.ContinueGame();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _model.StepCops();
            OnPropertyChanged(nameof(GameTime));
        }

        private void Model_CreateGame(object? sender, MaciLaciEventArgs e)
        {
            UpdateTable();
        }

        private void Game_GameAdvanced(object? sender, MaciLaciEventArgs e)
        {
            foreach (var field in GameField)
            {
                field.FieldOptions = _model.GameTable.GetValue(field.X, field.Y);
            }
        }

        private void Game_PlayerCollect(object? sender, MaciLaciEventArgs e)
        {
            OnPropertyChanged(nameof(GameBasketCollected));
        }

        private void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
