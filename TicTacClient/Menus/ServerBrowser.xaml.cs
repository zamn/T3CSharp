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
using System.Collections;

namespace TicTacClient.Menus
{
    /// <summary>
    /// Interaction logic for ServerBrowser.xaml
    /// </summary>
    public partial class ServerBrowser : Page, IMenuPages
    {
        public MainWindow MainWindow { get; private set; }
        private ProtocolHandler ph;

        public ServerBrowser(MainWindow mainWindow, ProtocolHandler ph)
        {
            InitializeComponent();
            this.ph = ph;
            MainWindow = mainWindow;            
        }

        private void LoadServerBrowser(object sender, RoutedEventArgs e)
        {
            ArrayList gl = ph.ListGames(0);
            if (gl != null)
                serverGrid.ItemsSource = gl;
            else
            {
                gl = new ArrayList();
                serverGrid.ItemsSource = gl;
            }
            
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SwapPage(MenuPages.MainMenu);
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            ArrayList gl = new ArrayList();
            if (fullRadio.IsChecked == true)
                gl = ph.ListGames(2);
            else if (singleRadio.IsChecked == true)
                gl = ph.ListGames(1);
            else if (anyRadio.IsChecked == true)
                gl = ph.ListGames(0);
            serverGrid.ItemsSource = gl;
        }

        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {
            IList<DataGridCellInfo> items = serverGrid.SelectedCells;

            int gameID = (items[1].Item as Game).GameID;

            int serverResponse = ph.Join(gameID);
            if (serverResponse == 2)
            {
                Player opponent = ph.GetOpponent();
                MainWindow.GenerateNewGame(opponent, false, gameID);
            }
            else
            {
                switch (serverResponse)
                {
                    case -1:
                        MessageBox.Show("FATAL ERROR! You are not connected to the server. Please close this application.");
                        break;
                    case 1:
                        MessageBox.Show("ERROR! There is no game with this GameID.");
                        break;
                    case 3:
                        MessageBox.Show("ERROR! This game's owner and you share the same symbol!");
                        break;
                    case 4:
                        MessageBox.Show("ERROR! This game's owner and you share the same nickname!");
                        break;
                    case 5:
                        MessageBox.Show("ERROR! This game is full!");
                        break;
                }
            }
        }        
    }
}
