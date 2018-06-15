using System;
using System.Linq;
using static Ex05.CheckersLogic.Enums;
using static Ex05.CheckersLogic.GameBoard;

namespace Ex05.CheckersLogic
{
    public class Computer : Player
    {
        #region Constructors
        public Computer() : base("Computer", ePlayersType.Computer, false)
        {
        }
        #endregion Constructors

        #region Public Methods
        public Coin ChooseRandomCoin()
        {

            Random random = new Random();
            int numbersOfCoins = CoinsList.Count;
            Coin newCoin = null;
            // Choose a random coin.
            int randomCoin = random.Next(0, numbersOfCoins);

            while (this.HasMoreCoins() && !CoinsList.ElementAt(randomCoin).IsFree())
            {
                randomCoin = random.Next(0, numbersOfCoins);
            }

            newCoin = CoinsList.ElementAt(randomCoin);
            return newCoin;
        }

        public Coordinate ChooseRandomCoordinate(Coin i_Coin)
        {
            Coordinate newCoord = new Coordinate();
            if (i_Coin != null && i_Coin.IsFree())
            {
                Random random = new Random();
                int numberOfAvailableCoordinates = i_Coin.AvailableCoordinates.Count;

                // Choose a random available coordinate
                int randomAvailableCoordinate = random.Next(0, numberOfAvailableCoordinates);
                newCoord = i_Coin.AvailableCoordinates.ElementAt(randomAvailableCoordinate);
            }

            return newCoord;
        }
        #endregion Public Methods
    }
}
