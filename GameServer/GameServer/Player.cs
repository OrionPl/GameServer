using System.Collections.Generic;

namespace GameServer
{
    public class Player
    {
        public string username;
        private List<string> unreadChat = new List<string>();

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
