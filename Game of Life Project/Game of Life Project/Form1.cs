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
    public partial class Form1 : Form
    {
        static int xArr = 7;
        static int yArr = 7;
        // The universe array
        bool[,] universe = new bool[xArr, yArr];
        bool[,] scratchPad = new bool[xArr, yArr];

        //Used to determine which CountNeighbor method to call
        bool isToroidal = true;


        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();
            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }


        // Calculate the next generation of cells
        private void NextGeneration()
        {
            int livingNeighbors;

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (isToroidal == true)
                        livingNeighbors = CountNeighborsTorroidal(x, y);
                    else
                        livingNeighbors = CountNeighborsFinite(x, y);

                    if (universe[x,y] == true && livingNeighbors < 2)
                    {
                        scratchPad[x, y] = false;
                    }
                    else if (universe[x,y] == true && livingNeighbors > 3)
                    {
                        scratchPad[x, y] = false;
                    }
                    else if (universe[x,y] == true && (livingNeighbors == 2 || livingNeighbors == 3))
                    {
                        scratchPad[x, y] = true;
                    }
                    else if (universe[x,y] == false && livingNeighbors == 3)
                    {
                        scratchPad[x,y] = true;
                    }
                }
            }

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            //Swap the arrays
            SwapArrays();

            //Repaint
            graphicsPanel1.Invalidate();
        }
        
        private void SwapArrays()
        {
            bool[,] temp = new bool[xArr, yArr];
            universe = scratchPad;
            scratchPad = temp;
        }

        //Counts the neighbor of each cell
        //For this method, the borders of the screen represent the end of the simulated world
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <=1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    if (yCheck >= yLen)
                    {
                        continue;
                    }
                    if (universe[xCheck, yCheck] == true)
                        count++;
                }
            }

            
            return count;
        }

        //This method also counts the neighbor of each cell
        //For this method, the borders of the screen wrap around to simulate an infinite world
        private int CountNeighborsTorroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }
                    
                    if (universe[xCheck, yCheck] == true)
                        count++;
                }
            }


            return count;
        }



        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Make into floats
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float) universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float) universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (float y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (float x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    //RectangleF for floats
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[(int)x, (int)y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Floats
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / (float)cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / (float)cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        /* Tool Strip functions - done
         * 
         * Play
         * Pause
         * Next
         */

        //Play
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        //Pause
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        //Next - Increments by one generation
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        /* File Menu Functions - in progress
         * 
         * New - done
         * Open - todo
         * Import - todo
         * Save - todo
         * Exit -done
         */

        //Resets the grid back to new
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }

            }
            timer.Enabled = false;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        //Used to open a cell file - unfinished
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //used to import a cell file - unfinished
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Saves current grid as a cell file - unfinished
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Exits the application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /* View Menu Functions - in progress
         * 
         * HUD - todo
         * NeighborCount - todo
         * Grid - todo
         * 
         * Toroidal - done
         * Finite - done
         */

        //Toggles HUD - unfinished
        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Toggles NeighborCount numbers - unfinished
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Toggles Grid lines - unfinished
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Sets grid to toroidal
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isToroidal = true;
        }

        //Sets grid to finite
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isToroidal = false;
        }

        /* Settings Menu Functions - in progress
         * 
         * Options - todo
         * 
         * 
         * 
         * 
         */

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();

            form2.ShowDialog();
        }
    }
}
