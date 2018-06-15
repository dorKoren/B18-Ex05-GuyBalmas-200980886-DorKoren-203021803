using System;
using System.Text;
using static Ex05.CheckersLogic.Enums;

namespace Ex05.CheckersLogic
{
    public class GameBoard
    {
    
        #region Nested Coordinate Class
        // Access modifier is public for the use of Coin class 
        // (each coin holds its location on the board).
        public class Coordinate
        {
            #region Regular members
            private int m_Row;
            private int m_Column;
            #endregion Regular members

            #region Constructors
            public Coordinate() 
            {
                Row = -1;
                Column = -1;
            }
            public Coordinate(int i_Row, int i_Column)
            {
                Row = i_Row;
                Column = i_Column;
            }

            public Coordinate(char i_Row, char i_Column)
            {
                Row = i_Row - 'a';
                Column = i_Column - 'A';
            }
            #endregion Constructors

            #region Properties
            public int Row
            {
                get { return this.m_Row; }            
                set { this.m_Row = value; }
            }

            public int Column
            {
                get { return this.m_Column; }
                set { this.m_Column = value; }
            }
            #endregion Properties


            #region public methods

            public bool IsUnset()
            {
                return (Row == -1) && (Column == -1);
            }

            /// <summary>
            /// Checks if the given 2 coordinates are equal by
            /// comparing their Row and Column properties.
            /// Two coordinates are equal if and only if they have the same
            /// Row and Column values.
            /// 
            /// </summary>
            /// <param name="i_FirstCoordinate"></param>
            /// <param name="i_SecondCoordinate"></param>
            /// <returns></returns>
            public override bool Equals(Object i_Object)     
            {                                               
                bool equalCoordinates = true;

                // Check for null values and compare run-time types.
                if (i_Object == null || GetType() != i_Object.GetType())
                {
                    equalCoordinates = !equalCoordinates;
                }
                else
                {
                    Coordinate otherCoordinate = i_Object as Coordinate;

                    if ((Row != otherCoordinate.Row) || (Column != otherCoordinate.Column))
                    {
                        equalCoordinates = !equalCoordinates;
                    }
                }

                return equalCoordinates;
            }

            public override int GetHashCode()      
            {
                return Row ^ Column;
            }

            public override string ToString()
            {
                return ("(" + Row + ", " + Column + ")");
            }
            #endregion public methods

            #region Internal methods
            /// <summary>
            /// This Method gets a given coordinate, and calculates 
            /// the distance from this coordinate.
            /// 
            /// </summary>
            /// <param name="i_OtherCoordinate"></param>
            /// <returns >distance in squares on the board</returns>
            public int Distance(Coordinate i_OtherCoordinate)
            {
                double dist = 0;

                dist = (Math.Pow(Row - i_OtherCoordinate.Row, 2) + 
                    Math.Pow(Column - i_OtherCoordinate.Column, 2));

                dist = Math.Sqrt(dist);

                return (int)dist;
            }

            /// <summary>
            /// Copy by value the given coordinate into this one
            /// </summary>
            /// <param name="i_other"></param>
            public void CopyCoordinates(Coordinate i_other) 
            {
                Row = i_other.Row;
                Column = i_other.Column;
            }
            #endregion Public methods
        }

        #endregion Nested Coordinate Class

        #region Data members

        public event EventHandler SetCoinOnButton;
        public event EventHandler RemoveCoinFromButton;


        /* Const Members */
        private const string k_LineBuffer = "====";
        private const string k_ColumnBuffer = "|";
        private const string k_EmptySpace = "   ";
        private const char k_EmptySquare = ' ';

        /* Regular Members */
        private int m_BoardSize;
        private char[,] m_BoardMatrix;
        #endregion Data members

        #region Constructor
        public GameBoard(int i_BoardSize)
        {
            if (i_BoardSize < 0)            
            {
                throw new ArgumentException("Board size must be positive!");
            }
            else
            {
                createNewGameBoard(i_BoardSize);
            }
        }
        #endregion Constructor

        #region Properties
        public int BoardSize
        {
            get { return this.m_BoardSize; }
            set { this.m_BoardSize = value; }
  
        }

        public char[,] BoardMatrix
        {
            get { return this.m_BoardMatrix; }     
            set { this.m_BoardMatrix = value; }
      
        }
        #endregion Properties

