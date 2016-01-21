namespace Poker
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class AddChips : Form
    {
        const string message = "Are you sure?";
        const string title = "Quit";

        public AddChips()
        {
            var fontFamily = new FontFamily("Arial");
            
            this.InitializeComponent();
            this.ControlBox = false;
            this.label1.BorderStyle = BorderStyle.FixedSingle;
        }
        public int AddedChips { get; set; }

        private void Button1_Click(object sender, EventArgs e)
        {
            int parsedValue;
            if (int.Parse(this.textBox1.Text) > 100000000)
            {
                MessageBox.Show("The maximium chips you can add is 100000000");
                return;
            }

            if (!int.TryParse(this.textBox1.Text, out parsedValue))
            {
                MessageBox.Show("This is a number only field");
                return;
            }

            else if (int.TryParse(this.textBox1.Text, out parsedValue) &&
                     int.Parse(this.textBox1.Text) <= 100000000)
            {
                this.AddedChips = int.Parse(this.textBox1.Text);
                this.Close();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            switch (result)
            {
                case DialogResult.No:
                    break;
                case DialogResult.Yes:
                    Application.Exit();
                    break;
            }
        }
    }
}