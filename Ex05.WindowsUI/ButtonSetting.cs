using System.Drawing;
using System.Windows.Forms;

namespace Ex05.WindowsUI
{
    internal class ButtonSetting : Button
    {
        internal ButtonSetting(string i_Text, int i_X, int i_Y)
        {
            this.Text = i_Text;
            this.Location = new Point(i_X, i_Y);
        }
    }
}
