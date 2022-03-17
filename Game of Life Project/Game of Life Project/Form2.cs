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
{/*
  *
  * Dialog Box for "Options" in Settings Menu
  *
  */
    public partial class Form2 : Form
    {
        public delegate void ApplyEventHandler(object sender, ApplyEventArgs e);

        public event ApplyEventHandler Apply;

        int milliseconds;
        int xArrSize;
        int yArrSize;

        public Form2(int milliseconds, int xArr, int yArr)
        {
            InitializeComponent();

            numericUpDown1.Value = milliseconds;
            numericUpDown2.Value = xArr;
            numericUpDown3.Value = yArr;

            Milliseconds = milliseconds;
            XArrSize = xArr;
            YArrSize = yArr;
        }

        public int Milliseconds { get { return milliseconds; } set { milliseconds = value; } }
        public int XArrSize { get { return xArrSize; } set { xArrSize = value; } }
        public int YArrSize { get { return yArrSize; } set { yArrSize = value; } }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (Apply != null)
                Apply(this, new ApplyEventArgs(this.Milliseconds, this.XArrSize, this.YArrSize));
        }

        public int NumericUpDown_1 { get { return (int)numericUpDown1.Value; } set { numericUpDown1.Value = value; } }
        public int NumericUpDown_2 { get { return (int)numericUpDown2.Value; } set { numericUpDown2.Value = value; } }
        public int NumericUpDown_3 { get { return (int)numericUpDown3.Value; } set { numericUpDown3.Value = value; } }
    }
}
