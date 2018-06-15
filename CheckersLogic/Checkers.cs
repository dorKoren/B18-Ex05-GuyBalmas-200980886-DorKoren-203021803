using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Ex05.CheckersLogic.GameBoard;
using static Ex05.CheckersLogic.Enums;

namespace Ex05.CheckersLogic
{
   public class Checkers
    {
        #region Data members
        private readonly int r_NumOfRowsToFillForSize8AndAbove = 3;
        private readonly int r_NumOfRowsToFillForBelowSize8Board = 2;
        private static readonly int[] r_ValidBoardSizes = { 6, 8, 10 };
        private const eCoinType k_FirstPlayerCoinType = eCoinType.O;
        private const eCoinType k_SecondPlayerCoinType = eCoinType.X;

        GameBoard m_Board;
        Player m_Player1;   // Assume player1 coins move: up --> bottom
        Player m_Player2;  // Assume player2 coins move: bottom --> up
        #endregion Data members

        #region Constructor
        public Checkers(Player i_Player1, Player i_Player2, int i_BoardSize)
        {
            this.m_Board = new GameBoard(i_BoardSize);
            this.m_Player1 = i_Player1;
            this.m_Player2 = i_Player2;
        }
        #endregion Constructor

        #region Properties
        public GameBoard Board
        {
            get { return m_Board; }
            set { m_Board = value; }
        }

        public Player Player1
        {
            get { return m_Player1; }
        }

        public Player Player2
        {
            get { return m_Player2; }
        }

        #endregion Properties

        #region Public Methods
        /// <summary>
        /// This method moves the given coin to the desired coordinate.
        /// If the move is invalid, the method will not change any coins location.
        /// </summary>
        /// <param name="i_Coin"></param>
        /// <param name="i_Target"></param>
        /// <returns>Type of move executed (Step, Eat, None)</returns>
        public eMoveType MoveCoin(Coin i_Coin, Coordinate i_Target)
        {
            eMoveType moveType = eMoveType.None;

            if (i_Coin != null && i_Target != null)
            {
                if (i_Coin is KingCoin)
                {
                    KingCoin king = i_Coin as KingCoin;
                    if (king.IsEatingMove(i_Target, out Coordinate rivalCoord) && king.AvailableCoordinates.Contains(i_Target))
                    {
                        moveType = eMoveType.Eat;
                        eatCoin(king, GetCoinFromBoard(rivalCoord));
                    }
                    else if (king.isValidMove(i_Target) && king.AvailableCoordinates.Contains(i_Target))
                    {
                        moveType = eMoveType.Step;
                        moveCoin(king, i_Target);
                    }

                    UpdatePlayersAvailableMoves();
                }
                else
                {
                    if (i_Coin.IsEatingMove(i_Target, out Coordinate rivalCoord) &&
                        i_Coin.AvailableCoordinates.Contains(i_Target))
                    {
                        moveType = eMoveType.Eat;
                        eatCoin(i_Coin, GetCoinFromBoard(rivalCoord));
                    }
                    else if (i_Coin.isValidMove(i_Target) && 
                        i_Coin.AvailableCoordinates.Contains(i_Target))
                    {
                        moveType = eMoveType.Step;
                        moveCoin(i_Coin, i_Target);
                    }
                    // If needs to be turned to a King coin
                    if (hasReachedTheFarSideOfTheBoard(i_Coin))
                    {
                        KingCoin newKing = i_Coin.MakeKing();
                        Player coinOwner = GetCoinOwner(i_Coin);
                        Board.RemoveCoinFromBoard(i_Coin);
                        coinOwner.CoinsList.Remove(i_Coin);
                        coinOwner.CoinsList.Add(newKing);
                        Board.SetCoinOnBoard(newKing);
                    }
                }

                UpdatePlayersAvailableMoves();
            }

            return moveType;
        }

        public void UpdatePlayersAvailableMoves()
        {
            List<Player> players = new List<Player>();
            players.Add(Player1);
            players.Add(Player2);

            foreach (Player currentPlayer in players)
            {
                foreach (Coin currentCoin in currentPlayer.CoinsList)
                {
                    if (!currentCoin.IsKing)
                    {
                        currentCoin.UpdateMoves();
                    }
                    // If king
                    else if (currentCoin is KingCoin)
                    {
                        KingCoin kingCopy = currentCoin as KingCoin;
                        kingCopy.UpdateMoves();
                    }
                }

                // Clean no eating moves                 
                if (HasEatingMoves(currentPlayer))
                {
                    cleanNoEatingMoves(currentPlayer.CoinsList);
                }
            }
        }

