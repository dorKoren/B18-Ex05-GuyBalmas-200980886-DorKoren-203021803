using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using static Ex05.WindowsUI.BoardButton;
using Ex05.CheckersLogic;
using static Ex05.CheckersLogic.Enums;
using static Ex05.CheckersLogic.GameBoard;
using System;

namespace Ex05.WindowsUI
{
    public class FormGameBoard : Form
    {
        #region Class members
        private const int k_SquareSize = 50;
        private const int k_SquareSpace = 10; 
        private const string k_FormTitle = "Damka";

        private Label labelPlayer1 = new Label();
        private Label labelPlayer2 = new Label();
        private BoardButton[,] m_BoardButtons;

        private Engine m_Engine;   
       #endregion Class members

        #region Constructor
        public FormGameBoard(GameSettings i_GameSettings)
        {
            m_Engine = new Engine(i_GameSettings);


            this.Text = k_FormTitle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            this.Width  =  k_SquareSpace + 
                (i_GameSettings.BoardSize * (k_SquareSize + k_SquareSpace));

            this.Height = (6 * k_SquareSpace) +
                (i_GameSettings.BoardSize * (k_SquareSize + k_SquareSpace));

            registerEventHandlers();
            initControls(i_GameSettings);
        }
        #endregion Constructor

        #region Properties
        public Engine Engine
        {
            get { return m_Engine; }
        }
        #endregion Properties

        #region Private methods
        private BoardButton getButtonFromBoard(Coordinate i_Coordinate)
        {
            int row = i_Coordinate.Row;
            int col = i_Coordinate.Column;

            return m_BoardButtons[row,col];
        }

        private void registerEventHandlers()
        {        
            m_Engine.EndOfGame += new EndOfGameHandler(m_Engine_EndOfGame);
            m_Engine.TurnMovedToNextPlayer += new EventHandler(m_Engine_TurnMovedToNextPlayer);
            m_Engine.PlayerHasScored += new EventHandler(m_Engine_PlayerHasScored);

            m_Engine.Board.SetCoinOnButton += new EventHandler(m_Engine_Board_SetCoinOnButton);
            m_Engine.Board.RemoveCoinFromButton += new EventHandler(m_Engine_Board_RemoveCoinFromButton);
        }

        private void initControls(GameSettings i_GameSettings)
        {
            initLabelPlayer1(i_GameSettings.Player1Name);
            initLabelPlayer2(i_GameSettings.Player2Name);
            initUIBoard(i_GameSettings.BoardSize);

            this.Controls.AddRange(new Control[] { labelPlayer1, labelPlayer2 });
        }

        public void initUIBoard(int i_BoardSize)  
        {
            Point anchor = new Point(k_SquareSize / 2, k_SquareSize);

            m_BoardButtons = new BoardButton[i_BoardSize, i_BoardSize];

            for (int i = 0; i < i_BoardSize; i++)  
            {
                for (int j = 0; j < i_BoardSize; j++)  
                {
                    m_BoardButtons[i, j] = new BoardButton(i, j);
                    BoardButton currentButton = m_BoardButtons[i, j];

                    currentButton.Location = new Point(
                        anchor.X + (j * currentButton.Width),
                        anchor.Y + (i * currentButton.Height));

                    this.Controls.Add(currentButton);

                    currentButton.Click +=
                        new EventHandler(currentButton_Click);
                }
            }

            m_Engine.Checkers.Board.ClearLogicBoard();
            m_Engine.ResetGame();
        }

        private void m_Engine_Board_RemoveCoinFromButton(object sender, EventArgs e)
        {
            Coordinate coordinate = sender as Coordinate;
            getButtonFromBoard(coordinate).Text = "";
        }

