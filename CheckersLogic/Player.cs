using System;
using System.Linq;
using System.Collections.Generic;
using static Ex05.CheckersLogic.GameBoard;
using static Ex05.CheckersLogic.Enums;

namespace Ex05.CheckersLogic
{
    public class Player
    {
        #region Data Members 
        private const int k_KingValue = 4;
        private const bool v_HasMoreCoins = true;
        
        private String m_Name;
        private ePlayersType m_Type;
        private eStatus m_Status;
        private List<Coin> m_CoinsList;
        private int m_Score;
        private int m_TotalScore;
        private bool m_HisTurn;       
        #endregion Data Members 

        #region Constructors 
        public Player(String i_Name, ePlayersType i_Type, bool i_HisTurn)
        {
            this.m_Name = i_Name;
            this.m_Score = 0;
            this.m_TotalScore = 0;
            this.m_Type = i_Type;
            this.m_Status = eStatus.None;
            this.m_CoinsList = new List<Coin>();
            this.m_HisTurn = i_HisTurn;
        }
        #endregion Constructors

        #region Properties
        public String Name
        {
            get { return this.m_Name; }
        }

        public List<Coin> CoinsList
        {
            get { return this.m_CoinsList; }
            set { this.m_CoinsList = value; }
        }

        public int Score
        {
            get { return this.m_Score; }
            set { this.m_Score = value; }
        }

        public int TotalScore
        {
            get { return this.m_TotalScore; }
            set { this.m_TotalScore = value; }
        }
        public eStatus Status  
        {
            get { return this.m_Status; }
            set { this.m_Status = value; }           
        }

        public bool HisTurn
        {
            get { return this.m_HisTurn; }
            set { this.m_HisTurn = value; }
 
        }
        #endregion Properties

        #region public Methods 
        public bool IsLoose()
        {
            return !HasMoreCoins() || (HasMoreCoins() && !HaveFreeCoins());
        }

        public bool HaveFreeCoins()
        {
            bool haveFreeCoins = false;

            foreach (Coin currentCoin in CoinsList)
            {
                if (currentCoin.IsFree()) // current coin is free
                {
                    haveFreeCoins = !haveFreeCoins; // true.
                    break;
                }
            }

            return haveFreeCoins;
        }

        public int CalculateScore()
        {
            int sum = 0;

            foreach (Coin currentCoin in CoinsList)
            {
                if(currentCoin is KingCoin)
                {
                    sum += k_KingValue;
                }
                else
                {
                    sum += 1;
                }
            }

            Score = sum;
            return sum;
        }

        public Coin GetCoinByCoordinate(Coordinate i_location)
        {
            Coin desiredCoin = null;            // The coin we want to find.            
                                                // If won't be found, will remain null.
            if (HasMoreCoins())
            {
                foreach (Coin currentCoin in CoinsList)
                {
                    if (i_location.Equals(currentCoin.Coordinates))
                    {
                        desiredCoin = currentCoin;
                    }
                }
            }

            return desiredCoin;
        }

        public bool HasMoreCoins()
        {
            bool hasMoreCoins = true;

            if (!CoinsList.Any())
            {
                hasMoreCoins = !hasMoreCoins;
                Status = eStatus.Loose;
            }

            return hasMoreCoins;
        }

        public bool HasLessCoins(Player i_RivalPlayer)
        {
            return CoinsList.Count < i_RivalPlayer.CoinsList.Count;
        }

        public bool Tie(Player i_RivalPlayer)
        {
            return (!this.HaveFreeCoins() && !i_RivalPlayer.HaveFreeCoins());
        }
        #endregion public Methods
    }
}