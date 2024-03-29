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
    public class Player
    {        
        /// <summary>
        /// The player's nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// The player's symbol character.
        /// </summary>
        public char Symbol { get; set; }

        /// <summary>
        /// The number of times this player has won during the current game.
        /// </summary>
        public int Wins { get; private set; }        
     
        /// <summary>
        /// Gives this player one more win.
        /// </summary>
        public void AddWin() 
        { 
            Wins++; 
        }

        /// <summary>
        /// Changes this player's nickname.
        /// </summary>
        /// <param name="nick"></param>
        public void ChangeNick(string nick) 
        { 
            Nickname = nick; 
        }

        /// <summary>
        /// Changes this player's symbol.
        /// </summary>
        /// <param name="symbol"></param>
        public void ChangeSymbol(char symbol) 
        { 
            Symbol = symbol; 
        }

        /// <summary>
        /// Resets this player's win/loss/tie record.
        /// </summary>
        public void ResetRecord()
        {
            Wins = 0;
        }        
    }
}