        private void m_Engine_Board_SetCoinOnButton(object sender, EventArgs e)
        {
            Coin coin = sender as Coin;
            int row = coin.Coordinates.Row;
            int col = coin.Coordinates.Column;
            BoardButton button = m_BoardButtons[row, col];
                     
            switch(coin.CoinType)
            {
                case (eCoinType.O):
                    button.Enabled = true;
                    button.Text = coin.Sign + ""; 
                    break;

                case (eCoinType.X):
                    button.Enabled = false;
                    button.Text = coin.Sign + "";  
                    break;

                default:
                    break;
            }
        }

          
        private void initLabelPlayer1(string i_Player1Name)
        {
            if (!(m_Engine.Checkers.Player2 is Computer))
            {
                this.labelPlayer1.Font = new Font(labelPlayer1.Font, FontStyle.Bold);
            }

            this.labelPlayer1.Text = string.Format("{0}: {1}", i_Player1Name, 0);   
            this.labelPlayer1.AutoSize = true;
            this.labelPlayer1.Location =
                new Point((this.Width / 4), k_SquareSize / 2);
        }

        private void initLabelPlayer2(string i_Player2Name)
        {
            this.labelPlayer2.Font = new Font(labelPlayer1.Font, FontStyle.Regular);
            this.labelPlayer2.Text = string.Format("{0}: {1}", i_Player2Name, 0); 
            this.labelPlayer2.AutoSize = true;
            this.labelPlayer2.Location = 
                new Point((1 * this.Width) / 2, k_SquareSize / 2);
        }

        private void currentButton_Click(object sender, EventArgs e)
        {
            bool enable = true;
            BoardButton button = sender as BoardButton;
            m_Engine.SourceCoordinate = button.Coordinate;
     
            if (button.Enabled)
            {
                // Unselect the current Coin
                if (m_Engine.SourceCoordinate.Equals(m_Engine.LastCoordinate))
                {
                    button.BackColor = Color.White;
                    ChangeAvailableCoordinatesColor(button, Color.White);
                    changeOtherCoinsAvailability(button, enable);
                    m_Engine.ResetCoordinates();
                }
                else
                {
                    if (m_Engine.LastCoordinate == null)
                    {
                        m_Engine.LastCoordinate = m_Engine.SourceCoordinate;
                        button.BackColor = Color.LightBlue;
                        ChangeAvailableCoordinatesColor(button, Color.Chocolate);
                        changeOtherCoinsAvailability(button, !enable);
                    }
                    else
                    {
                        m_Engine.TargetCoordinate = button.Coordinate;

                        BoardButton sourceButton = getButtonFromBoard(m_Engine.LastCoordinate);
                        ChangeAvailableCoordinatesColor(sourceButton, Color.White);
                        changeOtherCoinsAvailability(sourceButton, enable);
                        sourceButton.BackColor = Color.White;

                        if (!m_Engine.GameOver() && !(m_Engine.CurrentPlayer is Computer))
                        {
                            MakeCurrentPlayerCoinsDisabled();
                            m_Engine.TakeMoveFromPlayer();
                            m_Engine.SwitchTurnsIfNecessary();
                            m_Engine.ResetCoordinates();
                        }

                        if (!m_Engine.GameOver() && m_Engine.CurrentPlayer is Computer)
                        {
                            MakeCurrentPlayerCoinsDisabled();
                            m_Engine.TakeMoveFromPlayer();
                            m_Engine.SwitchTurnsIfNecessary();
                            m_Engine.ResetCoordinates();
                            m_Engine.GameOver();
                        }
                    }
                }           
            }           
        }

        // $G$ CSS-011 (-3) Bad private method name. Should be pascalCased.
        private void MakeCurrentPlayerCoinsDisabled()
        {
            makeAllButtonsDisables();

            foreach (Coin coin in m_Engine.WaitingPlayer.CoinsList)
            {
                Coordinate coordinate = coin.Coordinates;
                getButtonFromBoard(coordinate).Enabled = true;
            }
        }

        private void makeAllButtonsDisables()
        {
            for (int i = 0; i < m_Engine.Board.BoardSize; i++)
            {
                for (int j = 0; j < m_Engine.Board.BoardSize; j++)
                {
                    m_BoardButtons[i, j].Enabled = false;
                }
            }
        }

        // $G$ CSS-013 (-5) Bad input variable name (should be in the form of i_PascalCased)
        private void changeOtherCoinsAvailability(
            BoardButton button,
            bool isEnable)
        {

            foreach (Coin coin in m_Engine.CurrentPlayer.CoinsList)
            {
                Coordinate coordinate = coin.Coordinates;

                if (!coordinate.Equals(button.Coordinate))
                {
                    getButtonFromBoard(coordinate).Enabled = isEnable;
                }
            }
        }