        /// <summary>
        /// This method assigns Coins to the given player.
        /// The Coins assigned are init to the starting game coordinates.
        /// </summary>
        /// <param name="i_Player"></param>
        public void GiveCoinsToPlayer(Player i_Player)
        {
            if (i_Player != null)
            {
                Coordinate[] coinCoordinates = initialCoinsCoordinates(i_Player);
                eCoinType coinsType = eCoinType.None;
                List<Coin> coinsList = new List<Coin>();

                if (i_Player.Equals(this.m_Player1))
                {
                    coinsType = eCoinType.O;
                }
                else
                {
                    coinsType = eCoinType.X;
                }

                for (int coinIndex = 0; coinIndex <
                    coinCoordinates.Length; coinIndex++)
                {
                    Coin newCoin = new Coin(coinsType, Board);
                    {
                        newCoin.Coordinates = coinCoordinates[coinIndex];
                    }

                    coinsList.Add(newCoin);
                }

                i_Player.CoinsList = coinsList;
            }
        }

        public void SwitchTurnFlags()
        {
            if (Player1.HisTurn)
            {
                switchTurnFlags(Player1, Player2);
            }
            else
            {
                switchTurnFlags(Player2, Player1);
            }
        }

        public void SwitchTurns(ref Player o_CurrnetPlayer, ref Player o_WaitingPlayer)
        {
            Player tempPlayer = o_CurrnetPlayer;
            o_CurrnetPlayer = o_WaitingPlayer;
            o_WaitingPlayer = tempPlayer;
        }
 
        public bool IsDraw()
        {
            bool isDraw = false;

            if (!Player1.HaveFreeCoins() && !Player2.HaveFreeCoins())
            {
                isDraw = !isDraw;
                Player1.Status = eStatus.Draw;
                Player2.Status = eStatus.Draw;
            }

            return !Player1.HaveFreeCoins() && !Player2.HaveFreeCoins();
        }

        public bool HasEatingMoves(Player i_Player)
        {
            bool hasEatingMoves = false;
            if (i_Player != null)
            {
                foreach (Coin currentCoin in i_Player.CoinsList)
                {
                    if (currentCoin.HasEatingMoves())
                    {
                        hasEatingMoves = !hasEatingMoves; // true
                        break;
                    }
                    else if (currentCoin is KingCoin)
                    {
                        KingCoin kingCopy = currentCoin as KingCoin;
                        if (kingCopy.HasEatingMoves())
                        {
                            hasEatingMoves = !hasEatingMoves; // true
                            break;
                        }
                    }
                }
            }
            return hasEatingMoves;
        }

