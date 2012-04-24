using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows;

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
        /// Attempts to join the game with the specified GameID.
        /// </summary>
        /// <param name="GameID">The GameID of the game to join.</param>
        /// <returns>
        /// -6: Not connected
        /// 1: Invalid GameID
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

                if ((receiveInfo[0] & 15) == 7)
                    return (receiveInfo[0] >> 4) * -1;

                return receiveInfo[1];
            }

            return -6;
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

        public bool SendMove(int spot)
        {
            return false;
        }

        public int GetMove()
        {
            return 0;
        }

        // Returns an ArrayList populated of all the games on the server
        // Returns null if not connected to the server!
        public ArrayList ListGames(int filter)
        {
            if (connected)
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
                    else if (count < nickLen-1)
                    {
                        nick += Convert.ToChar(receiveInfo[i]);
                        count++;
                    }
                    else if (count == nickLen-1)
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
                return gl;
            }
            return null;
        }

        public bool SetNick(string nick)
        {
            return false;
        }

        public bool SetSymbol(char symbol)
        {
            return false;
        }

        public bool LeaveGame(int GameID)
        {
            return false;
        }


    }
}
