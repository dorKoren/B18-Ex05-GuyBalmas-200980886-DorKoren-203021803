using System;
using System.Collections.Generic;
using System.Linq;
using static Ex05.CheckersLogic.GameBoard;
using static Ex05.CheckersLogic.Enums;
using static Ex05.CheckersLogic.Directions;

namespace Ex05.CheckersLogic
{
    public class Coin
    {
        #region Regular Members

        public event EventHandler ChangedStatus;


        private GameBoard m_Board;
        private Char m_Sign;                   // 'X' , 'O' , 'K' , 'U'
        private eCoinType m_CoinType;          // 'X' , 'O' , None
        private Coordinate m_Coordinates;
        private bool m_IsKing;
        private List<Coordinate> m_AvailableCoordinates;
        #endregion Regular Members

        #region Constructor
        public Coin(eCoinType i_CoinType, GameBoard i_Board)
        {
            this.m_Board = i_Board;
            this.m_CoinType = i_CoinType;
            this.m_Coordinates = new Coordinate();
            this.m_IsKing = false;
            this.m_AvailableCoordinates = new List<Coordinate>();
            this.m_Sign = i_CoinType.Equals(eCoinType.X) ? 'X' : 'O';


            ChangedStatus = null;  
            
        }
        #endregion Constructor

        #region Properties
        public Coordinate Coordinates
        {
            get { return this.m_Coordinates; }
            set { this.m_Coordinates = value; }
        }

        public GameBoard Board
        {
            get { return this.m_Board; }
            set { this.m_Board = value; }
        }
        public char Sign
        {
            get { return this.m_Sign; }
            set { this.m_Sign = value; }
        }

        public eCoinType CoinType
        {
            get { return this.m_CoinType; }
        }

        public bool IsKing
        {
            get { return this.m_IsKing; }
            set { this.m_IsKing = value; }
        }

        public List<Coordinate> AvailableCoordinates
        {
            get { return this.m_AvailableCoordinates; }
            set { this.m_AvailableCoordinates = value; }
        }
        #endregion Properties


        #region public methods
        public void ChangeMyStatus()  
        {
            if (ChangedStatus != null)
            {
                ChangedStatus(this, new EventArgs());
            }
        }

        public KingCoin MakeKing()
        {
            KingCoin newKing = new KingCoin(CoinType, Board);   
            newKing.Coordinates.CopyCoordinates(Coordinates);
            return newKing;
        }

        public bool IsFree()
        {
            bool isFree = true;

            return AvailableCoordinates.Any() ? isFree : !isFree;
        }
        
        public virtual bool HasEatingMoves()
        {
            bool hasEatingMoves = false;

            foreach (Coordinate currentCoordinate in AvailableCoordinates)
            {
                if (IsEatingMove(currentCoordinate, out Coordinate rival))
                {
                    hasEatingMoves = !hasEatingMoves; // true
                }
            }
            return hasEatingMoves;
        }

        /*********************************************** Virtual Methods ************************************************/
        /// <summary>
        /// Returns true iff it is possible to go to the i_Target coordinate AND eat a rival coin during.
        /// </summary>
        /// <param name="i_Target"></param>
        /// <returns></returns>
        public virtual bool IsEatingMove(Coordinate i_Target, out Coordinate o_RivalCoord)
        {
            bool isEatingMove = false;
            eVerticalDirections vertical = movesForwardOrBackward(this, i_Target);
            eHorizontalDirections horizontal = movesRightOrLeft(this, i_Target);
            o_RivalCoord = null;
            Coin copy = new Coin(CoinType, Board);
            copy.Coordinates.CopyCoordinates(Coordinates);

            if (vertical == eVerticalDirections.Forword && horizontal != eHorizontalDirections.SamePlace)
            {
                Coordinate nextSquare = GetNextSquare(ref copy, vertical, horizontal);
                if (IsAbleToEat(nextSquare, out Coordinate target))
                {
                    isEatingMove = !isEatingMove;
                    isEatingMove = (isEatingMove && i_Target.Equals(target));
                    o_RivalCoord = new Coordinate();
                    o_RivalCoord.CopyCoordinates(nextSquare);
                }
            }
            return isEatingMove;
        }

        public virtual bool IsAbleToEat(Coordinate i_Rival, out Coordinate o_Target)
        {
            bool isEatingPossibility = false;
            eVerticalDirections forwordOrBackword = movesForwardOrBackward(this, i_Rival);
            eHorizontalDirections rightOrLeft = movesRightOrLeft(this, i_Rival);

            Coordinate currentSquare = new Coordinate();
            currentSquare.CopyCoordinates(Coordinates);
            o_Target = new Coordinate();

            Coin copy = new Coin(CoinType, Board);
            copy.Coordinates.CopyCoordinates(Coordinates);
            currentSquare = GetNextSquare(ref copy, forwordOrBackword, rightOrLeft);

            // Eating possibility detected
            if (i_Rival.Equals(currentSquare) 
                && Board.GetCoinType(currentSquare) != CoinType && Board.GetCoinType(currentSquare) != eCoinType.None)
            {
                copy.Coordinates.CopyCoordinates(currentSquare);
                currentSquare = GetNextSquare(ref copy, forwordOrBackword, rightOrLeft);

                if (Board.IsEmptyValidSquare(currentSquare))
                {
                    isEatingPossibility = !isEatingPossibility; // true
                    o_Target.CopyCoordinates(currentSquare);
                }
            }
            return isEatingPossibility;
        }

        public virtual bool isValidMove(Coordinate i_Target)
        {
            bool isValid = false;

            if (i_Target != null)
            {
                eVerticalDirections vertical = movesForwardOrBackward(this, i_Target);
                eHorizontalDirections horizontal = movesRightOrLeft(this, i_Target);
                Coin copy = new Coin(CoinType, Board);
                copy.Coordinates.CopyCoordinates(Coordinates);

                Coordinate newSquare = GetNextSquare(ref copy, vertical, horizontal);
                if (newSquare.Equals(i_Target))
                {
                    isValid = !isValid; // true
                }

                else if (IsAbleToEat(newSquare, out Coordinate target))
                {
                    isValid = !isValid; // true
                }
            }

            return isValid;
        }

        public virtual void UpdateMoves()
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<Coordinate> newMoves = new List<Coordinate>();
            Coin copy = new Coin(CoinType, Board);
            copy.Coordinates.CopyCoordinates(Coordinates);

            Coordinate forwordRight = GetNextSquare(ref copy, eVerticalDirections.Forword, eHorizontalDirections.Right);
            copy.Coordinates.CopyCoordinates(Coordinates);
            Coordinate forwordLeft = GetNextSquare(ref copy, eVerticalDirections.Forword, eHorizontalDirections.Left);
            coords.Add(forwordRight);
            coords.Add(forwordLeft);

            foreach (Coordinate currentCoord in coords)
            {
                if (currentCoord != null)
                {
                    if (Board.IsEmptyValidSquare(currentCoord))
                    {
                        newMoves.Add(currentCoord);
                    }
                    else if(IsAbleToEat(currentCoord, out Coordinate target))
                    {
                        newMoves.Add(target);
                    }
                }
            }
            AvailableCoordinates = newMoves;
        }
        /******************************************** End of Virtual Methods *********************************************/
        #endregion public methods
    }
}
