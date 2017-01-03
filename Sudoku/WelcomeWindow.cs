using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace Sudoku
{
    public partial class WelcomeWindow : Form
    {
        private GameControl gameControl;
        private Stopwatch stopWatch;
        private long stopWatchMillis;
        private string path = @"C:\Users\a6f3mzz\Documents\Visual Studio 2010\Projects\Sudoku\Sudoku\data\";
        private string file = "solved_grids.txt";

        public WelcomeWindow()
        {
            InitializeComponent();

            // Create our GameControl here
            GameControl gc = new GameControl();
            gameControl = gc;
        }

        private int numOfSolvedGrids()
        {
            string fileName = path + "" + file;
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(fileName))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                int numOfLines = 0;
                while ((line = streamReader.ReadLine()) != null)
                {
                    numOfLines++;
                }
                return numOfLines;
            }
        }

        private List<int> getPuzzle(int position)
        {
            List<int> result = new List<int>();
            char[] delimitChars = { ',' };
            string fileName = path + "" + file;
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(fileName))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                int numOfLines = 1;
                // Iterate over lines until we reach our position in text file
                while ((line = streamReader.ReadLine()) != null && numOfLines != position)
                {
                    numOfLines++;
                }
                string[] splitLine = line.Split(delimitChars);
                for (int i = 0; i < splitLine.Length; i++)
                {
                    result.Add(Int32.Parse(splitLine[i]));
                }
                return result;
            }
        }

        private void easyButton_Click(object sender, EventArgs e)
        {
            // First pick a random entry in solved_grids.txt to pass as args[1]
            Random rnd = new Random();
            int puzzlePick = rnd.Next(1, numOfSolvedGrids() + 1);
            SudokuGrid sudokuGrid = new SudokuGrid(getPuzzle(puzzlePick));

            PlayWindow pw = new PlayWindow(gameControl, sudokuGrid, 0);
            pw.FormClosed += (s, args) => this.Close();
            this.Hide();
            pw.Show();
        }

        private void generatePuzzleButton_Click(object sender, EventArgs e)
        {
            progressBar.Maximum = 100;
            progressBar.Step = 3;
            progressBar.Value = 0;

            //BackgroundWorker bw = new BackgroundWorker();
            //bw.RunWorkerAsync();
            //ThreadPool.QueueUserWorkItem(doWork);

            Thread t = new Thread(doWork);
            t.IsBackground = true;
            t.Name = "WORK_THREAD";
            t.Start();
        }

        private void timeCheck()
        {
            while (true)
            {
                long currentMillis = stopWatch.ElapsedMilliseconds;
                long difference = currentMillis - stopWatchMillis;
                long millisToSecons = difference / 1000;
            }
        }

        delegate void SetTextCallback(string text);

        private void setTotalLettersGuessedText(string text)
        {
            string labelToFind = "cpuWordLabel";
            Control[] controls = this.Controls.Find(labelToFind, true);
            if (controls.Length == 1)
            {
                Label lab = controls[0] as Label;
                if (lab.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(setTotalLettersGuessedText);
                    Invoke(d, new Object[] { text });
                }
                else
                {
                    lab.Text = text;
                }
            }
        }

        delegate void PerformProgress();

        public void updateProgressBar()
        {
            if (progressBar.InvokeRequired)
            {
                PerformProgress d = new PerformProgress(updateProgressBar);
                Invoke(d, new Object[] { });
            }
            else
            {
                progressBar.PerformStep();
            }
        }

        public void completeProgressBar()
        {
            if (progressBar.InvokeRequired)
            {
                PerformProgress d = new PerformProgress(completeProgressBar);
                Invoke(d, new Object[] { });
            }
            else
            {
                progressBar.Value = 100;
            }
        }

        delegate void PerformProgressStatus(string text);

        public void updateProgressLabel(string text)
        {
            if (statusLabel.InvokeRequired)
            {
                PerformProgressStatus d = new PerformProgressStatus(updateProgressLabel);
                Invoke(d, new Object[] { text });
            }
            else
            {
                statusLabel.Text = "Status: " + text;
            }
        }

        private void doWork(object state)
        {
            SudokuGrid sg = new SudokuGrid(path, file);
            updateProgressLabel("Working...");
            sg.setUpPlacementOrders();
            updateProgressBar();
            for (int i = 0; i < 9; i++)
                sg.doInitialPlacementFill(i);

            updateProgressBar();
            sg.populate(this);
        }
    }
}
