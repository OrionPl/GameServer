using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class Player
    {
        public string username;
        private List<string> unreadChat;

        public Player(string Username)
        {
            username = Username;
        }

        public string[] ReadChat()
        {
            string[] tempChat = unreadChat.ToArray();
            unreadChat.Clear();
            return tempChat;
        }

        public void AddStringToChat(string s)
        {
            unreadChat.Add(s);
        }
    }
}
