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
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : Page, IMenuPages
    {
        public MainWindow MainWindow { get; private set; }

        /// <summary>
        /// A list containing the 9 spots on a tic-tac-toe board.        
        /// </summary>
        public List<GameBoardSpot> Board { get; private set; }

        /// <summary>
        /// The player using this client.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// The player connecting to this game from another client.
        /// </summary>
        public Player Opponent { get; private set; }

        /// <summary>
        /// The player whose turn it currently is.
        /// </summary>
        public Player CurrentPlayer
        {
            get
            {
                if (thisPlayersTurn) return Player;
                else return Opponent;
            }
        }

        ProtocolHandler ph;

        bool gameOver = false;
        bool thisPlayersTurn;        

        /// <summary>
        /// Creates a new GameBoard page.
        /// </summary>
        /// <param name="ph">The ProtocolHandler used to stage connections.</param>
        /// <param name="thisPlayer">The player using this client.</param>
        /// <param name="opponent">The opponent of the player using this client.</param>
        public GameBoard(MainWindow mainWindow, ProtocolHandler ph, Player thisPlayer, Player opponent, bool thisPlayersTurn)
        {
            InitializeComponent();

            MainWindow = mainWindow;
            this.ph = ph;
            this.thisPlayersTurn = thisPlayersTurn;

            if (!thisPlayersTurn)
            {
                playerLabel.FontWeight = FontWeights.Normal;
                opponentLabel.FontWeight = FontWeights.Bold;
                statusLabel.Content = "Waiting on opponent..."; 
            }


            Board = new List<GameBoardSpot>()
            {
                new GameBoardSpot(spot1Label), 
                new GameBoardSpot(spot2Label), 
                new GameBoardSpot(spot3Label), 
                new GameBoardSpot(spot4Label), 
                new GameBoardSpot(spot5Label), 
                new GameBoardSpot(spot6Label), 
                new GameBoardSpot(spot7Label), 
                new GameBoardSpot(spot8Label), 
                new GameBoardSpot(spot9Label)
            };

            Player = thisPlayer;
            Opponent = opponent;

            playerLabel.Content = String.Format("{0}({1})", Player.Nickname, Player.Symbol);
            opponentLabel.Content = String.Format("{0}({1})", Opponent.Nickname, Opponent.Symbol);            
        }

        private void SelectSpot(object sender, MouseButtonEventArgs e)
        {
            if (!gameOver && thisPlayersTurn)
            {
                Rectangle selectedSpot = (Rectangle)sender;
                int spotNumber = int.Parse(selectedSpot.Name.Substring(4));

                //Will return false if this spot is filled already.
                if (Board[spotNumber - 1].AttemptMove(CurrentPlayer))
                {
                    //Checks to see if the current player has won.
                    if (CheckForWin() == false)
                    {
                        //If not, change turns.
                        ChangeTurn();
                    }
                    else
                    {
                        //If they've won, end the game.
                        EndGame();
                    }
                }
            }
        }

        private void ChangeTurn()
        {
            thisPlayersTurn = !thisPlayersTurn;

            if (thisPlayersTurn)
            {
                playerLabel.FontWeight = FontWeights.Bold;
                opponentLabel.FontWeight = FontWeights.Normal;
                statusLabel.Content = "Your move...";
            }
            else
            {
                playerLabel.FontWeight = FontWeights.Normal;
                opponentLabel.FontWeight = FontWeights.Bold;
                statusLabel.Content = "Waiting on opponent...";                
            }            
        }

        private bool CheckForWin()
        {
            char symbol = CurrentPlayer.Symbol;           

            if ((Board[0].Symbol == symbol && Board[1].Symbol == symbol && Board[2].Symbol == symbol) ||
                (Board[3].Symbol == symbol && Board[4].Symbol == symbol && Board[5].Symbol == symbol) ||
                (Board[6].Symbol == symbol && Board[7].Symbol == symbol && Board[8].Symbol == symbol) ||
                (Board[0].Symbol == symbol && Board[3].Symbol == symbol && Board[6].Symbol == symbol) ||
                (Board[1].Symbol == symbol && Board[4].Symbol == symbol && Board[7].Symbol == symbol) ||
                (Board[2].Symbol == symbol && Board[5].Symbol == symbol && Board[8].Symbol == symbol) ||
                (Board[0].Symbol == symbol && Board[4].Symbol == symbol && Board[8].Symbol == symbol) ||
                (Board[2].Symbol == symbol && Board[4].Symbol == symbol && Board[6].Symbol == symbol))
            {
                return true;
            }
            
            return false;            
        }

        private void EndGame()
        {
            resultLabel.Visibility = Visibility.Visible;
            playAgainButton.Visibility = System.Windows.Visibility.Visible;
            quitButton.Visibility = System.Windows.Visibility.Visible;
            resultLabel.Content = "You Win!";

            gameOver = true;
        }

        private void forfeitButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
