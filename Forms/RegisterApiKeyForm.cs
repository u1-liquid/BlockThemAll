using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BlockThemAll.Forms
{
    public partial class RegisterApiKeyForm : Form
    {
        public string TargetSection = "Authenticate";

        public static readonly Tuple<string, string, string>[] knownkeys = {
            new Tuple<string, string, string>(
                "Twitter for Android",
                "3nVuSoBZnx6U4vzUxf5w",
                "Bcs59EFbbsdF6Sl9Ng71smgStWEGwXXKSjYvPVt7qys"
            ), new Tuple<string, string, string>(
                "Twitter for iPhone",
                "IQKbtAYlXLripLGPWd0HUA",
                "GgDYlkSvaPxGxC4X8liwpUoqKwwr3lCADbz8A7ADU"
            ), new Tuple<string, string, string>(
                "Twitter for iPad",
                "CjulERsDeqhhjSme66ECg",
                "IQWdVyqFxghAtURHGeGiWAsmCAGmdW3WmbEx6Hck"
            ), new Tuple<string, string, string>(
                "Twitter for Mac",
                "3rJOl1ODzm9yZy63FACdg",
                "5jPoQ5kQvMJFDYRNE8bQ4rHuds4xJqhvgNJM4awaE8"
            ), new Tuple<string, string, string>(
                "Twitter for Windows Phone",
                "yN3DUNVO0Me63IAQdhTfCA",
                "c768oTKdzAjIYCmpSNIdZbGaG0t6rOhSFQP0S5uC79g"
            ), new Tuple<string, string, string>(
                "Twitter for Google TV",
                "iAtYJ4HpUVfIUoNnif1DA",
                "172fOpzuZoYzNYaU3mMYvE8m8MEyLbztOdbrUolU"
            ), new Tuple<string, string, string>(
                "Tweetbot for iOS",
                "8AeR93em84Pyum5i1QGA",
                "ugCImRuw376Y9t9apIq6bgWGNbb1ymBrx2K5NK0ZI"
            ), new Tuple<string, string, string>(
                "HootSuite",
                "w1Gybt9LP9zG46mS1X3UAw",
                "hRIK4RWjAO4pokQCvmNCynRAY8Jc8edV1kcV2go6g"
            ), new Tuple<string, string, string>(
                "Instagram",
                "7YBPrscvh0RIThrWYVeGg",
                "sMO1vDyJ9A0xfOE6RyWNjhTUS1sNqsa7Ae14gOZnw"
            )
        };

        private readonly List<Tuple<string, string, string>> presets = new List<Tuple<string, string, string>>(knownkeys);

        public RegisterApiKeyForm()
        {
            InitializeComponent();

            foreach (KeyValuePair<string, Dictionary<string, object>> settings in MainForm.Instance.settings)
            {
                object key, secret;
                if (settings.Value.TryGetValue("ConsumerKey", out key) && settings.Value.TryGetValue("ConsumerSecret", out secret))
                    presets.RemoveAll(x => x.Item2.Equals((string)key) && x.Item3.Equals((string)secret));
            }
        }

        private void RegisterApiKey_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (Tuple<string, string, string> item in presets)
                comboBox1.Items.Add(item.Item1);

            if (string.IsNullOrEmpty(TargetSection)) return;

            comboBox1.Text = TargetSection;
            textBox1.Text = (string)MainForm.Instance.settings.GetValue(TargetSection, "ConsumerKey");
            textBox1.Text = (string)MainForm.Instance.settings.GetValue(TargetSection, "ConsumerSecret");

            if (TargetSection.Equals("Authenticate") && textBox1.Text.Length == 0)
            {
                comboBox1.SelectedIndex = 0;
                comboBox1_SelectionChangeCommitted(null, null);
            }

            comboBox1.Select();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Tuple<string, string, string> v = presets.Find(x => x.Item1.Equals(comboBox1.SelectedItem));
            textBox1.Text = v.Item2;
            textBox2.Text = v.Item3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            if (string.IsNullOrEmpty(TargetSection)) TargetSection = comboBox1.Text;
            MainForm.Instance.settings.SetValue(TargetSection, "ConsumerKey", textBox1.Text);
            MainForm.Instance.settings.SetValue(TargetSection, "ConsumerSecret", textBox2.Text);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
