using System.Drawing;
using System.Windows.Forms;

namespace Ex05.WindowsUI
{
    internal class RadioButtonSetting : RadioButton
    {
        internal RadioButtonSetting(string i_Text, int i_Tag, bool i_Checked, int i_X, int i_Y)
        {
            this.Text = i_Text;
            this.Tag = i_Tag;
            this.Checked = i_Checked;
            this.AutoSize = true;
            this.Location = new Point(i_X, i_Y);
        }
    }
}
