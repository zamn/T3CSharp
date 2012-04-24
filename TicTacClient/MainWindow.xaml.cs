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

namespace TicTacClient
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Random Rand { get; private set; }
        
        public Dictionary<MenuPages, IMenuPages> menuPages = new Dictionary<MenuPages, IMenuPages>();
        public ProtocolHandler ProtocolHandler { get; private set; }

        public Player Player { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Rand = new Random();
            ProtocolHandler = new ProtocolHandler();
            Player = new TicTacClient.Player();

            //Build and open login page.
            menuPages.Add(MenuPages.Login, new Menus.Login(this, ProtocolHandler));            
            this.Content = menuPages[MenuPages.Login];

            //Build other menu pages.
            menuPages.Add(MenuPages.CreateGame, new Menus.CreateGame(this, ProtocolHandler));
            menuPages.Add(MenuPages.JoinGame, new Menus.JoinGame(this, ProtocolHandler));
            menuPages.Add(MenuPages.MainMenu, new Menus.MainMenu(this, ProtocolHandler));
            menuPages.Add(MenuPages.ServerBrowser, new Menus.ServerBrowser(this, ProtocolHandler));
            menuPages.Add(MenuPages.Settings, new Menus.Settings(this, ProtocolHandler));            
        }

        public void ChangeUserSettings(string username, char symbol)
        {
            ProtocolHandler.SetNick(username);
            ProtocolHandler.SetSymbol(symbol);

            Player.ChangeNick(username);
            Player.ChangeSymbol(symbol);
        }

        public void GenerateNewGame(Player opponent, bool thisPlayersTurn)
        {
            menuPages.Add(MenuPages.InGame, new Menus.GameBoard(this, ProtocolHandler, Player, opponent, thisPlayersTurn));
            this.Content = menuPages[MenuPages.InGame];
        }

        public void SwapPage(MenuPages page)
        {
            this.Content = menuPages[page];
        }
    }
}