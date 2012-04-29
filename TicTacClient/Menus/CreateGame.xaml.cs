using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace TicTacClient.Menus
{
    /// <summary>
    /// Interaction logic for CreateGame.xaml
    /// </summary>
    public partial class CreateGame : Page, IMenuPages
    {
        public MainWindow MainWindow { get; private set; }
        private ProtocolHandler ph;

        Action _waitingAction;
        Thread _waitingOperation;

        int gameID = -1;

        public CreateGame(MainWindow mainWindow, ProtocolHandler ph)
        {
            InitializeComponent();
            this.ph = ph;
            MainWindow = mainWindow;

            ///Waits for someone to join the game.
            ///Also animates the waiting label.
            _waitingAction = delegate ()
                {
                    try
                    {
                        int numberOfPeriods = 1;

                        while ((bool)this.Dispatcher.Invoke((Func<bool>)(() => MainWindow.ProtocolHandler.Client.Available == 0)))
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                                    waitingLabel.Content = String.Format("Waiting on opponent{0}", new string('.', numberOfPeriods))
                                ));

                            if (numberOfPeriods <= 6)
                                numberOfPeriods++;
                            else
                                numberOfPeriods = 1;

                            System.Threading.Thread.Sleep(350);
                        }

                        Player opponent = (Player)this.Dispatcher.Invoke((Func<Player>)(() => ph.GetOpponent()));
                        this.Dispatcher.Invoke((Action)(() => MainWindow.GenerateNewGame(opponent, true, gameID)));                        
                    }
                    catch (NullReferenceException) { }
                };

            _waitingOperation = new Thread(_waitingAction.Invoke);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SwapPage(MenuPages.MainMenu);
        }

        public void GenerateGameID()
        {
            if (_waitingOperation.IsAlive == false)
            {
                _waitingOperation = new Thread(_waitingAction.Invoke);
                _waitingOperation.Start();

                gameID = ph.Create();
                gameIDLabel.Content = String.Format("Your GameID is {0}.", gameID);
            }
        }
    }
}
