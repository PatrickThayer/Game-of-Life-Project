using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life_Project
{
    public partial class ChooseSeedForm : Form
    {
        public ChooseSeedForm(int seed)
        {
            InitializeComponent();

            numericUpDown1.Value = seed;
        }

        public int NumericUpDown_1 { get { return (int) numericUpDown1.Value; } set { numericUpDown1.Value = value; } }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            numericUpDown1.Value = rand.Next((int)numericUpDown1.Minimum, (int)numericUpDown1.Maximum);
        }
    }
}
