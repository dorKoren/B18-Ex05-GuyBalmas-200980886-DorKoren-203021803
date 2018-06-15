using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ex05.CheckersLogic.Enums;
using static Ex05.CheckersLogic.GameBoard;

namespace Ex05.CheckersLogic
{
    public static class Directions
    {
        #region Public Methods
        
        public static Coordinate NextForwordRight(Coin i_Coin)
        {
            Coordinate newCoord = new Coordinate();
            newCoord.CopyCoordinates(i_Coin.Coordinates);

            if (i_Coin.CoinType == eCoinType.O)           // Coin Belongs to First Player
            {
                newCoord.Row++;
                newCoord.Column--;
            }
            else if (i_Coin.CoinType == eCoinType.X)      // Coin Belongs to Second Player
            {
                newCoord.Row--;
                newCoord.Column++;
            }
            return newCoord;
        }

        public static Coordinate NextForwordLeft(Coin i_Coin)
        {
            Coordinate newCoord = new Coordinate();
            newCoord.CopyCoordinates(i_Coin.Coordinates);

            if (i_Coin.CoinType == eCoinType.O)           //Coin Belongs to First Player
            {
                newCoord.Row++;
                newCoord.Column++;
            }
            else if (i_Coin.CoinType == eCoinType.X)      // Coin Belongs to Second Player
            {
                newCoord.Row--;
                newCoord.Column--;
            }
            return newCoord;
        }

        public static Coordinate NextBackwordRight(Coin i_Coin)
        {
            Coordinate newCoord = new Coordinate();
            newCoord.CopyCoordinates(i_Coin.Coordinates);

            if (i_Coin.CoinType == eCoinType.O)           //Coin Belongs to First Player
            {
                newCoord.Row--;
                newCoord.Column--;
            }
            else if (i_Coin.CoinType == eCoinType.X)      // Coin Belongs to Second Player
            {
                newCoord.Row++;
                newCoord.Column++;
            }
            
            return newCoord;
        }

        public static Coordinate NextBackwordLeft(Coin i_Coin)
        {
            Coordinate newCoord = new Coordinate();
            newCoord.CopyCoordinates(i_Coin.Coordinates);

            if (i_Coin.CoinType == eCoinType.O)           //Coin Belongs to First Player
            {
                newCoord.Row--;
                newCoord.Column++;
            }
            else if (i_Coin.CoinType == eCoinType.X)      // Coin Belongs to Second Player
            {
                newCoord.Row++;
                newCoord.Column--;
            }
            
            return newCoord;
        }

        /// <summary>
        /// Gets the next coordinate on the board in relevence to the given i_Coin, 
        /// and with respect to the vertical and horizonatl directions.
        /// </summary>
        /// <param name="i_Coin"></param>
        /// <param name="i_Vertical"></param>
        /// <param name="i_Horizontal"></param>
        /// <returns></returns>
        public static Coordinate GetNextSquare(ref Coin i_Coin, eVerticalDirections i_Vertical, eHorizontalDirections i_Horizontal)     // Added ref 07.06.18
        {
            Coordinate newCoord = new Coordinate();
            if (i_Coin != null)
            {
                if (i_Vertical == eVerticalDirections.Forword)
                {
                    if (i_Horizontal == eHorizontalDirections.Right)
                    {
                        newCoord = NextForwordRight(i_Coin);
                    }
                    else if (i_Horizontal == eHorizontalDirections.Left)
                    {
                        newCoord = NextForwordLeft(i_Coin);
                    }
                }
                else if (i_Vertical == eVerticalDirections.Backword)
                {
                    if (i_Horizontal == eHorizontalDirections.Right)
                    {
                        newCoord = NextBackwordRight(i_Coin);
                    }
                    else if (i_Horizontal == eHorizontalDirections.Left)
                    {
                        newCoord = NextBackwordLeft(i_Coin);
                    }
                }
            }
            return newCoord;
        }

        /// <summary>
        /// This method checks if the given coin is to be moved forward 
        /// or backward with respect to the given coordinate; 
        /// Assumes: 
        /// Forward is (down) for first player and (up) for second player.
        /// Backward is (up) for first player and (down) for second player.         
        /// StayInPlace means the given coordinate has the same Row value
        /// as the given Coin's Row.         
        /// 
        /// </summary>
        /// <param name="i_Coin"></param>
        /// <param name="i_target"></param>
        /// <returns></returns>
        public static eVerticalDirections movesForwardOrBackward(
            Coin i_Coin,
            Coordinate i_target)
        {
            eVerticalDirections move = eVerticalDirections.SamePlace;
            int targetRow = i_target.Row;
            int sourceRow = i_Coin.Coordinates.Row;

            int diff = targetRow - sourceRow;

            if (diff != 0)
            {
                // This coin belongs to the first player 
                // (moves from up ---> down)
                if (i_Coin.CoinType == eCoinType.O)
                {
                    move = (diff > 0) ?
                        eVerticalDirections.Forword :
                        eVerticalDirections.Backword;
                }
                // This coin belongs to the second player 
                // (moves from down ---> up)
                else
                {
                    move = (diff < 0) ?
                        eVerticalDirections.Forword :
                        eVerticalDirections.Backword;
                }
            }

            return move;
        }

        public static eHorizontalDirections movesRightOrLeft(
            Coin i_Coin,
            Coordinate i_target)
        {
            eHorizontalDirections move = eHorizontalDirections.SamePlace;
            int targetColumn = i_target.Column;
            int sourceColumn = i_Coin.Coordinates.Column;

            int diff = targetColumn - sourceColumn;

            if (diff != 0)
            {
                // This coin belongs to the first player.
                if (i_Coin.CoinType == eCoinType.O)
                {
                    move = (diff < 0) ?
                        eHorizontalDirections.Right :
                        eHorizontalDirections.Left;
                }

                // This coin belongs to the second player 
                // (moves from down ---> up).
                else
                {
                    move = (diff > 0) ?
                        eHorizontalDirections.Right :
                        eHorizontalDirections.Left;
                }
            }

            return move;
        }
        #endregion Public Methods
    }
}