        #region public Methods
        
        public void SetPlayerCoinsOnBoard(Player i_Player)
        {
            if (i_Player != null)
            {
                foreach (Coin currentCoin in i_Player.CoinsList)
                {
                    SetCoinOnBoard(currentCoin);
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public void ClearLogicBoard()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    BoardMatrix[i, j] =  k_EmptySquare;
                }
            }
        }

        /// <summary>
        /// Set the given coin on the board according his coordinate.
        /// </summary>
        /// <param name="i_Coin"></param>
        public void SetCoinOnBoard(Coin i_Coin)
        {
            if (i_Coin != null)
            {
                m_BoardMatrix[
                i_Coin.Coordinates.Row,
                i_Coin.Coordinates.Column] = i_Coin.Sign;

                if (SetCoinOnButton != null)
                {
                    SetCoinOnButton.Invoke(i_Coin, new EventArgs());
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Remove the given Coin from the board.
        /// </summary>
        /// <param name="i_Coin"></param>
        public void RemoveCoinFromBoard(Coin i_Coin)
        {
            if (i_Coin != null)
            {
                if (IsValid(i_Coin.Coordinates))
                {
                    m_BoardMatrix[i_Coin.Coordinates.Row, i_Coin.Coordinates.Column] = k_EmptySquare;

                    if (RemoveCoinFromButton != null)
                    {
                        RemoveCoinFromButton.Invoke(i_Coin.Coordinates, new EventArgs());
                    }

                    i_Coin.Coordinates = new Coordinate();
                }
            }
            else
            {
                throw new ArgumentNullException();
            }

        }

        public bool IsValid(Coordinate i_Location)
        {
            bool isValid = false;
            if (i_Location != null)
            {
                if (i_Location != null)
                {
                    isValid = areValidCoordinates(i_Location.Row, i_Location.Column);
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
            return isValid;
        }

        /// <summary>
        /// This method checks if the given coordinate points to an empty 
        /// tile on the board.
        /// </summary>
        /// <param name="i_Location"></param>
        /// <returns></returns>
        public bool IsEmptySquare(Coordinate i_Location)
        {
            bool isEmptyTile = false;

            if (IsValid(i_Location))
            {
                // If this location is empty
                if (BoardMatrix[i_Location.Row, i_Location.Column] 
                    == k_EmptySquare)
                {
                    // isEmptyTile is true
                    isEmptyTile = !isEmptyTile;
                }
            }

            return isEmptyTile;
        }

        public bool IsEmptyValidSquare(Coordinate i_Coordinate)
        {
            bool isEmpty = IsEmptySquare(i_Coordinate);
            bool isValid = IsValid(i_Coordinate);
            return (isValid && isEmpty);
        }

        public eCoinType GetCoinType(Coordinate i_Target)
        {
            eCoinType type = eCoinType.None;

            if (i_Target != null && IsValid(i_Target))
            {
                char sign = BoardMatrix[i_Target.Row, i_Target.Column];
                switch (sign)
                {
                    case 'O':
                        type = eCoinType.O;
                        break;
                    case 'U':
                        type = eCoinType.O;
                        break;
                    case 'X':
                        type = eCoinType.X;
                        break;
                    case 'K':
                        type = eCoinType.X;
                        break;
                    default:
                        break;
                }
            }

            return type;
        }


        #endregion public Methods

        #region Private methods
        private void createNewGameBoard(int i_Size)
        {
            Char[,] newBoardMatrix = new Char[i_Size, i_Size];
            this.m_BoardSize = i_Size;

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    newBoardMatrix[i, j] = k_EmptySquare;
                }
            }

            this.m_BoardMatrix = newBoardMatrix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i_Row"></param>
        /// <param name="i_Column"></param>
        /// <returns></returns>
        private bool areValidCoordinates(int i_Row, int i_Column)
        {
            bool validCoordinates = true;

            if (i_Row < 0 || 
                i_Row >= BoardSize ||
                i_Column < 0 ||
                i_Column >= BoardSize)
            {
                validCoordinates = !validCoordinates;
            }

            return validCoordinates;
        }

        private static bool isEmptySquare(char i_Square)
        {
            bool isEmptySquare = true;

            return (i_Square == k_EmptySquare) ? isEmptySquare : !isEmptySquare;

        }
        #endregion Private methods
    }
}