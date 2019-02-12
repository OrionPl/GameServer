using System.Collections.Generic;

namespace GameServer
{
    public class Player
    {
        public string username;
        private List<string> unreadChat = new List<string>();
        public float[] position = new float[2];

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

        public void WritePosition(float x, float y)
        {
            position[0] = x;
            position[1] = y;
        }

        public float[] GetPosition()
        {
            return new float[2] {position[1], position[2]};
        }
    }
}
