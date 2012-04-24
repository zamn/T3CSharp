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
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page, IMenuPages
    {
        public MainWindow MainWindow { get; private set; }
        private ProtocolHandler ph;
        public MainMenu(MainWindow mainWindow, ProtocolHandler ph)
        {
            InitializeComponent();
            this.ph = ph;
            MainWindow = mainWindow;
        }

        private void createGameButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SwapPage(MenuPages.CreateGame);            
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            usernameAndSymbolLabel.Content = String.Format("Username: {0}\nSymbol: {1}", MainWindow.Player.Nickname, MainWindow.Player.Symbol);
        }

        private void userSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SwapPage(MenuPages.Settings);
        }

        private void browseGamesButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SwapPage(MenuPages.ServerBrowser);
        }

        private void joinGameButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SwapPage(MenuPages.JoinGame);
        }
    }
}
