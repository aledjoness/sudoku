using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Sudoku
{
    public class GameControl : Form
    {
        private string selectedToolkitButtonName;
        private List<List<Button>> playerViewGrid;
        private List<int> numberInstances;
        private int playerPenalty;
        private Stopwatch stopWatch;

        // Creates9 3x3 grids of buttons ("above" labels)
        public void createButtonGrid(PlayWindow window, int squareSize)
        {
            List<List<Button>> buttonGrid = new List<List<Button>>();
            int nextX = 0, nextY = 0;
            for (int i = 0; i < 9; i++)
            {
                nextX = 0;
                if (i % 3 == 0)
                {
                    nextY += 6;
                }

                List<Button> col = new List<Button>();
                for (int j = 0; j < 9; j++)
                {
                    Button newButton = new Button();
                    newButton.Size = new Size(squareSize, squareSize);
                    newButton.Text = "";
                    if (j % 3 == 0) // Start a new block horizontally
                    {
                        nextX += 6;
                    }
                    newButton.Location = new Point(nextX, nextY);
                    string concatName = (i) + "" + (j);
                    newButton.Name = concatName;
                    newButton.Enabled = true;
                    newButton.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                    newButton.MouseLeave += new EventHandler(window.button_leaveHighlight);
                    newButton.MouseClick += new MouseEventHandler(window.button_click);
                    window.Controls.Add(newButton);
                    col.Add(newButton);
                    nextX += squareSize;
                }
                nextY += squareSize;
                buttonGrid.Add(col);
            }
            playerViewGrid = buttonGrid;
        }

        public void updatePlayerViewGrid(Button buttonToUpdate, string newText)
        {
            buttonToUpdate.Text = newText;
            buttonToUpdate.Enabled = false;
        }

        public string getPlayerViewGridValue(Button button)
        {
            char[] buttonNameAsChars = button.Name.ToCharArray();
            int row = int.Parse(button.Name[0].ToString());
            int col = int.Parse(button.Name[1].ToString());
            return playerViewGrid[row][col].Text;
        }

        public void setHighlightNumber(PlayWindow window, int number)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    string concatName = (i) + "" + (j);
                    Control[] controls = window.Controls.Find(concatName, true);
                    if (controls.Length == 1)
                    {
                        Button b = controls[0] as Button;
                        b.MouseEnter += new EventHandler((sender, e) => window.button_enterHighlight(sender, e, number));                        
                    }
                }
        }

        public void setSizeOfWindow(PlayWindow window, int squareSize)
        {
            int formula = squareSize + (9 * squareSize);
            window.Width = formula + 185;
            window.Height = formula + 20;
        }

        public void createToolbox(PlayWindow window, int squareSize)
        {
            Label newLabel = new Label();
            newLabel.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            newLabel.Location = new Point(397, 95);
            newLabel.Text = "CHOOSE A NUMBER";
            newLabel.Size = new Size(200,25);
            window.Controls.Add(newLabel);
            int nextX = 0, nextY = 120;
            int incrementor = 1;
            for (int i = 0; i < 3; i++)
            {
                nextX = 400;
                for (int j = 0; j < 3; j++)
                {
                    Button newButton = new Button();
                    newButton.Size = new Size(squareSize, squareSize);
                    newButton.Location = new Point(nextX, nextY);
                    newButton.Text = incrementor.ToString();
                    newButton.Name = "toolkit" + incrementor;
                    if (newButton.Name == "toolkit1")
                        newButton.Enabled = false;
                    newButton.Click += new EventHandler(window.toolkit_Click);
                    newButton.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                    window.Controls.Add(newButton);
                    nextX += squareSize;
                    incrementor++;
                }
                nextY += squareSize;
            }

            // Also create bit below toolbox (penalty + time)
            Label timeLabel = new Label();
            timeLabel.Font = new Font("Microsoft Sans Serif", 8);
            timeLabel.Location = new Point(420, 323);
            timeLabel.Text = "Time Elapsed: 0:00";
            timeLabel.Name = "timeElapsedLabel";
            stopWatch = new Stopwatch();
            window.Controls.Add(timeLabel);

            Label penLabel = new Label();
            penLabel.Font = new Font("Microsoft Sans Serif", 8);
            penLabel.Location = new Point(420, 345);
            penLabel.Text = "Penalty: 0";
            penLabel.Name = "penaltyLabel";
            playerPenalty = 0;
            window.Controls.Add(penLabel);

            // Set up number of instances to an empty 9-depth list
            numberInstances = new List<int>();
            for (int i = 0; i < 10; i++)
                numberInstances.Add(0);
        }

        public void showStartingNumbers(int difficulty, SudokuGrid sg)
        {
            // Size of nums dependant on difficulty
            List<string> nums = new List<string>();

            int size = 0;
            Random rnd = new Random();

            switch (difficulty)
            {
                case 0:
                    {
                        size = rnd.Next(36, 40);
                        // Below is for testing only
                        //size = rnd.Next(80, 81);
                        break;
                    }
                case 1:
                    {
                        size = rnd.Next(27, 31);
                        break;
                    }
                case 2:
                    {
                        size = rnd.Next(20, 24);
                        break;
                    }
            }

            // Add our random squares to show by using random row/cols
            for (int i = 0; i < size; i++)
            {
                bool placementOk = false;
                while (!placementOk)
                {
                    int row = rnd.Next(0, 9);
                    int col = rnd.Next(0, 9);
                    string res = row + "" + col;

                    if (!nums.Contains(res))
                    {
                        nums.Add(res);
                        placementOk = true;
                    }
                }                
            }
            // Now we get the values at each location, and change playerViewGrid to show these numbers
            for (int i = 0; i < size; i++)
            {                
                // Val is the value which will be shown
                string val = sg.getValueOfButtonInGrid(nums[i]).ToString();
                updateNumberInstance(val);

                string currentNums = nums[i];
                string row = currentNums[0].ToString();
                string col = currentNums[1].ToString();
                int colAsInt = int.Parse(col);
                int rowAsInt = int.Parse(row);
                playerViewGrid[rowAsInt][colAsInt].Enabled = false;
                playerViewGrid[rowAsInt][colAsInt].Text = val;
            }
        }

        public void updateNumberInstance(string number)
        {
            int ourNumber = int.Parse(number);
            numberInstances[ourNumber]++;
        }

        public bool numberInstanceStillInPlay(string number)
        {
            bool result = true;
            int ourNumber = int.Parse(number);
            if (numberInstances[ourNumber] == 9)
                result = false;
            return result;
        }

        delegate void WindowActionCallBack(PlayWindow window);

        public int getPlayerPenalty()
        {
            return playerPenalty;
        }

        public void incrementPenaltyLabel(PlayWindow window)
        {
            Control[] controls = window.Controls.Find("penaltyLabel", true);            
            if (controls.Length == 1)
            {
                Label lab = controls[0] as Label;
                if (lab.InvokeRequired)
                {
                    WindowActionCallBack d = new WindowActionCallBack(incrementPenaltyLabel);
                    Invoke(d, new object[] { window }); 
                }
                else
                {
                    playerPenalty++;
                    lab.Text = "Penalty: " + playerPenalty;
                }                
            }
        }

        public bool checkForPlayerWin()
        {
            bool result = true;
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    if (playerViewGrid[i][j].Enabled == true)
                    {
                        result = false;
                        break;
                    }
                }
            return result;
        }

        public string getSelectedToolkitButtonName()
        {
            return selectedToolkitButtonName;
        }

        public void setSelectedToolkitButtonName(string nameToSet)
        {
            selectedToolkitButtonName = nameToSet;
        }

        delegate void HighlightCallBack(string number, bool highlight);

        public void highlightOrDehighlightDisabledButtons(string number, bool highlight)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    if (playerViewGrid[i][j].Text == number)
                    {
                        if (playerViewGrid[i][j].InvokeRequired)
                        {
                            HighlightCallBack d = new HighlightCallBack(highlightOrDehighlightDisabledButtons);
                            Invoke(d, new object[] { number, highlight });
                        }
                        else
                        {
                            if (highlight)
                            {
                                playerViewGrid[i][j].BackColor = Color.Pink;
                            }
                            else
                            {
                                playerViewGrid[i][j].BackColor = default(Color);
                                playerViewGrid[i][j].UseVisualStyleBackColor = true;
                            }
                        }
                    }
                }
        }
    }
}
