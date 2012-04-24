using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacClient
{
    class Game
    {
        public int GameID { get; set; }
        public string Player1 { get; set; }
        public int NumPlayers { get; set; }

        public Game(int gid, int np, string player)
        {
            GameID = gid;
            NumPlayers = np;
            Player1 = player;
        }
    }
}
