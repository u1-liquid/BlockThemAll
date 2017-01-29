using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockThemAll
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
