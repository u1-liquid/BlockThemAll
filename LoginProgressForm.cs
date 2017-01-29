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
    public partial class LoginProgressForm : Form
    {
        private string authorizationUrlString = "https://api.twitter.com/oauth/authorize?oauth_token=" + MainForm.Instance.twitter.OAuth.User.Token;

        public LoginProgressForm()
        {
            InitializeComponent();
        }

        private void manualLoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(authorizationUrlString);
            MessageBox.Show(this,
                "Manual login URL has been copied into your clipboard!" + Environment.NewLine +
                " Open your web browser and paste URL into address bar." + Environment.NewLine + "Fill the PIN code in next form.",
                "Manual login process", MessageBoxButtons.OK, MessageBoxIcon.Information);
            TextInputForm tiform = new TextInputForm();
            if (DialogResult.OK == tiform.ShowDialog(this))
            {
                MainForm.Instance.twitter.Authenticate(tiform.UserText.Trim());
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void LoginProgressForm_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(authorizationUrlString);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElementCollection collection = webBrowser1.Document?.GetElementsByTagName("code");
            if (collection == null || collection.Count != 1) return;

            HtmlElement pincode = collection[0];
            MainForm.Instance.twitter.Authenticate(pincode.InnerText.Trim());
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
