﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Sudoku
{
    public partial class PlayWindow : Form
    {
        private GameControl gameControl;
        private SudokuGrid sudokuGrid;
        private object ourLock = new object();
        private bool startTimer;
        private bool gameAlive;
        private int gridDifficulty;
        private Stopwatch stopWatch;

        public PlayWindow(GameControl gc, SudokuGrid sg, int difficulty) // difficulty: 0 - easy, 1 - medium, 2 - hard
        {
            InitializeComponent();
            gameControl = gc;
            gameControl.createButtonGrid(this, 40);
            gameControl.setSizeOfWindow(this, 40);
            gameControl.createToolbox(this, 50);
            gameControl.setSelectedToolkitButtonName("toolkit1");
            gameControl.setHighlightNumber(this, 1);
            gameControl.showStartingNumbers(difficulty, sg);
            gridDifficulty = difficulty;
            sudokuGrid = sg;
            initialToolkitCheck();
        }

        // See if we have already placed 9 of one number during startup
        private void initialToolkitCheck()
        {
            string toolkitButtonToLookFor = "";
            for (int i = 1; i < 10; i++)
            {
                toolkitButtonToLookFor = "toolkit" + i;

                if (!gameControl.numberInstanceStillInPlay(i.ToString()))
                {
                    // If not still in play, mark it so
                    Control[] controls = Controls.Find(toolkitButtonToLookFor, true);
                    Button b = controls[0] as Button;
                    b.Enabled = false;
                }                
            }
        }

        public void toolkit_Click(object sender, EventArgs e)
        {
            // Set previous selected button to enabled
            Control[] controls = Controls.Find(gameControl.getSelectedToolkitButtonName(), true);
            if (controls.Length == 1)
            {
                Button b = controls[0] as Button;
                if (gameControl.numberInstanceStillInPlay(b.Name[7].ToString()))
                {
                    b.Enabled = true;
                }
                gameControl.highlightOrDehighlightDisabledButtons(b.Name[7].ToString(), false);
            }
            // Disable button that was clicked
            Button b2 = sender as Button;
            b2.Enabled = false;
            gameControl.setSelectedToolkitButtonName(b2.Name);
            gameControl.setHighlightNumber(this, int.Parse(b2.Name[7].ToString()));
            gameControl.highlightOrDehighlightDisabledButtons(b2.Name[7].ToString(), true);
        }

        public void button_enterHighlight(object sender, EventArgs e, int num)
        {
            Button b = sender as Button;
            if (b.Enabled == true)
            {
                b.Text = num.ToString();
                b.ForeColor = Color.SteelBlue;
            }
        }

        public void button_leaveHighlight(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b.Enabled == true)
                b.Text = "";
        }

        public void button_click(object sender, EventArgs e)
        {
            // If first button clicked, start the clock
            if (startTimer == false)
            {
                startStopWatch();
                Thread t = new Thread(doWork);
                t.IsBackground = true;
                t.Name = "TIME_THREAD";
                Debug.WriteLine("[PlayWindow] Starting clock");
                t.Start();
                startTimer = true;
            }
            
            // Have to work out whether or not it was a correct click
            Button b = sender as Button;

            string res1 = sudokuGrid.getValueOfButtonInGrid(b.Name).ToString();
            string res2 = gameControl.getSelectedToolkitButtonName()[7].ToString();

            // Determining whether or not user has guessed correctly
            if (sudokuGrid.getValueOfButtonInGrid(b.Name).ToString() == gameControl.getSelectedToolkitButtonName()[7].ToString())
            {
                b.Text = sudokuGrid.getValueOfButtonInGrid(b.Name).ToString();
                b.Enabled = false;
                gameControl.updateNumberInstance(b.Text);

                // Also highlight new number
                gameControl.highlightOrDehighlightDisabledButtons(b.Text, true);

                // Perform a win-check
                if (gameControl.checkForPlayerWin())
                {
                    // Deal with player winning
                    gameControl.highlightOrDehighlightDisabledButtons(b.Text, false);
                    gameAlive = false;
                    TimeSpan ts = stopWatch.Elapsed;
                    string secs, mins;
                    if (ts.Seconds.ToString().Length == 1)
                    {
                        secs = "0" + ts.Seconds;
                    }
                    else
                    {
                        secs = ts.Seconds.ToString();
                    }
                    if (ts.Minutes.ToString().Length == 1)
                    {
                        mins = "0" + ts.Minutes;
                    }
                    else
                    {
                        mins = ts.Minutes.ToString();
                    }
                    string finalResult = mins + ":" + secs;
                    int totalSecs = (ts.Minutes * 60) + ts.Seconds;
                    string penaltyEnding = "";
                    int penaltyNum = gameControl.getPlayerPenalty();
                    int finalScore = getFinalScore(ts.Minutes, ts.Seconds, penaltyNum, gridDifficulty);
                    penaltyEnding = (gameControl.getPlayerPenalty() == 1) ? "penalty" : "penalties";
                    MessageBox.Show("Puzzle completed! Done in " + finalResult + " with " + penaltyNum + " " + penaltyEnding +".\n" +
                        "Final score: " + finalScore);

                    // Add score to leaderboard
                    Leaderboard lb = new Leaderboard();
                    if (lb.scoreMadeItOntoLeaderboard(finalScore))
                    {
                        MessageBox.Show("Congratulations you made it onto the leaderboard!");
                        NameRequest nr = new NameRequest(lb, gridDifficulty, totalSecs, finalScore);
                        nr.FormClosed += (s, args) => Close();
                        Hide();
                        nr.Show();
                    }

                    // Show leaderboard
                }
            }
            // Show the user they got it wrong - give penalty
            else
            {
                gameControl.incrementPenaltyLabel(this);
            }
        }

        private void doWork(object state)
        {
            while (gameAlive)
            {                
                Thread.Sleep(1000);
                updateTimeLabel();
            }
        }

        delegate void WindowActionCallBack();

        public void startStopWatch()
        {
            stopWatch = Stopwatch.StartNew();
        }

        private int getFinalScore(int mins, int secs, int numOfPenalties, int difficulty)
        {
            int result = 0;

            // First convert everything to seconds
            int totalSeconds = (mins * 60) + secs;

            // Determine multiplier based on difficulty
            double difficultyMultiplier = 0;
            double penaltyMultiplier = 0;
            double initialScore = 200;
            switch (difficulty)
            {
                case 0: // easy
                {
                    difficultyMultiplier = 1;
                    penaltyMultiplier = 1;
                    break;
                }
                case 1: // medium
                {
                    difficultyMultiplier = 2;
                    penaltyMultiplier = 0.75;
                    break;
                }
                case 2: // hard
                {
                    difficultyMultiplier = 3;
                    penaltyMultiplier = 0.5;
                    break;
                }
            }
            // Deduct incorrect placement penalty
            initialScore -= (numOfPenalties * penaltyMultiplier);

            // Deduct time penalty
            initialScore -= (totalSeconds * 0.2);

            // Add difficulty bonus
            initialScore += (initialScore * difficultyMultiplier);

            result = (int)initialScore;

            return result;
        }

        public void updateTimeLabel()
        {
            // Do some string manipulation here (format: 00:00)
            TimeSpan ts = stopWatch.Elapsed;
            string result = "";
            if (Math.Floor(Math.Log10(ts.Seconds) + 1) == 1)
                result = ts.Minutes + ":0" + ts.Seconds;
            else
                result = ts.Minutes + ":" + ts.Seconds;
            Control[] controls = Controls.Find("timeElapsedLabel", true);
            if (controls.Length == 1)
            {
                Label lab = controls[0] as Label;
                if (lab.InvokeRequired)
                {
                    WindowActionCallBack d = new WindowActionCallBack(updateTimeLabel);
                    Invoke(d, new object[] { });
                }
                else
                {
                    lab.Text = "Time Elapsed: " + result;
                }
            }
        }

        private void PlayWindow_Load(object sender, EventArgs e)
        {
            startTimer = false;
            gameAlive = true;
        }
    }
}
