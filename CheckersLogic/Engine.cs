using System;
using static Ex05.CheckersLogic.Enums;
using static Ex05.CheckersLogic.GameBoard;

namespace Ex05.CheckersLogic
{
    
    public class EndOfGameEventArgs : EventArgs
    {
        // $G$ CSS-002 (-5) Bad member variable name (should be m_CamelCased)
        public bool m_GameOver;
    } 

    public delegate void EndOfGameHandler(object sender, EndOfGameEventArgs e);
    public class Engine
    {
        #region Class members
        public event EndOfGameHandler EndOfGame;
        public event EventHandler TurnMovedToNextPlayer;
        public event EventHandler PlayerHasScored;

        private Checkers m_Checkers;
        private Coordinate m_SourceCoordinate;
        private Coordinate m_TargetCoordinate;
        private Coordinate m_LastCoordinate; 
        Player m_CurrentPlayer;
        Player m_WaitingPlayer;
        #endregion Class members

        #region Constructor
        public Engine(GameSettings i_GameSettings)
        {      
            this.m_Checkers =  initialCheckersSetting(i_GameSettings);
            this.m_SourceCoordinate = null;
            this.m_TargetCoordinate = null;
            this.m_LastCoordinate = null; 
        }
        #endregion Constructor

        #region Properties
        public Checkers Checkers
        {
            get { return m_Checkers; }
        }

        public Coordinate SourceCoordinate
        {
            get { return m_SourceCoordinate; }
            set { m_SourceCoordinate = value; }
        }

        public Coordinate TargetCoordinate
        {
            get { return m_TargetCoordinate; }
            set { m_TargetCoordinate = value; }
        }

        public Coordinate LastCoordinate  
        {
            get { return m_LastCoordinate; }
            set { m_LastCoordinate = value; }
        }

        public GameBoard Board
        {
            get { return m_Checkers.Board; }
        }

        public Player CurrentPlayer
        {
            get { return m_CurrentPlayer; }
            set { m_CurrentPlayer = value; }
        }

        public Player WaitingPlayer
        {
            get { return m_WaitingPlayer; }
            set { m_WaitingPlayer = value; }
        }
        #endregion Properties

        #region Methods

        public void ResetCoordinates()
        {
            SourceCoordinate = null;
            TargetCoordinate = null;
            LastCoordinate = null;
        }

        public eMoveType TakeMoveFromPlayer()
        {
            eMoveType moveType = eMoveType.None;

            if (CurrentPlayer is Computer)
            {
                moveType = computerMove(CurrentPlayer as Computer);
            }
            else
            {
                moveType = realPlayerMove();
            }

            m_Checkers.UpdatePlayersAvailableMoves();
            return moveType;
        }

        public void SwitchTurnsIfNecessary()
        {
            if (!CurrentPlayer.HisTurn)
            {
                SwitchTurns();
                
                if (TurnMovedToNextPlayer != null && !(CurrentPlayer is Computer) && !(WaitingPlayer is Computer))
                {
                    TurnMovedToNextPlayer.Invoke(this, new EventArgs());
                }
            }
        }

        public bool GameOver()
        {
            bool gameOver = false;

            if (CurrentPlayer.IsLoose() || WaitingPlayer.Tie(CurrentPlayer))
            {
                gameOver = !gameOver;  // true
                CalcFinalScore();
                EndOfGameEventArgs e = new EndOfGameEventArgs();
                EndOfGame.Invoke(this, e);

                if (!e.m_GameOver) 
                {
                    PlayerHasScored.Invoke(this, new EventArgs());
                    Checkers.Board.ClearLogicBoard();
                    ResetGame();
                    
                }
            }

            return gameOver;
        }

        private void SwitchTurns()
        {
            Player tempPlayer = CurrentPlayer;
            CurrentPlayer = WaitingPlayer;
            WaitingPlayer = tempPlayer;
        }

        public void ResetGame()
        {
            Player player1 = m_Checkers.Player1;
            Player player2 = m_Checkers.Player2;

            m_CurrentPlayer = player1;
            m_WaitingPlayer = player2;

            m_CurrentPlayer.HisTurn = true;
            m_WaitingPlayer.HisTurn = false;

            giveCoinsToPlayers();
            setCoinsOnBoard();

            m_Checkers.UpdatePlayersAvailableMoves();
        }

        private void giveCoinsToPlayers()
        {
            m_Checkers.GiveCoinsToPlayer(CurrentPlayer);
            m_Checkers.GiveCoinsToPlayer(WaitingPlayer);
        }

        private void setCoinsOnBoard()
        {
            m_Checkers.Board.SetPlayerCoinsOnBoard(CurrentPlayer);
            m_Checkers.Board.SetPlayerCoinsOnBoard(WaitingPlayer);
        }
      
        private eMoveType computerMove(Computer i_Computer)
        {
            Coin randomCoin = i_Computer.ChooseRandomCoin();            
            SourceCoordinate = randomCoin.Coordinates;
            TargetCoordinate = i_Computer.ChooseRandomCoordinate(randomCoin);


            eMoveType moveType = 
                m_Checkers.MoveCoin(randomCoin, TargetCoordinate);

            // If the source and the target coordinates are valid make move.
            if (moveType.Equals(eMoveType.Step) || moveType.Equals(eMoveType.Eat))
            {
                // If this player doesn't have an eating possibility, 
                // choose available move and then switch turns.
                if (moveType.Equals(eMoveType.Step) || 
                   (!m_Checkers.HasEatingMoves(i_Computer) && 
                     moveType.Equals(eMoveType.Eat)))                
                {                
                    m_Checkers.SwitchTurnFlags();
                }
            }
            return moveType;
        }

        private eMoveType realPlayerMove()
        {
            // Get the right coin according to the source coordinate.
            Coin coin =
                CurrentPlayer.GetCoinByCoordinate(LastCoordinate);  

            eMoveType moveType =
                m_Checkers.MoveCoin(coin, TargetCoordinate);

            // If the source and target coordinates are valid make move.
            if (moveType.Equals(eMoveType.Step) ||
                moveType.Equals(eMoveType.Eat))
            {
                // If this player doesn't have an eating possibility, 
                // choose available moove and then switch turns.
                if (moveType.Equals(eMoveType.Step) ||
                    (!m_Checkers.HasEatingMoves(m_Checkers.GetCoinOwner(coin)) &&
                      moveType.Equals(eMoveType.Eat)))
                {
                    // Switch the turn flags of the players.                                                          
                    m_Checkers.SwitchTurnFlags();
                }
            }

            return moveType;
        }

        private Checkers initialCheckersSetting(GameSettings i_GameSettings)
        {
            string player1Name = i_GameSettings.Player1Name;
            string player2Name = i_GameSettings.Player2Name;
            bool isSinglePlayer = i_GameSettings.IsSinglePlayer;
            bool hisTurn = true;
            int boardSize = i_GameSettings.BoardSize;

            Player player1 =
                new Player(player1Name, ePlayersType.RealPlayer, hisTurn);

            Player player2 = isSinglePlayer ?
                new Computer() :
                new Player(player2Name, ePlayersType.RealPlayer, !hisTurn);

            return new Checkers(player1, player2, boardSize);
        }

        public void CalcFinalScore()
        {
            m_Checkers.Player1.CalculateScore();
            m_Checkers.Player2.CalculateScore();
            m_Checkers.Player1.TotalScore += m_Checkers.Player1.Score;
            m_Checkers.Player2.TotalScore += m_Checkers.Player2.Score;
        }  
        #endregion Methods
    }
}

