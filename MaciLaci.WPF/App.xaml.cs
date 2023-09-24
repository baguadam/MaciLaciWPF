using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ConsoleMaciLaci.Persistence;
using MaciLaci.Model;
using System.Windows.Threading;
using MaciLaci.WPF;
using MaciLaci.WPF.ViewModel;
using Microsoft.Win32;

namespace MaciLaci.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MaciLaciModel _model = null!;
        private MaciLaciViewModel _viewModel = null!;
        private MainWindow _view = null!;
        private DispatcherTimer _timer = null!;


        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object? sender, StartupEventArgs e)
        {
            _model = new MaciLaciModel(new MaciLaciDataAccess());
            _model.GameOver += new EventHandler<MaciLaciEventArgs>(Model_GameOver);
            _model.NewGame();

            _viewModel = new MaciLaciViewModel(_model);
            _viewModel.NewGame += new EventHandler(ViewModel_NewGame);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Show();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            _model.StepCops();
            _model.AdvanceTime();
            _viewModel.UpdateTable();
        }

        private void ViewModel_NewGame(object? sender, EventArgs e)
        {
            _model.NewGame();
            _viewModel.StartNewGame();
            _timer.Start();
        }

        private void ViewModel_ExitGame(object? sender, System.EventArgs e)
        {
            _view.Close();
        }

        private void ViewModel_LoadGame(object? sender, System.EventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;
            _timer.Stop();

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Tábla betöltése";
                openFileDialog.Filter = "sav files (*.sav)|*.sav";

                if (openFileDialog.ShowDialog() == true)
                {

                    _model.LoadGame(openFileDialog.FileName);
                    _timer.Start();

                }

            }
            catch (MaciLaciDataException)
            {
                MessageBox.Show("Fájl betöltése sikertelen!");
            }
            if (restartTimer)
            {
                _timer.Start();
            }

            _viewModel.StartNewGame();
        }

        private void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;
            _timer.Stop();

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Tábla mentése!";
                saveFileDialog.Filter = "sav files (*.sav)|*.sav";

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        _model.SaveGame(saveFileDialog.FileName);
                    }
                    catch (MaciLaciDataException)
                    {
                        MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }
            catch
            {
                MessageBox.Show("A fájl mentése sikertelen!");
            }
            if (restartTimer)
            {
                _timer.Start();
            }
        }

        private void Model_GameOver(object? sender, MaciLaciEventArgs e)
        {
            _viewModel.UpdateTable();
            _timer.Stop();

            if (e.IsWon)
            {
                MessageBox.Show("Gratulálok, győztél!" + Environment.NewLine +
                "Összesen " + e.Collected + " kosarat gyűjtöttél és " +
                $"{e.GameTime}" + " másodpercig játszottál.",
                "Maci Laci játék",
                MessageBoxButton.OK,
                MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("A játék vereséggel zárult, a rendőrök elkaptak!" + Environment.NewLine +
                "Összesen " + e.Collected + " kosarat sikerült begyűjtened...",
                "Maci Laci játék",
                MessageBoxButton.OK,
                MessageBoxImage.Asterisk);
            }
        }
    }
}
