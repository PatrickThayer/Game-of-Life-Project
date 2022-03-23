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
        // Used to adjust the width (xArr) and height (yArr) of the array
        static int xArr;
        static int yArr;

        // Used to adjust the speed at which NextGeneration is called
        static int milliseconds;

        // Counts the number of living cells
        static int livingCells;

        // The random seed of the universe 
        static int seed;

        // Used to run the application to a specific generation
        int counter;

        // The universe array
        static bool[,] universe;

        // The scratchPad array
        static bool[,] scratchPad;

        // Used to determine which CountNeighbor method to call
        bool isToroidal = true;

        // Used to toggle the grid display on and off
        bool displayGrid = true;

        // Used to toggle the neighbor count display on and off
        bool displayNeighborCount = true;

        // Used to determine if application is running to a specific generation
        bool CounterOn = false;


        // Drawing colors
        Color gridColor = Color.Black;
        Color gridColor_x10 = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        static Timer timer = new Timer();

        // Generation count
        int generations = 0;

        // Class Constructor
        public Form1()
        {
            InitializeComponent();

            // Update application to display persistent settings
            graphicsPanel1.BackColor = Properties.Settings.Default.BackColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor_x10 = Properties.Settings.Default.GridColor_x10;

            seed = Properties.Settings.Default.RandSeed;
            milliseconds = Properties.Settings.Default.Milliseconds;
            xArr = Properties.Settings.Default.ArrLen_X;
            yArr = Properties.Settings.Default.ArrLen_Y;
            universe = new bool[xArr, yArr];
            scratchPad = new bool[xArr, yArr];

            // Update the status strip with value of persistent settings
            toolStripStatusLabelInterval.Text = "Interval: " + milliseconds;
            toolStripStatusLabelSeed.Text = "Seed: " + seed;


            // Setup the timer
            timer.Interval = milliseconds; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Updates Settings properties on close so that settings persist between sessions
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.BackColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.GridColor_x10 = gridColor_x10;

            Properties.Settings.Default.RandSeed = seed;
            Properties.Settings.Default.Milliseconds = milliseconds;
            Properties.Settings.Default.ArrLen_X = xArr;
            Properties.Settings.Default.ArrLen_Y = yArr;

            Properties.Settings.Default.Save();
        }


        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // Tracks the number of generations for the application's run-to operation, and deactivates it when the requested
            // number of generations is reached
            if (counter <= 1 && CounterOn)
            {
                CounterOn = false;
                timer.Enabled = false;
            }
            if (counter > 0 && CounterOn)
            {
                counter--;
            }

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
            toolStripStatusLabelGenerations.Text = "Generations: " + generations.ToString();

            //Swap the arrays
            SwapArrays();



            //Repaint
            graphicsPanel1.Invalidate();
        }
        
        // Swap arrays to iterate through generations accurately
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

            // reset livingCells so that dead cells do not persist into consecutive paints
            livingCells = 0;

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
                        livingCells++;
                        toolStripStatusLabelAlive.Text = "Alive: " + livingCells;
                    }

                    // if displayGrid is true, paint the grid to the screen
                    if (displayGrid)
                    {
                        // Used to write the number of neighbors into each cell
                        Font font = new Font("Arial", 8f);
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;


                        // Determine which number to write to the cell
                        int neighbors;
                        
                        if (isToroidal)
                            neighbors = CountNeighborsTorroidal((int)x, (int)y);
                        else
                            neighbors = CountNeighborsFinite((int)x, (int)y);

                        // Outline the cell with a pen
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                        // if displayNeighborCount is true, write number of living neighbors in each cell
                        if (displayNeighborCount)
                        {
                            // Write in the neighbor count
                            if (neighbors != 0)
                            {
                                if (neighbors == 3 || (neighbors == 2 && universe[(int)x, (int)y] == true))
                                    e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                                else
                                    e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                            }
                        }

                        // Outline 10x10 cell with a thicker pen
                        if (x % 10 == 0 && y % 10 == 0)
                            e.Graphics.DrawRectangle(gridPen_x10, cellRect.X, cellRect.Y, cellRect.Width * 10, cellRect.Height * 10);
                    }
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

        /* Tool Strip functions - finished
         * 
         * New - done
         * Open - done
         * Save - done
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

        //Used to open a cell file
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }

        //Saves current grid as a cell file
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);
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
         * Open - todo
         * Import - Not required for implementation
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
            toolStripStatusLabelGenerations.Text = "Generations: " + generations.ToString();
            toolStripStatusLabelAlive.Text = "Alive: 0";
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
         * NeighborCount - done
         * Grid - done
         * 
         * Toroidal - done
         * Finite - done
         */

        //Toggles HUD - unfinished
        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Toggles NeighborCount numbers
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!displayNeighborCount)
                displayNeighborCount = true;
            else
                displayNeighborCount = false;

            graphicsPanel1.Invalidate();
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
            graphicsPanel1.Invalidate();
        }

        //Sets grid to finite
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isToroidal = false;
            graphicsPanel1.Invalidate();
        }


        /* Run Functions - finished
         * 
         * Play - done
         * Pause - done
         * Next - done
         * To - done
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
            ToSpecificGeneration tsg = new ToSpecificGeneration(generations);

            if(DialogResult.OK == tsg.ShowDialog())
            {
                int generationToRunTo = tsg.NumericUpDown_1;
                RunToGeneration(generationToRunTo);
            }

        }

        // Triggers the run-to operation and sets the number of generations to be executed
        private void RunToGeneration(int generationToRunTo)
        {
            if (generationToRunTo > generations)
            {
                CounterOn = true;
                counter = generationToRunTo - generations;
                timer.Enabled = true;
            }
        }


        /* Randomize Functions - finished
         * 
         * From Seed - done
         * From Current Seed - done
         * From Time - done
         * 
         */

        // Choose the seed by which to randomize the universe 
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChooseSeedForm csf = new ChooseSeedForm(seed);

            if (DialogResult.OK == csf.ShowDialog())
            {
                seed = csf.NumericUpDown_1;
                toolStripStatusLabelSeed.Text = "Seed: " + seed;

                Random rand = new Random(seed);

                for (int x = 0; x < universe.GetLength(1); x++)
                {
                    for (int y = 0; y < universe.GetLength(0); y++)
                    {
                        int num = rand.Next(0, 3);

                        if (num == 0)
                            universe[x, y] = true;
                        else
                            universe[x, y] = false;
                    }
                }

                generations = 0;
                toolStripStatusLabelGenerations.Text = "Generations: " + generations;

                graphicsPanel1.Invalidate();
            }

        }

        // Randomize the universe based on the current seed
        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random rand = new Random(seed);
            for (int x = 0; x < universe.GetLength(1); x++)
            {
                for (int y = 0; y < universe.GetLength(0); y++)
                {
                    int num = rand.Next(0, 3);

                    if (num == 0)
                        universe[x, y] = true;
                    else
                        universe[x, y] = false;
                }
            }

            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations: " + generations;
            graphicsPanel1.Invalidate();
        }

        // Randomize the universe based on the system clock
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            for (int x = 0; x < universe.GetLength(1); x++)
            {
                for (int y = 0; y < universe.GetLength(0); y++)
                {
                    int num = rand.Next(0, 3);

                    if (num == 0)
                        universe[x, y] = true;
                    else
                        universe[x, y] = false;
                }
            }

            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations: " + generations;
            graphicsPanel1.Invalidate();
        }

        /* Settings Menu Functions - in progress
         * 
         * Background Color - done
         * Cell Color - done
         * Grid Color - done
         * Grid x10 Color - todo
         * 
         * Options - done
         * 
         * Reset - done
         * Reload - done
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

        //Set a separate color for every 10x10 section of the grid - unfinished
        private void gridX10ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = gridColor_x10;

            if (DialogResult.OK == dlg.ShowDialog())
                gridColor_x10 = dlg.Color;

            graphicsPanel1.Invalidate();
        }

        // Opens modal dialog box to allow user to adjust the height and width of grid as well as the interval between generations
        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(milliseconds, xArr, yArr);

            if (DialogResult.OK == form2.ShowDialog())
            {
                if (xArr != form2.NumericUpDown_2 || yArr != form2.NumericUpDown_3)
                {
                    xArr = form2.NumericUpDown_2;
                    yArr = form2.NumericUpDown_3;

                    universe = new bool[xArr, yArr];
                    scratchPad = new bool[xArr, yArr];

                    graphicsPanel1.Invalidate();

                    pauseToolStripMenuItem_Click(sender, e);
                }

                milliseconds = form2.NumericUpDown_1;
                timer.Interval = milliseconds;

                // Update status strip intervals
                toolStripStatusLabelInterval.Text = "Interval: " + milliseconds.ToString();

            }
        }

        // Returns the application to its default settings
        // Affects Color of grid, grid_x10, cells, and background, and also affects grid size and milliseconds
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reset the settings values
            Properties.Settings.Default.Reset();

            // Update the variables with the new values
            graphicsPanel1.BackColor = Properties.Settings.Default.BackColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor_x10 = Properties.Settings.Default.GridColor_x10;

            seed = Properties.Settings.Default.RandSeed;
            milliseconds = Properties.Settings.Default.Milliseconds;
            xArr = Properties.Settings.Default.ArrLen_X;
            yArr = Properties.Settings.Default.ArrLen_Y;
            universe = new bool[xArr, yArr];
            scratchPad = new bool[xArr, yArr];

            timer.Stop();

            generations = 0;
            livingCells = 0;

            // Update the status strip
            toolStripStatusLabelGenerations.Text = "Generations: " + 0;
            toolStripStatusLabelInterval.Text = "Interval: " + milliseconds;
            toolStripStatusLabelAlive.Text = "Alive: " + 0;
            toolStripStatusLabelSeed.Text = "Seed: " + seed;

            graphicsPanel1.Invalidate();
        }

        // Revert application to last saved settings
        // As with reset, affects Color of grid, grid_x10, cells, and background, and also affects grid size and milliseconds
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reset the settings values
            Properties.Settings.Default.Reload();

            // Update the variables with the new values
            graphicsPanel1.BackColor = Properties.Settings.Default.BackColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor_x10 = Properties.Settings.Default.GridColor_x10;

            seed = Properties.Settings.Default.RandSeed;
            milliseconds = Properties.Settings.Default.Milliseconds;
            xArr = Properties.Settings.Default.ArrLen_X;
            yArr = Properties.Settings.Default.ArrLen_Y;
            universe = new bool[xArr, yArr];
            scratchPad = new bool[xArr, yArr];

            timer.Stop();

            generations = 0;
            livingCells = 0;

            // Update the status strip
            toolStripStatusLabelGenerations.Text = "Generations: " + generations;
            toolStripStatusLabelInterval.Text = "Interval: " + milliseconds;
            toolStripStatusLabelAlive.Text = "Alive: " + livingCells;
            toolStripStatusLabelSeed.Text = "Seed: " + seed;

            graphicsPanel1.Invalidate();
        }

        /* Context Menu Functions - finished
         * 
         *  --- Color ---
         *  Set Background Color - done
         *  Set Cell Color - done
         *  Set Grid Color - done
         *  Set Girdx10 Color - done
         * 
         *  -- View ---
         *  HUD - done
         *  Neighbor Count - done
         *  Grid - done
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

        // Toggles HUD
        private void hUDToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            customizeToolStripMenuItem_Click(sender, e);
        }

        // Toggles Neighbor Count Numbers
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            optionsToolStripMenuItem_Click(sender, e);
        }

        // Toggles grid lines
        private void gridToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            gridToolStripMenuItem_Click(sender, e);
        }

    }
}
