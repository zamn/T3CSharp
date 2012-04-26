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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page, IMenuPages
    {
        public MainWindow MainWindow { get; private set; }
        private ProtocolHandler ph;
        public Settings(MainWindow mainWindow, ProtocolHandler ph)
        {
            InitializeComponent();
            this.ph = ph;
            MainWindow = mainWindow;
        }

        private void LoadSettingsWindow(object sender, RoutedEventArgs e)
        {
            usernameTextbox.Text = MainWindow.Player.Nickname;
            symbolTextbox.Text = MainWindow.Player.Symbol.ToString();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextbox.Text.Trim();
            if (username.Contains(" "))
            {
                username = username.Substring(0, username.IndexOf(" "));
            }
            if (!username.Equals("") && !symbolTextbox.Text.Equals(""))
            {
                MainWindow.ChangeUserSettings(username, symbolTextbox.Text[0]);
                ph.SetNick(username);
                ph.SetSymbol(symbolTextbox.Text[0]);
                MainWindow.SwapPage(MenuPages.MainMenu);
            }
            else
            {
                errorLabel.Content = "ERROR! Please fill in both text fields and try again.";
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            errorLabel.Content = "";
            MainWindow.SwapPage(MenuPages.MainMenu);
        }
    }
}
