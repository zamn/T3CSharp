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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page, IMenuPages
    {
        public MainWindow MainWindow { get; private set; }
        private ProtocolHandler ph;

        public Login(MainWindow mainWindow, ProtocolHandler ph)
        {
            InitializeComponent();

            this.ph = ph;
            MainWindow = mainWindow;
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            connectBtn.IsEnabled = false;
            errorLabel.Content = "";

            if (usernameTextbox.Text.Length > 10)
            {
                errorLabel.Content = "Username must not be longer than 10 characters!";
            }
            else if (symbolTextbox.Text.Length != 1)
            {
                errorLabel.Content = "Please use only one character for the symbol.";
            }
            else
            {
                if (!symbolTextbox.Text.Equals("") && !usernameTextbox.Text.Equals(""))
                {
                    AttemptConnect();
                }
                else
                {
                    errorLabel.Content = "Please fill in both text boxes, then try again.";
                }
            }

            connectBtn.IsEnabled = true;
        }

        private void AttemptConnect()
        {
            /*MainWindow.ChangeUserSettings(usernameTextbox.Text, symbolTextbox.Text[0]);
            MainWindow.SwapPage(MenuPages.MainMenu);*/            
            
            if (ph.Connect("zamn.net", 6000, usernameTextbox.Text, symbolTextbox.Text[0]))
            {
                MainWindow.ChangeUserSettings(usernameTextbox.Text, symbolTextbox.Text[0]);
                MainWindow.SwapPage(MenuPages.MainMenu);
            }
            else
                errorLabel.Content = "Cannot connect to server!";            
        }
    }
}
