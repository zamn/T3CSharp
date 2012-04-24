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


namespace TicTacClient
{
    public class GameBoardSpot
    {
        public Label Label { get; set; }
        public char Symbol { get; set; }

        public GameBoardSpot(Label label)
        {
            Label = label;
        }

        /// <summary>
        /// Attempts to flag this spot for a player.
        /// </summary>
        /// <param name="player">The player who wishes to move into this spot.</param>
        /// <returns>True if the move was successful, false if this spot has been moved to already.</returns>
        public bool AttemptMove(Player player)
        {
            if (this.Symbol == default(char))
            {
                Symbol = player.Symbol;
                Label.Content = Symbol;

                return true;
            }

            return false;
        }
    }
}
