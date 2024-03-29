﻿using System;
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
            gameIDTextbox.Text = "";
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            errorLabel.Content = "";
            gameIDTextbox.Text = "";

            MainWindow.SwapPage(MenuPages.MainMenu);
        }

        public void OnLoaded(object sender, EventArgs e)
        {
            gameIDTextbox.Focus();
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            
            string gameID = gameIDTextbox.Text;
            errorLabel.Content = "";

            if (gameID.All(x => char.IsDigit(x)))
            {
                MessageBox.Show("Is this being called?1111");
                int serverResponse;

                if ((int.Parse(gameID) < 1) || (int.Parse(gameID) > 255))
                    serverResponse = 6;
                else
                    serverResponse = ph.Join(int.Parse(gameID));
                MessageBox.Show("am i gettin past diz?" + serverResponse.ToString());
                if (serverResponse == 2)
                {
                    MessageBox.Show("AM i summoning this?");
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
                        case 6:
                            errorLabel.Content = "ERROR! Invalid Game ID Range!";
                            break;
                    }
                }
            }
            else
            {
                errorLabel.Content = "Not a valid GameID.";
            }
        }

        private void gameIDTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            Key k = e.Key;
            if (k.ToString().Equals("Return"))
                connectButton_Click(this, e);
        }
    }
}
