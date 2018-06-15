using System;
using System.Drawing;
using System.Windows.Forms;
using Ex05.CheckersLogic;
using static Ex05.CheckersLogic.Enums;
using static Ex05.CheckersLogic.GameBoard;

namespace Ex05.WindowsUI
{
    public class BoardButton : Button
    {
        #region Class members

        public event EventHandler CoinChangedStaus; 

        private GameBoard.Coordinate m_Coordinate;
        private const int k_Size = 50;
        #endregion Class members

        #region Constructor
        public BoardButton(int i_RowIndex, int i_ColIndex)
        {          
            this.Height = k_Size;
            this.Width = k_Size;
            this.Enabled = false;
            this.BackColor = initialBackColor(i_RowIndex, i_ColIndex);
            this.m_Coordinate = new Coordinate(i_RowIndex, i_ColIndex);         
        }

        #endregion Constructor

        #region Properties
        public Coordinate Coordinate
        {
            get { return m_Coordinate; }
        }
        #endregion Properties

        #region Methods
        private Color initialBackColor(int i_RowIndex, int i_ColIndex)
        {
            bool isEvenRow = (i_RowIndex % 2 == 0);
            bool isEvenCol = (i_ColIndex % 2 == 0);

            Color color =
                ((isEvenRow && !isEvenCol) || (!isEvenRow && isEvenCol)) ?
                Color.White :
                Color.Gray;

            return color;
        }
        
        #endregion Methods
    }
}