        public Player GetCoinOwner(Coin i_Coin)
        {
            Player player = null;
            if (i_Coin.CoinType == k_FirstPlayerCoinType)
            {
                player = Player1;
            }
            else if (i_Coin.CoinType == k_SecondPlayerCoinType)
            {
                player = Player2;
            }
            return player;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// This method given a player, returns the player's coins' coordinates
        /// of the initial game state.
        /// </summary>
        /// <param name="i_Player"></param>
        /// <returns></returns>
        private Coordinate[] initialCoinsCoordinates(Player i_Player)
        {
            int numCoinsInEachRow = this.m_Board.BoardSize / 2;

            int numOfRowsToFill = (Board.BoardSize < 8) ?
                r_NumOfRowsToFillForBelowSize8Board :
                r_NumOfRowsToFillForSize8AndAbove;

            Coordinate[] coinsCoordinates =
                new Coordinate[numOfRowsToFill * numCoinsInEachRow];

            int coordinateIndex = 0;
            int row, rowsLimit;

            // For the first player, valid coordinates will be in the first
            // top 2 or 3 rows.
            if (i_Player.Equals(Player1))
            {
                row = 0;
                rowsLimit = numOfRowsToFill;
            }

            // For the second player, valid coordinates will be in the last
            // bottum 2 or 3 rows.
            else
            {
                row = Board.BoardSize - numOfRowsToFill;
                rowsLimit = Board.BoardSize;
            }

            for (; row < rowsLimit; row++)
            {
                // Even row numbers goto odd column numbers, and vise versa 
                int column = (row % 2 == 0) ? 1 : 0;

                for (; column < m_Board.BoardSize; column += 2)
                {
                    // Creat a new coordinate with respect to the given
                    // row and column
                    Coordinate newCoordinate = new Coordinate(row, column);
                    coinsCoordinates[coordinateIndex] = newCoordinate;
                    coordinateIndex++;
                }
            }

            return coinsCoordinates;
        }

        public Coin GetCoinFromBoard(Coordinate i_Target)
        {
            Coin target = null;
            eCoinType type = Board.GetCoinType(i_Target);

            if (type == k_FirstPlayerCoinType)
            {
                target = getCoinFromPlayer(Player1, i_Target);
            }
            else if (type == k_SecondPlayerCoinType)
            {
                target = getCoinFromPlayer(Player2, i_Target);
            }

            return target;
        }

        private Coin getCoinFromPlayer(Player i_Player, Coordinate i_Target)
        {
            Coin desiredCoin = null;

            foreach (Coin currentCoin in i_Player.CoinsList)
            {
                if (currentCoin.Coordinates.Equals(i_Target))
                {
                    desiredCoin = currentCoin;
                    break;
                }
            }

            return desiredCoin;
        }

        private void moveCoin(Coin i_Coin, Coordinate i_Target)
        {
            Board.RemoveCoinFromBoard(i_Coin);
            i_Coin.Coordinates.CopyCoordinates(i_Target);
            Board.SetCoinOnBoard(i_Coin);
            i_Coin.UpdateMoves();
        }

        private bool hasReachedTheFarSideOfTheBoard(Coin i_Coin)
        {
            bool hasReached = true;

            if (GetCoinOwner(i_Coin).Equals(Player1))
            {
                hasReached = (i_Coin.Coordinates.Row == Board.BoardSize - 1)
                    ? hasReached : !hasReached;
            }

            else
            {
                hasReached = (i_Coin.Coordinates.Row == 0) ?
                    hasReached : !hasReached;
            }

            return hasReached;
        }

        private void eatCoin(Coin i_Coin, Coin i_RivalCoin)
        {
            if (i_Coin != null && i_RivalCoin != null)
            {
                if (i_Coin is KingCoin)
                {
                    KingCoin kingCopy = i_Coin as KingCoin;

                    if (kingCopy.IsAbleToEat(i_RivalCoin.Coordinates, out Coordinate target))
                    {
                        Board.RemoveCoinFromBoard(kingCopy);
                        kingCopy.Coordinates.CopyCoordinates(target);
                        Board.SetCoinOnBoard(kingCopy);
                        Player rivalPlayer = GetCoinOwner(i_RivalCoin);
                        Board.RemoveCoinFromBoard(i_RivalCoin);
                        rivalPlayer.CoinsList.Remove(i_RivalCoin);
                        kingCopy.UpdateMoves();
                    }
                }
                else if (!i_Coin.IsKing && i_Coin.IsAbleToEat(i_RivalCoin.Coordinates, out Coordinate target))
                {
                    Board.RemoveCoinFromBoard(i_Coin);
                    i_Coin.Coordinates.CopyCoordinates(target);
                    Board.SetCoinOnBoard(i_Coin);
                    Player rivalPlayer = GetCoinOwner(i_RivalCoin);
                    Board.RemoveCoinFromBoard(i_RivalCoin);
                    rivalPlayer.CoinsList.Remove(i_RivalCoin);
                    i_Coin.UpdateMoves();
                }                
            }
        }
        
        private void switchTurnFlags(Player i_ThisPlayer, Player i_OtherPlayer)
        {
            i_ThisPlayer.HisTurn = !i_ThisPlayer.HisTurn;   // Update this player flag from true to fale.
            i_OtherPlayer.HisTurn = !i_OtherPlayer.HisTurn; // Update other player flag from false to true.   
        }

        private void cleanNoEatingMoves(List<Coin> i_CoinsList)
        {
            if (i_CoinsList.Any())
            {
                foreach (Coin currentCoin in i_CoinsList)
                {
                    List<Coordinate> eatingPossibilities = new List<Coordinate>();

                    foreach (Coordinate currentCoordinate in currentCoin.AvailableCoordinates)
                    {
                        if (!currentCoin.IsKing && currentCoin.IsEatingMove(currentCoordinate, out Coordinate o_RivalCoord))
                        {
                            eatingPossibilities.Add(currentCoordinate);
                        }
                        else if (currentCoin is KingCoin)
                        {
                            KingCoin kingCopy = currentCoin as KingCoin;
                            if (kingCopy.IsEatingMove(currentCoordinate, out Coordinate o_RivalCoordinate))
                            {
                                eatingPossibilities.Add(currentCoordinate);
                            }
                        }
                    }
                    currentCoin.AvailableCoordinates = eatingPossibilities;
                }
            }
        }
            #endregion Private Methods

        }
    }