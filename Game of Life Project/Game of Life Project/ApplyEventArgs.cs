using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_of_Life_Project
{
    public class ApplyEventArgs : EventArgs
    {
        int milliseconds;
        int xArrSize;
        int yArrSize;

        public int Milliseconds { get { return milliseconds; } set { milliseconds = value; } }
        public int XArrSize { get { return xArrSize; } set { xArrSize = value; } }
        public int YArrSize { get { return yArrSize; } set { yArrSize = value; } }


        public ApplyEventArgs(int milliseconds, int xArrSize, int yArrSize)
        {
            this.milliseconds = milliseconds;
            this.xArrSize = xArrSize;
            this.yArrSize = yArrSize;
        }

        public delegate void ApplyEventHandler(object sender, ApplyEventArgs e);

    }
}
