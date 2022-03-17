using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Game_of_Life_Project.Form2;

namespace Game_of_Life_Project
{
    public partial class Form1 : Form
    {
        static int xArr = 25;
        static int yArr = 25;
        static int milliseconds = 100;
        // The universe array
        static bool[,] universe = new bool[xArr, yArr];
        static bool[,] scratchPad = new bool[xArr, yArr];

        //Used to determine which CountNeighbor method to call
        bool isToroidal = true;
        bool displayGrid = true;


        // Drawing colors
        Color gridColor = Color.Black;
        Color gridColor_x10 = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        static Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();
            // Setup the timer
            timer.Interval = milliseconds; // milliseconds
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

            // A Pen for drawing thicker grid lines every 10 cells (color, width)
            Pen gridPen_x10 = new Pen(gridColor_x10, 3);

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
                    if (displayGrid)
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    //if ( x % 10 == 0 && y % 10 == 0)
                    //    e.Graphics.DrawRectangle(gridPen_x10, x, y, cellRect.Width * 10, cellRect.Height * 10);
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

        /* Tool Strip functions
         * 
         * New - done
         * Open - todo
         * Save - todo
         * 
         * Play - done
         * Pause - done
         * Next - done
         */

        //Resets the grid back to new
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            newToolStripMenuItem_Click(sender, e);
        }

        //Used to open a cell file - unfinished
        private void openToolStripButton_Click(object sender, EventArgs e)
        {

        }

        //Saves current grid as a cell file - unfinished
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {

        }

        // Plays the simulation
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            startToolStripMenuItem_Click(sender, e);
        }

        // Pauses the simulation
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            pauseToolStripMenuItem_Click(sender, e);
        }

        // Advances the simulation to the next generation
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            nextToolStripMenuItem_Click(sender, e);
        }

        /* File Menu Functions - in progress
         * 
         * New - done
         * Open - done
         * Import - todo
         * Save - done
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

        //Used to open a cell file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openToolStripButton_Click(sender, e);
        }

        //used to import a cell file - unfinished
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Saves current grid as a cell file
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToolStripButton_Click(sender, e);
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
         * Grid - done
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

        //Toggles Grid lines
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (displayGrid)
                displayGrid = false;
            else
                displayGrid = true;

            graphicsPanel1.Invalidate();
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


        /* Run Functions - in progress
         * 
         * Play - done
         * Pause - done
         * Next - done
         * To - todo
         * 
         */

        // Plays the simulation
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        // Pauses the simulation
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        // Advances the simulation to the next generation
        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }


        // Advances the simulation to a specific generation
        private void toToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        /* Randomize Functions - in progress
         * 
         * From Seed - todo
         * From Current Seed - todo
         * From Time - todo
         * 
         */

        /* Settings Menu Functions - in progress
         * 
         * Background Color - done
         * Cell Color - done
         * Grid Color - done
         * Grid x10 Color - todo
         * 
         * Options - done
         * 
         * Reset - todo
         * Reload - todo
         * 
         */

        // Set Background Color
        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = graphicsPanel1.BackColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
            }

            graphicsPanel1.Invalidate();

        }

        // Set cell color
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = cellColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
            }

            graphicsPanel1.Invalidate();
        }

        // Set grid color
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = gridColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;
            }

            graphicsPanel1.Invalidate();
        }

        //Set a separate color for every 10x10 section of the grid
        private void gridX10ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // Opens modal dialog box to allow user to adjust the height and width of grid as well as the interval between generations
        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(milliseconds, xArr, yArr);

            if (DialogResult.OK == form2.ShowDialog())
            {

                milliseconds = form2.NumericUpDown_1;
                xArr = form2.NumericUpDown_2;
                yArr = form2.NumericUpDown_3;

                universe = new bool[xArr, yArr];
                scratchPad = new bool[xArr, yArr];
                timer.Interval = milliseconds;
                graphicsPanel1.Invalidate();

            }
            graphicsPanel1.Invalidate();

        }

        // Returns the application to its default settings
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // Revert application to last saved settings
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /* Context Menu Functions - finished
         * 
         *  Set Background Color - done
         *  Set Cell Color - done
         *  Set Grid Color - done
         *  Set Girdx10 Color - done
         * 
         */

        // Set Background Color
        private void backColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            backColorToolStripMenuItem_Click(sender, e);
        }

        // Set cell color
        private void cellColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColorToolStripMenuItem_Click(sender, e);
        }

        // Set grid color
        private void gridColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            gridColorToolStripMenuItem_Click(sender, e);
        }

        //Set a separate color for every 10x10 section of the grid
        private void gridX10ColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            gridX10ColorToolStripMenuItem_Click(sender, e);
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fromCurrentTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
