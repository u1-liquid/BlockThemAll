using System;
using System.Windows.Forms;

namespace BlockThemAll.Forms
{
    public partial class TextInputForm : Form
    {
        public string UserText = string.Empty;

        public TextInputForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserText = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
