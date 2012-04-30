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
using System.Net.Sockets;

namespace TicTacClient.Menus
{
    /// <summary>
    /// Decision enum for clarity.
    /// </summary>
    public enum Decision { NO = 0, YES = 1, INVALID = -1 };

    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : Page, IMenuPages
    {
        public MainWindow MainWindow { get; private set; }
        public SocketAsyncEventArgs SocketArgs { get; private set; }

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
        /// The Game ID for the game the 2 players are playing in.
        /// </summary>
        public int GameID { get; set; }

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
        bool initialTurn;

        /// <summary>
        /// Creates a new GameBoard page.
        /// </summary>
        /// <param name="ph">The ProtocolHandler used to stage connections.</param>
        /// <param name="thisPlayer">The player using this client.</param>
        /// <param name="opponent">The opponent of the player using this client.</param>
        public GameBoard(MainWindow mainWindow, ProtocolHandler ph, Player thisPlayer, Player opponent, bool thisPlayersTurn, int GameID)
        {
            InitializeComponent();

            this.GameID = GameID;
            MainWindow = mainWindow;
            this.ph = ph;

            SocketArgs = ph.GetMoveArgs();
            SocketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveOpponentsMove);

            this.thisPlayersTurn = thisPlayersTurn;
            initialTurn = this.thisPlayersTurn;

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

            if (!thisPlayersTurn)
            {
                playerLabel.FontWeight = FontWeights.Normal;
                opponentLabel.FontWeight = FontWeights.Bold;
                statusLabel.Content = "Waiting on opponent...";

                ph.GetMove(SocketArgs);
            }
        }

        private void SelectSpot(object sender, MouseButtonEventArgs e)
        {
            if (!gameOver && thisPlayersTurn)
            {
                Rectangle selectedSpot = (Rectangle)sender;
                int spotNumber = int.Parse(selectedSpot.Name.Substring(4));

                UpdateGameboard(spotNumber);
            }
        }

        private void ReceiveOpponentsMove(object o, SocketAsyncEventArgs args)
        {
            int spot = (int)args.UserToken;
            this.Dispatcher.Invoke((Action<int>)UpdateGameboard, spot);
        }

        private void UpdateGameboard(int spot)
        {
            //Server return -1 when an opponent has quit.
            if (spot == -1)
            {
                resultLabel.Visibility = Visibility.Visible;
                playAgainButton.Visibility = System.Windows.Visibility.Visible;
                quitButton.Visibility = System.Windows.Visibility.Visible;

                resultLabel.Content = "Opponent Quit!";                
            }
            else
            {
                if (Board[spot - 1].AttemptMove(CurrentPlayer))
                {
                    //Checks to see if the current player has won.
                    if (CheckForWin() == false)
                    {
                        //If not, change turns.
                        ChangeTurn(spot);
                    }
                    else
                    {
                        //If they've won, end the game.
                        if (thisPlayersTurn)
                            ph.SendMove(spot);
                        EndGame();
                    }
                }
            }
        }

        private void ChangeTurn(int spot)
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
                ph.SendMove(spot);

                playerLabel.FontWeight = FontWeights.Normal;
                opponentLabel.FontWeight = FontWeights.Bold;
                statusLabel.Content = "Waiting on opponent...";

                ph.GetMove(SocketArgs);
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
            
            if (thisPlayersTurn)
            {
                resultLabel.Content = "You Win!";
                Player.AddWin();                
            }
            else
            {
                resultLabel.Content = "You Lose!";                
                Opponent.AddWin();
            }

            scoreLabel.Content = String.Format("{0} - {1}", Player.Wins, Opponent.Wins);

            gameOver = true;
        }

        private void forfeitButton_Click(object sender, RoutedEventArgs e)
        {
            ph.LeaveGame(); // Leaves current game for player
            MainWindow.SwapPage(MenuPages.MainMenu);
        }

        private void playAgainButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Board.Count; i++)
            {
                Board[i].Reset();
            }
            Decision theirDecision = ph.SendReplay(Decision.YES);
            if (theirDecision == Decision.YES)
            {
                resultLabel.Visibility = Visibility.Hidden;
                playAgainButton.Visibility = System.Windows.Visibility.Hidden;
                quitButton.Visibility = System.Windows.Visibility.Hidden;
                gameOver = false;
                thisPlayersTurn = initialTurn;

                if (!thisPlayersTurn)
                {
                    playerLabel.FontWeight = FontWeights.Normal;
                    opponentLabel.FontWeight = FontWeights.Bold;
                    statusLabel.Content = "Waiting on opponent...";

                    ph.GetMove(SocketArgs);
                }
                else
                {
                    playerLabel.FontWeight = FontWeights.Bold;
                    opponentLabel.FontWeight = FontWeights.Normal;
                    statusLabel.Content = "Your move...";
                }
            }
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            //ph.LeaveGame();
            ph.SendReplay(Decision.NO);
            MainWindow.SwapPage(MenuPages.MainMenu);
        }
    }
}
