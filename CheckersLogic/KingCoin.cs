using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ex05.CheckersLogic.Enums;
using static Ex05.CheckersLogic.GameBoard;
using static Ex05.CheckersLogic.Directions;

namespace Ex05.CheckersLogic
{
    public class KingCoin : Coin
    {
        #region Constructor
        public KingCoin(eCoinType i_CoinType, GameBoard i_Board) : base(i_CoinType, i_Board)
        {
            IsKing = true;
            setKingSign();
        }
        #endregion Constructor

        #region Public Methods
        public override bool HasEatingMoves()
        {
            bool hasEatingMoves = false;
            foreach (Coordinate currentCoordinate in AvailableCoordinates)
            {
                eVerticalDirections vertical = movesForwardOrBackward(this, currentCoordinate);
                eHorizontalDirections horizontal = movesRightOrLeft(this, currentCoordinate);

                if (IsEatingMove(currentCoordinate, out Coordinate rival))
                {
                    hasEatingMoves = !hasEatingMoves; // true
                }
            }
            return hasEatingMoves;
        }

        public override bool IsEatingMove(Coordinate i_Target, out Coordinate o_RivalCoord)
        {
            bool isEatingMove = false;
            o_RivalCoord = new Coordinate();
            eVerticalDirections vertical = movesForwardOrBackward(this, i_Target);
            eHorizontalDirections horizontal = movesRightOrLeft(this, i_Target);

            Coordinate prevSquare = new Coordinate();
            prevSquare.CopyCoordinates(Coordinates);

            Coin kingCopy = new Coin(CoinType, Board);
            kingCopy.Coordinates.CopyCoordinates(Coordinates);
            Coordinate nextSquare = GetNextSquare(ref kingCopy, vertical, horizontal);
            kingCopy.Coordinates = prevSquare;
            
            while (Board.IsEmptyValidSquare(nextSquare))
            {
                prevSquare.CopyCoordinates(nextSquare);
                nextSquare = GetNextSquare(ref kingCopy, vertical, horizontal);
            }

            if (kingCopy.IsAbleToEat(nextSquare, out Coordinate target))
            {
                isEatingMove = !isEatingMove; // true
                isEatingMove = (isEatingMove && i_Target.Equals(target));
                if (isEatingMove)
                {
                    o_RivalCoord.CopyCoordinates(nextSquare);
                }
            }

            return isEatingMove;
        }

        public override bool IsAbleToEat(Coordinate i_Rival, out Coordinate o_Target)
        {
            o_Target = new Coordinate();

            Coordinate prevSquare = new Coordinate();
            prevSquare.CopyCoordinates(Coordinates);

            bool isEatingPossibility = false;
            eVerticalDirections forwordOrBackword = movesForwardOrBackward(this, i_Rival);
            eHorizontalDirections rightOrLeft = movesRightOrLeft(this, i_Rival);

            Coordinate currentSquare = new Coordinate();
            Coin kingCopy = new Coin(CoinType, Board);
            kingCopy.Coordinates.CopyCoordinates(Coordinates);
            currentSquare = GetNextSquare(ref kingCopy, forwordOrBackword, rightOrLeft);
            kingCopy.Coordinates = prevSquare;

            while (currentSquare != null && Board.IsEmptyValidSquare(currentSquare))
            {
                prevSquare.CopyCoordinates(currentSquare);
                currentSquare = GetNextSquare(ref kingCopy, forwordOrBackword, rightOrLeft);
            }

            if (Board.IsValid(currentSquare))
            {
                isEatingPossibility = kingCopy.IsAbleToEat(currentSquare, out Coordinate target);
                if (isEatingPossibility)
                {
                    o_Target.CopyCoordinates(target);
                }
            }
            
            return isEatingPossibility;
        }

        public override bool isValidMove(Coordinate i_Target)
        {
            bool isValid = false;

            if (i_Target != null)
            {
                eVerticalDirections vertical = movesForwardOrBackward(this, i_Target);
                eHorizontalDirections horizontal = movesRightOrLeft(this, i_Target);

                Coordinate prevSquare = new Coordinate();
                prevSquare.CopyCoordinates(Coordinates);

                Coin kingCopy = new Coin(CoinType, Board);
                kingCopy.Coordinates.CopyCoordinates(Coordinates);
                Coordinate nextSquare = GetNextSquare(ref kingCopy, vertical, horizontal);
                kingCopy.Coordinates = prevSquare;

                while (Board.IsValid(nextSquare))
                {
                    if (Board.IsEmptySquare(nextSquare) && nextSquare.Equals(i_Target))
                    {
                        isValid = !isValid; // true
                        break;
                    }
                    else if (kingCopy.IsAbleToEat(nextSquare, out Coordinate target)
                        && i_Target.Equals(target))
                    {
                        isValid = !isValid; // true
                        break;
                    }
                    prevSquare.CopyCoordinates(nextSquare);
                    nextSquare = GetNextSquare(ref kingCopy, vertical, horizontal);
                }
            }

            return isValid;
        }

        public override void UpdateMoves()
        {
            List<Coordinate> newMoves = new List<Coordinate>();

            newMoves.AddRange(getNextMovesByDirections(eVerticalDirections.Forword, eHorizontalDirections.Right));
            newMoves.AddRange(getNextMovesByDirections(eVerticalDirections.Forword, eHorizontalDirections.Left));
            newMoves.AddRange(getNextMovesByDirections(eVerticalDirections.Backword, eHorizontalDirections.Right));
            newMoves.AddRange(getNextMovesByDirections(eVerticalDirections.Backword, eHorizontalDirections.Left));

            AvailableCoordinates = newMoves;
        }
        #endregion Public Methods

        #region Private Methods
        private void setKingSign()
        {
            Sign = (CoinType == eCoinType.X) ? 'K' : 'U';
        }

        private List<Coordinate> getNextMovesByDirections(eVerticalDirections i_Vertical, eHorizontalDirections i_Horizontal)
        {
            List<Coordinate> moves = new List<Coordinate>();

            Coordinate prevSquare = new Coordinate();
            prevSquare.CopyCoordinates(Coordinates);

            Coin kingCopy = new KingCoin(CoinType, Board);
            kingCopy.Coordinates.CopyCoordinates(Coordinates);
            Coordinate nextSquare = GetNextSquare(ref kingCopy, i_Vertical, i_Horizontal);
            kingCopy.Coordinates = prevSquare;


            while (Board.IsValid(nextSquare))
            {
                if (Board.IsEmptySquare(nextSquare))
                {
                    moves.Add(nextSquare);
                }
                else if (kingCopy.IsAbleToEat(nextSquare, out Coordinate target))
                {
                    moves.Add(target);
                    break;
                }
                else
                {
                    break;
                }
                prevSquare.CopyCoordinates(nextSquare);
                nextSquare = GetNextSquare(ref kingCopy, i_Vertical, i_Horizontal);
            }

            return moves;
        }

        #endregion Private Methods
    }
}
