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
            serverGrid.ItemsSource = gl;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.SwapPage(MenuPages.MainMenu);
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {

        }        
    }
}