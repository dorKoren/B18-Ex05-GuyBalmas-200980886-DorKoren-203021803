using System.Drawing;
using System.Windows.Forms;

namespace Ex05.WindowsUI
{
    internal class TextBoxSetting : TextBox
    {
        internal TextBoxSetting(
            string i_Text, string i_AccessibleName,
            bool i_Enabled, int i_X, int i_Y)
        {
            this.Text = i_Text;
            this.AccessibleName = i_AccessibleName;
            this.Enabled = i_Enabled;
            this.AutoSize = true;
            this.Location = new Point(i_X, i_Y);
        }
    }
}
