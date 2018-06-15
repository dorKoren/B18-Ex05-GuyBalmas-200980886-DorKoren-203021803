
namespace Ex05.CheckersLogic
{
    public struct GameSettings
    {
        private int m_BoardSize;
        private string m_Player1Name;
        private string m_Player2Name;
        private bool m_IsSinglePlayer;

        public int BoardSize
        {
            get { return m_BoardSize; }
            set { m_BoardSize = value; }
        }

        public string Player1Name
        {
            get { return m_Player1Name; }
            set { m_Player1Name = value; }
        }

        public string Player2Name
        {
            get { return m_Player2Name; }
            set { m_Player2Name = value; }
        }

        public bool IsSinglePlayer
        {
            get { return m_IsSinglePlayer; }
            set { m_IsSinglePlayer = value; }
        }
    }
}
