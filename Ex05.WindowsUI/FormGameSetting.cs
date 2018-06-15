using System;
using System.Drawing;
using System.Windows.Forms;
using Ex05.CheckersLogic;
using static Ex05.CheckersLogic.Enums;

namespace Ex05.WindowsUI
{
    public class FormGameSetting : Form
    {
        #region Class Members
        private const string k_BoardSizeStr = "Board Size:";
        private const string k_Board6x6Str = "6x6:";
        private const string k_Board8x8Str = "8x8:";
        private const string k_Board10x10Str = "10x10:";
        private const string k_Player1AccecibleNameTextBox = "Player1";
        private const string k_Player2AccecibleNameTextBox = "Player2";
        private const string k_Player1Str = "Player 1:";
        private const string k_Player2Str = "Player 2:";
        private const string k_Player1Name = "";
        private const string k_Player2Name = "Computer";
        private const string k_PlayersStr = "Players:";
        private const string k_FormTitleStr = "Game Settings";
        private const string k_DefaultSinglePlayerText = "[Computer]";
        private const string k_DoneStr = "Done";

        private GameSettings m_GameSettings;

        private LabelSetting labelBoardSize;
        private LabelSetting labelPlayers;
        private LabelSetting labelPlayer1;

        private RadioButtonSetting radioButton6x6;
        private RadioButtonSetting radioButton8x8;
        private RadioButtonSetting radioButton10x10;

        private TextBoxSetting textBoxPlayer1Name;
        private TextBoxSetting textBoxPlayer2Name;

        private CheckBoxSetting checkBoxPlayer2;

        private ButtonSetting buttonDone;
        #endregion Class Members

        #region Constructor
        public FormGameSetting()
        {
            this.Size = new Size(220, 220);
            this.Text = k_FormTitleStr;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow; // Optional ? <-- yes

        
            m_GameSettings = new CheckersLogic.GameSettings();

            initSettings();
            initControls();
            registerEventHandlers();
        }
        #endregion Constructor

        #region Properties
        public GameSettings GameSettings
        {
            get { return m_GameSettings; }
        }
        #endregion Properties

        #region Methods
        private void initSettings()
        {
            m_GameSettings.IsSinglePlayer = true;
            m_GameSettings.Player1Name = k_Player1Name;
            m_GameSettings.Player2Name = k_Player2Name;
        }

        private void initControls()
        {
            bool isChecked = true;
            bool isEnabled = true;

            labelBoardSize = new LabelSetting(k_BoardSizeStr, 10, 10);

            radioButton6x6 = new RadioButtonSetting(
                k_Board6x6Str, 6, isChecked,
                labelBoardSize.Left + 10,labelBoardSize.Height + 15);

            radioButton8x8 = new RadioButtonSetting(
                k_Board8x8Str, 8, !isChecked,
                radioButton6x6.Right - 50, radioButton6x6.Top);

            radioButton10x10 = new RadioButtonSetting(
                k_Board10x10Str, 10, !isChecked,
                radioButton8x8.Right - 50, radioButton6x6.Top);

            labelPlayers = new LabelSetting(
                k_PlayersStr,
                labelBoardSize.Left,
                radioButton6x6.Bottom);

            labelPlayer1 = new LabelSetting(
                k_Player1Str,
                radioButton6x6.Left,
                labelPlayers.Bottom);

            textBoxPlayer1Name = new TextBoxSetting(
                k_Player1Name,
                k_Player1AccecibleNameTextBox,
                isEnabled,
                labelPlayer1.Right - 25,
                labelPlayers.Bottom);

            checkBoxPlayer2 = new CheckBoxSetting(
                k_Player2Str,
                labelPlayer1.Left,
                labelPlayer1.Bottom + 10);

            textBoxPlayer2Name = new TextBoxSetting(
                k_DefaultSinglePlayerText, 
                k_Player2AccecibleNameTextBox,
                !isEnabled,
                textBoxPlayer1Name.Left,
                textBoxPlayer1Name.Bottom + 10);

            buttonDone = new ButtonSetting(
                k_DoneStr, 
                textBoxPlayer1Name.Left + textBoxPlayer1Name.Width / 4,
                textBoxPlayer2Name.Bottom + 10);

            // Add all the controls to the game setting form.
            this.Controls.AddRange(new Control[] {
                labelBoardSize,
                radioButton6x6,
                radioButton8x8,
                radioButton10x10,
                labelPlayers,
                labelPlayer1,
                textBoxPlayer1Name,
                checkBoxPlayer2,
                textBoxPlayer2Name,
                buttonDone });
        }

        private void registerEventHandlers()
        {
            this.radioButton6x6.Click += new EventHandler(radioButton_Click);
            this.radioButton8x8.Click += new EventHandler(radioButton_Click);
            this.radioButton10x10.Click += new EventHandler(radioButton_Click);

            this.textBoxPlayer1Name.Click += new EventHandler(textBox_Click);
            this.textBoxPlayer2Name.Click += new EventHandler(textBox_Click);
            this.checkBoxPlayer2.Click += new EventHandler(checkBoxPlayer2_Click);
            this.buttonDone.Click += new EventHandler(buttonDone_Click);
        }

        private void radioButton_Click(object sender, EventArgs e)
        {
            m_GameSettings.BoardSize = (int)(sender as RadioButton).Tag; 
        }

        private void checkBoxPlayer2_Click(object sender, EventArgs e)
        {
            bool isSinglePlayer = true;  

            this.textBoxPlayer2Name.Enabled = !textBoxPlayer2Name.Enabled;

            if (this.textBoxPlayer2Name.Enabled)
            {
                m_GameSettings.Player2Name = textBoxPlayer2Name.Text;
                m_GameSettings.IsSinglePlayer = !isSinglePlayer;
            }
            else
            {
                m_GameSettings.Player2Name = k_Player2Name;
                m_GameSettings.IsSinglePlayer = isSinglePlayer;
                this.textBoxPlayer2Name.Text = k_DefaultSinglePlayerText;
            }
        }

        private void textBox_Click(object sender, EventArgs e)
        {
            switch((sender as TextBox).AccessibleName)
            {
                case ("Player1"):
                    m_GameSettings.Player1Name = (sender as TextBox).Text;
                    break;
                case ("Player2"):
                    m_GameSettings.Player1Name = (sender as TextBox).Text;
                    break;
                default:
                    break;
            }
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            if (isValidPlayersNames())
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private bool isValidPlayersNames()
        {
            bool isValid = true;
            if (textBoxPlayer1Name.Text == string.Empty)
            {
                isValid = false;
                MessageBox.Show(
                    "Please enter a name for the first player", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                m_GameSettings.Player1Name = textBoxPlayer1Name.Text;
            }

            if (!m_GameSettings.IsSinglePlayer)
            {
                if (textBoxPlayer2Name.Text == string.Empty)
                {
                    isValid = false;
                    MessageBox.Show(
                        "Please enter a name for the second player", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    m_GameSettings.Player2Name = textBoxPlayer2Name.Text;
                }
            }

            return isValid;
        }
        #endregion Methods
    }
}
