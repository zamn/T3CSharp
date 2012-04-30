using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using TicTacClient.Menus;

namespace TicTacClient
{
    public class ProtocolHandler
    {
        private Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private bool connected = false;

        public Socket Client { get { return sock; } }

        /// <summary>
        /// Connects to the server with nickname/symbol
        /// </summary>
        /// <param name="server">The address of the server to connect to.</param>
        /// <param name="port">The port number to use for the connection.</param>
        /// <param name="nick">The user's nickname.</param>
        /// <param name="symbol">The user's symbol.</param>
        /// <returns>False if server is down, otherwise true.</returns>
        public bool Connect(string server, int port, string nick, char symbol)
        {
            try
            {
                IPHostEntry heserver = Dns.GetHostEntry(server);
                IPAddress ip = heserver.AddressList[0];
                sock.Connect(ip, port);

                byte[] info = new byte[20];
                byte[] receiveInfo = new byte[20];
                int nickLen = nick.Length << 4;
                info[0] = Convert.ToByte(nickLen | 1);
                info[1] = Convert.ToByte(symbol);

                for (int i = 0; i < nick.Length; i++)
                {
                    info[i + 2] = Convert.ToByte(nick[i]);
                }

                sock.Send(info);
                sock.Receive(receiveInfo);

                if (Convert.ToInt32(receiveInfo[0]) == 2)
                {
                    connected = true;
                    return true;
                }
            }
            catch (SocketException)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Creates a new game on the server.
        /// </summary>
        /// <returns>
        /// The Game ID as an int, OR 
        /// 0 if there are no more spaces left on the server for new games, OR 
        /// -1 if you are not connected.
        /// -2 if there are no free games open.
        /// </returns>
        public int Create()
        {
            if (connected)
            {
                byte[] info = new byte[1];
                info[0] = Convert.ToByte(3);
                sock.Send(info);

                byte[] receiveInfo = new byte[20];                
                sock.Receive(receiveInfo);    

                if (receiveInfo[0] == 2)
                    return Convert.ToInt32(receiveInfo[1]);
                else if (((receiveInfo[0] & 15) == 7) && ((receiveInfo[0] >> 4) == 7))
                    return -2;
            }
            return -1;
        }

        /// <summary>
        /// Attempts to join the game with the specified Game ID.
        /// </summary>
        /// <param name="GameID">The Game ID of the game to join.</param>
        /// <returns>
        /// -1: Not connected
        /// 1: Invalid Game ID.
        /// 3: Opponent has same symbol
        /// 4: Opponent has same nick
        /// 5: Game is full
        /// </returns>
        public int Join(int GameID)
        {
            if (connected)
            {
                byte[] info = new byte[2];
                info[0] = Convert.ToByte(4);
                info[1] = Convert.ToByte(GameID);
                sock.Send(info);

                byte[] receiveInfo = new byte[20];
                sock.Receive(receiveInfo);
                //MessageBox.Show("Getting: " + receiveInfo[0].ToString());
                if ((receiveInfo[0] & 15) == 7)
                    return (receiveInfo[0] >> 4);

                return receiveInfo[0];
            }

            return -1;
        }

        /// <summary>
        /// Use after a successful Join() call to get the opponent's information.
        /// </summary>
        /// <returns></returns>
        public Player GetOpponent()
        {
            Player opponent;
            byte[] buffer = new byte[20];
            
            sock.Receive(buffer);

            int nickLength = buffer[0] >> 4;
            char opponentSymbol = Convert.ToChar(buffer[1]);
            string opponentNick = Encoding.UTF8.GetString(buffer, 2, nickLength);

            opponent = new Player() { Nickname = opponentNick, Symbol = opponentSymbol };

            return opponent;
        }

        /// <summary>
        /// Sends the move specified to the opponent.
        /// </summary>
        /// <param name="spot">The position the player wants to move.</param>
        public void SendMove(int spot)
        {
            byte[] info = new byte[10];
            info[0] = Convert.ToByte(spot << 4);
            info[0] |= 5;
            sock.Send(info);
        }

        /// <summary>
        /// Generates EventArgs for sending moves to the opponent.
        /// </summary>
        /// <returns></returns>
        public SocketAsyncEventArgs GetMoveArgs()
        {
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            byte[] buffer = new byte[10];            
            e.SetBuffer(buffer, 0, 10);

            e.Completed += delegate(object o, SocketAsyncEventArgs se)
            {
                byte[] receiveInfo = se.Buffer;
                int move = Convert.ToInt32(receiveInfo[0]);
                if (((move & 15) == 7) && ((move >> 4) == 6))
                    se.UserToken = -1;
                else
                    se.UserToken = Convert.ToInt32(receiveInfo[0] >> 4);
            };
            
            return e;
        }

        /// <summary>
        /// Puts in a request to get the opponent's next move using the
        /// specified SockeyAsyncEventArgs.
        /// </summary>
        /// <param name="e">The arguments to use in the connection.</param>
        public void GetMove(SocketAsyncEventArgs e)
        {            
            sock.ReceiveAsync(e);
        }

        /// <summary>
        /// Gathers a list of all the active games currently on the server.
        /// </summary>
        /// <returns>
        /// Returns an ArrayList populated of all the games on the server
        /// Returns null if not connected to the server!
        /// </returns>
        public ArrayList ListGames(int filter)
        {
            if (connected)
            {
                try
                {
                    ArrayList gl = new ArrayList();
                    byte[] info = new byte[1];
                    info[0] = Convert.ToByte(6);
                    sock.Send(info);
                    byte[] receiveInfo = new byte[1024];
                    sock.Receive(receiveInfo);
                    string nick = "";
                    int byteNum = 1,
                        count = 0,
                        gameID = 0,
                        numPlayers = 0,
                        nickLen = 0,
                        i = 0;
                    while (receiveInfo[i] != 0)
                    {
                        if (byteNum == 1)
                        {
                            nickLen = receiveInfo[i] >> 4;
                            numPlayers = receiveInfo[i] & 15;

                            if (numPlayers == 1)
                                numPlayers = 2;
                            else
                                numPlayers = 1;
                        }
                        else if (byteNum == 2)
                        {
                            gameID = receiveInfo[i];
                        }
                        else if (count < nickLen - 1)
                        {
                            nick += Convert.ToChar(receiveInfo[i]);
                            count++;
                        }
                        else if (count == nickLen - 1)
                        {
                            nick += Convert.ToChar(receiveInfo[i]);
                            gl.Add(new Game(gameID, numPlayers, nick));
                            byteNum = 0;
                            count = 0;
                            nick = "";
                        }

                        byteNum++;
                        i++;
                    }
                    ArrayList newGL = new ArrayList();
                    switch (filter)
                    {
                        case 0:
                            newGL = gl;
                            break;
                        case 1:
                            for (int x = 0; x < gl.Count; x++)
                            {
                                if (((Game)gl[x]).NumPlayers == 1)
                                    newGL.Add(gl[x]);
                            }
                            break;
                        case 2:
                            for (int x = 0; x < gl.Count; x++)
                            {
                                if (((Game)gl[x]).NumPlayers == 2)
                                    newGL.Add(gl[x]);
                            }
                            break;
                    }
                    return newGL;
                }
                catch (SocketException)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the nickname on the server side.
        /// </summary>
        /// <param name="nick">The new nickname.</param>
        /// <returns>Returns true if the server successfully changed nickname.</returns>
        public bool SetNick(string nick)
        {
            byte[] info = new byte[10];
            info[0] = Convert.ToByte(nick.Length << 4);
            info[0] |= 8;
            for (int i = 1; i <= nick.Length; i++)
            {
                info[i] = Convert.ToByte(nick[i - 1]);
            }
            sock.Send(info);
            byte[] receiveInfo = new byte[20];
            sock.Receive(receiveInfo);
            if ((receiveInfo[0] & 240) == 7)
                return false;
            else if ((receiveInfo[0] & 240) == 2)
                return true;

            return false;
        }

        /// <summary>
        /// Sets the symbol on the server side.
        /// </summary>
        /// <param name="symbol">The new symbol to be used.</param>
        /// <returns></returns>
        public bool SetSymbol(char symbol)
        {
            byte[] info = new byte[10];
            info[0] = 9;
            info[1] = Convert.ToByte(symbol);
            sock.Send(info);
            byte[] receiveInfo = new byte[10];
            sock.Receive(receiveInfo);
            if ((receiveInfo[0] & 240) == 7)
                return false;
            else if ((receiveInfo[0] & 240) == 2)
                return true;

            return false;
        }

        /// <summary>
        /// Leaves the game you are currently in.
        /// </summary>
        /// <param name="GameID">The game you are currently in.</param>
        public void LeaveGame()
        {
            byte[] info = new byte[10];
            info[0] |= 10;
            sock.Send(info);
        }

        public SocketAsyncEventArgs GetReplayDecisionArgs()
        {
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            byte[] buffer = new byte[10];
            e.SetBuffer(buffer, 0, 10);

            e.Completed += delegate(object o, SocketAsyncEventArgs se)
            {
                byte[] receiveInfo = se.Buffer;

                if ((receiveInfo[0] & 15) == 11)
                {
                    if (((receiveInfo[0] & 240) >> 4) == 0)
                        se.UserToken = Decision.NO;
                    else
                        se.UserToken = Decision.YES;
                }
                else
                    se.UserToken = Decision.INVALID;
            };

            return e;
        }

        public void GetReplayDecision(SocketAsyncEventArgs e)
        {
            sock.ReceiveAsync(e);
        }

        /// <summary>
        /// Inform other player whether or not you desire a rematch.
        /// </summary>
        /// <param name="decision">Players decision on Play Again?</param>
        /// <returns></returns>
        public void SendReplayDecision(Decision decision)
        {
            byte[] info = new byte[10];
            info[0] = Convert.ToByte((int)decision << 4);
            info[0] |= 11;

            sock.Send(info);            
        }                
    }
}