        private void ChangeAvailableCoordinatesColor(BoardButton i_Button, Color i_Color)
        {
            Coin coin = m_Engine.Checkers.GetCoinFromBoard(i_Button.Coordinate);
            
            if (coin != null)
            {
                foreach (Coordinate coordinate in coin.AvailableCoordinates)
                {
                    BoardButton currentButton = getButtonFromBoard(coordinate);

                    if (i_Color == Color.White)
                    {
                        currentButton.Enabled = false;
                    }
                    else
                    {
                        currentButton.Enabled = true;
                    }

                    currentButton.BackColor = i_Color;
                }
            }
        }
        private void m_Engine_PlayerHasScored(object sender, EventArgs e)
        {
            string Player1Name = m_Engine.Checkers.Player1.Name;
            string Player2Name = m_Engine.Checkers.Player2.Name;

            int player1Score = m_Engine.Checkers.Player1.TotalScore;
            int player2Score = m_Engine.Checkers.Player2.TotalScore;

            labelPlayer1.Text = string.Format("{0}: {1} ", Player1Name, player1Score);
            labelPlayer2.Text = string.Format("{0}: {1} ", Player2Name, player2Score);
        }

        private void m_Engine_TurnMovedToNextPlayer(object sender, EventArgs e)
        {
            Player currentPlayer = m_Engine.CurrentPlayer;
            Player player1 = m_Engine.Checkers.Player1;

            if (currentPlayer.Equals(player1))
            {
                labelPlayer1.Font = new Font(labelPlayer1.Font, FontStyle.Bold);
                labelPlayer2.Font = new Font(labelPlayer2.Font, FontStyle.Regular);

            } 
            else
            {
                labelPlayer2.Font = new Font(labelPlayer2.Font, FontStyle.Bold);
                labelPlayer1.Font = new Font(labelPlayer1.Font, FontStyle.Regular);
            }
        }

        private void m_Engine_EndOfGame(object sender, EndOfGameEventArgs e)
        {
            Player currentPlayer = m_Engine.CurrentPlayer;
            Player waitingPlayer = m_Engine.WaitingPlayer;
            currentPlayer.CalculateScore();
            waitingPlayer.CalculateScore();

            if (currentPlayer.Tie(waitingPlayer))
            {
                e.m_GameOver = showTieMessageBox();
            }
            else if (currentPlayer.IsLoose())
            {
                e.m_GameOver = showWinMessageBox(waitingPlayer);
            }
            else if (currentPlayer.Score != waitingPlayer.Score)
            {
                Player winner =
                    (currentPlayer.Score > waitingPlayer.Score) ?
                    currentPlayer :
                    waitingPlayer;

                e.m_GameOver = showWinMessageBox(winner);
            }
        }

        private bool showWinMessageBox(Player i_WinPlayer)
        {
            bool gameOver = false;

            string winMessage = String.Format(
@"{0} Won!
Another Round?", i_WinPlayer.Name);

            DialogResult dialogResult = 
                MessageBox.Show(winMessage, k_FormTitle, MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                clearUIBoard();
            }
            else
            {
                gameOver = !gameOver; // true
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }

            return gameOver;
        }

        private bool showTieMessageBox()
        {
            bool gameOver = false;
            string tieMessage = string.Format(
@"Tie!
Another Round?");

            DialogResult dialogResult = 
                MessageBox.Show(tieMessage, k_FormTitle, MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                clearUIBoard();
            }
            else
            {
                gameOver = !gameOver; // true
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }

            return gameOver;
        }

        private void clearUIBoard()
        {
            for (int i = 0; i < m_Engine.Board.BoardSize; i++)
            {
                for (int j = 0; j < m_Engine.Board.BoardSize; j++)
                {
                    m_BoardButtons[i, j].Text = "";
                    m_BoardButtons[i, j].Enabled = false;
                }
            }
        }
        #endregion Private methods
    }
}
