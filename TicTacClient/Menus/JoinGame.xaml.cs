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

namespace TicTacClient.Menus
{
    /// <summary>
    /// Interaction logic for JoinGame.xaml
    /// </summary>
    public partial class JoinGame : Page, IMenuPages
    {
        public MainWindow MainWindow { get; private set; }
        private ProtocolHandler ph;
        public JoinGame(MainWindow mainWindow, ProtocolHandler ph)
        {
            InitializeComponent();
            this.ph = ph;
            MainWindow = mainWindow;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            errorLabel.Content = "";
            gameIDTextbox.Text = "";

            MainWindow.SwapPage(MenuPages.MainMenu);
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            string gameID = gameIDTextbox.Text;
            errorLabel.Content = "";

            if (gameID.All(x => char.IsDigit(x)))
            {
                int serverResponse = ph.Join(int.Parse(gameID));

                if (serverResponse == 2)
                {
                    Player opponent = ph.GetOpponent();
                    MainWindow.GenerateNewGame(opponent, false, Convert.ToInt32(gameID));
                }
                else
                {
                    switch (serverResponse)
                    {
                        case -1:
                            errorLabel.Content = "FATAL ERROR! You are not connected to the server. Please close this application.";
                            break;
                        case 1:
                            errorLabel.Content = "ERROR! There is no game with this GameID.";
                            break;
                        case 3:
                            errorLabel.Content = "ERROR! This game's owner and you share the same symbol!";
                            break;
                        case 4:
                            errorLabel.Content = "ERROR! This game's owner and you share the same nickname!";
                            break;
                        case 5:
                            errorLabel.Content = "ERROR! This game is full!";
                            break;
                    }
                }
            }
            else
            {
                errorLabel.Content = "Not a valid GameID.";
            }
        }
    }
}
