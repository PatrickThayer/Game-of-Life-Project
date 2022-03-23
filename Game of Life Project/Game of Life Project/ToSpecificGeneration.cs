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
    public partial class ToSpecificGeneration : Form
    {
        public ToSpecificGeneration(int generations)
        {
            InitializeComponent();

            numericUpDown1.Value = generations + 1;
        }

        public int NumericUpDown_1 { get { return (int) numericUpDown1.Value; } set { numericUpDown1.Value = value; } }
    }
}
