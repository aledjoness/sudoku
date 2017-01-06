using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Sudoku
{
    public partial class Leaderboard : Form
    {
        private WelcomeWindow welcomeWindow;
        private NameRequest nameRequest;
        private int numOfLeaderboardEntries;
        private string path;
        private string name;

        public Leaderboard()
        {
            InitializeComponent();
            path = ConfigurationManager.AppSettings["leaderboardPath"];
            name = ConfigurationManager.AppSettings["leaderboardName"];
            setLeaderboardToBlank();

            // Calculate number of leaderboard entries
            numOfLeaderboardEntries = getNumOfLeaderboardEntries();

            // Then load in our current top 10 high scores
            if (numOfLeaderboardEntries != 0)
                loadLeaderboard();
        }

        public Leaderboard(WelcomeWindow ww)
        {
            InitializeComponent();
            welcomeWindow = ww;

            // Load in our leaderboard path + name
            path = ConfigurationManager.AppSettings["leaderboardPath"];
            name = ConfigurationManager.AppSettings["leaderboardName"];

            // Initially set everything to a default
            setLeaderboardToBlank();

            // Calculate number of leaderboard entries
            numOfLeaderboardEntries = getNumOfLeaderboardEntries();

            // Then load in our current top 10 high scores
            if (numOfLeaderboardEntries != 0)
                loadLeaderboard();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            welcomeWindow.Show();
            Close();
        }

        private void setLeaderboardToBlank()
        {
            // Names
            string nameToFind = "";
            for (int i = 1; i <= 10; i++)
            {
                nameToFind = "name" + i + "Label";
                Control[] controls = Controls.Find(nameToFind, true);
                if (controls.Length == 1)
                {
                    Label l = controls[0] as Label;
                    l.Text = "-";
                }
            }

            // Date
            string dateToFind = "";
            for (int i = 1; i <= 10; i++)
            {
                dateToFind = "date" + i + "Label";
                Control[] controls = Controls.Find(dateToFind, true);
                if (controls.Length == 1)
                {
                    Label l = controls[0] as Label;
                    l.Text = "-";
                }
            }

            // Difficulty
            string difficultyToFind = "";
            for (int i = 1; i <= 10; i++)
            {
                difficultyToFind = "diff" + i + "Label";
                Control[] controls = Controls.Find(difficultyToFind, true);
                if (controls.Length == 1)
                {
                    Label l = controls[0] as Label;
                    l.Text = "-";
                }
            }

            // Time
            string timeToFind = "";
            for (int i = 1; i <= 10; i++)
            {
                timeToFind = "time" + i + "Label";
                Control[] controls = Controls.Find(timeToFind, true);
                if (controls.Length == 1)
                {
                    Label l = controls[0] as Label;
                    l.Text = "-";
                }
            }

            // Score
            string scoreToFind = "";
            for (int i = 1; i <= 10; i++)
            {
                scoreToFind = "score" + i + "Label";
                Control[] controls = Controls.Find(scoreToFind, true);
                if (controls.Length == 1)
                {
                    Label l = controls[0] as Label;
                    l.Text = "-";
                }
            }
        }

        private int getNumOfLeaderboardEntries()
        {
            int numOfLines = 0;
            int lineCheck = 0;
            string fileAddress = path + name;
            int BufferSize = 128;
            bool mustFixInconsistentState = false;
            string[] splitString;
            using (var fileStream = File.OpenRead(fileAddress))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    lineCheck++;
                    // Determine whether we have an entry or just a filler
                    splitString = line.Split(',');

                    if (splitString.Length == 5)
                    {
                        if (splitString[0] == "" || splitString[1] == "" || splitString[2] == "" || splitString[3] == "" || splitString[4] == "")
                        {
                            mustFixInconsistentState = true;
                        }
                        // One way of determining filler is if splitString[1] = -1
                        try
                        {
                            int.Parse(splitString[1]);
                        }
                        catch (Exception)
                        {
                            // Means it is an actual date, due to '/' being included
                            numOfLines++;
                        }
                    }
                    else
                    {
                        // Must fix inconsistent state
                        mustFixInconsistentState = true;
                    }
                }
                if (lineCheck != 10 || mustFixInconsistentState)
                {
                    // If we don't get 10 lines (fillers or entries) then we have a corrupted file
                    MessageBox.Show("The leaderboard file has become inconsistent. Attempting to fix.",
                                    "Sudoku", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fixInconsistentState(numOfLines);
                    MessageBox.Show("Leaderboard is now in a consistent state. Some saved data may have been lost.",
                                    "Sudoku", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return numOfLines;
            }
        }

        private void fixInconsistentState(int numOfConsistentLines)
        {
            // Save our consistent lines (if any), then add filler lines
            string fileAddress = path + name;
            if (numOfConsistentLines > 0)
            {
                List<List<string>> consistentLeaderboard = new List<List<string>>();

                int BufferSize = 128;
                int currentLine = 0;
                using (var fileStream = File.OpenRead(fileAddress))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (currentLine < numOfConsistentLines)
                        {
                            currentLine++;
                            List<string> consistentLine = new List<string>(line.Split(','));
                            consistentLeaderboard.Add(consistentLine);
                        }
                    }
                }
                File.WriteAllText(fileAddress, String.Empty);

                string res = "";
                for (int i = 0; i < numOfConsistentLines; i++)
                {
                    res = consistentLeaderboard[i][0] + "," + consistentLeaderboard[i][1] + ","
                        + consistentLeaderboard[i][2] + "," + consistentLeaderboard[i][3] + ","
                        + consistentLeaderboard[i][4];
                    File.AppendAllText(fileAddress, res + Environment.NewLine);
                }
                // Now add fillers
                for (int i = numOfConsistentLines; i < 10; i++)
                {
                    File.AppendAllText(fileAddress, "-1,-1,-1,-1,-1" + Environment.NewLine);
                }
            }
            else
            {
                // If we had no consistent lines to begin with, then clear file and add all fillers
                File.WriteAllText(fileAddress, String.Empty);

                for (int i = 0; i < 10; i++)
                {
                    File.AppendAllText(fileAddress, "-1,-1,-1,-1,-1" + Environment.NewLine);
                }
            }
        }

        private void loadLeaderboard()
        {
            string fileAddress = path + name;
            int BufferSize = 128;
            using (var fileStream = File.OpenRead(fileAddress))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                string line;
                int pos = 0;
                string labelToFind = "";
                string[] splitString;
                while ((line = streamReader.ReadLine()) != null)
                {
                    pos++;
                    // 0 = name, 1 = date, 2 = difficulty, 3 = time, 4 = score
                    splitString = line.Split(',');

                    // Determine if entry or filler, display accordingly
                    if (splitString[1] == "-1")
                    {
                        // name
                        labelToFind = "name" + pos + "Label";
                        Control[] controls = Controls.Find(labelToFind, true);
                        Label l = controls[0] as Label;
                        l.Text = "-";

                        // date
                        labelToFind = "date" + pos + "Label";
                        controls = Controls.Find(labelToFind, true);
                        l = controls[0] as Label;
                        l.Text = "-";

                        // difficulty
                        labelToFind = "diff" + pos + "Label";
                        controls = Controls.Find(labelToFind, true);
                        l = controls[0] as Label;
                        l.Text = "-";

                        // time
                        labelToFind = "time" + pos + "Label";
                        controls = Controls.Find(labelToFind, true);
                        l = controls[0] as Label;
                        l.Text = "-";

                        // score
                        labelToFind = "score" + pos + "Label";
                        controls = Controls.Find(labelToFind, true);
                        l = controls[0] as Label;
                        l.Text = "-";
                    }
                    else
                    {
                        // name
                        labelToFind = "name" + pos + "Label";
                        Control[] controls = Controls.Find(labelToFind, true);
                        Label l = controls[0] as Label;
                        l.Text = splitString[0];

                        // date
                        labelToFind = "date" + pos + "Label";
                        controls = Controls.Find(labelToFind, true);
                        l = controls[0] as Label;
                        l.Text = splitString[1];

                        // difficulty
                        labelToFind = "diff" + pos + "Label";
                        controls = Controls.Find(labelToFind, true);
                        l = controls[0] as Label;
                        l.Text = splitString[2];

                        // time
                        labelToFind = "time" + pos + "Label";
                        controls = Controls.Find(labelToFind, true);
                        l = controls[0] as Label;
                        l.Text = splitString[3];

                        // score
                        labelToFind = "score" + pos + "Label";
                        controls = Controls.Find(labelToFind, true);
                        l = controls[0] as Label;
                        l.Text = splitString[4];
                    }                    
                }
            }
        }

        public bool scoreMadeItOntoLeaderboard(int newScore)
        {
            bool result = false;

            string fileAddress = path + name;
            int BufferSize = 128;
            string line;
            int scoreInLeaderboard = 0;
            string[] splitString;

            if (numOfLeaderboardEntries < 1)
            {
                result = true;
            }
            else
            {

                using (var fileStream = File.OpenRead(fileAddress))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        splitString = line.Split(',');
                        scoreInLeaderboard = int.Parse(splitString[4]);

                        if (splitString[4] == "-")
                        {
                            result = true;
                            break;
                        }

                        if (newScore > scoreInLeaderboard)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        // Returns true if score made it onto the leaderboard, false otherwise
        public void addToLeaderboard(string submittedName, string date, int difficulty, int timeInSecs, int newScore, NameRequest nameReq)
        {
            nameRequest = nameReq;
            string fileAddress = path + name;
            int BufferSize = 128;
            string line;
            int pos = 0;
            int scoreInLeaderboard = 0;
            string[] splitString;

            using (var fileStream = File.OpenRead(fileAddress))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    pos++;
                    splitString = line.Split(',');
                    // if splitString[4] > score ...
                    scoreInLeaderboard = int.Parse(splitString[4]);

                    if (newScore > scoreInLeaderboard)
                    {
                        break;
                    }
                }
            }
            propagateDownLeaderboard(submittedName, date, difficulty, timeInSecs, newScore, pos);
        }

        private void propagateDownLeaderboard(string submittedName, string date, int difficulty, int timeInSecs, int newScore, int newScorePos)
        {
            List<List<string>> currentLeaderboard = new List<List<string>>();
            // Firstly, preserve current leaderboard in string[]
            string fileAddress = path + name;

            int BufferSize = 128;
            string line;
            int pos = 0;
            using (var fileStream = File.OpenRead(fileAddress))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    List<string> currentLine = new List<string>(line.Split(','));
                    currentLeaderboard.Add(currentLine);
                }
            }
            // At this point we have preserved the leaderboard in our string arrays, so now we can wipe it clean and re-write
            DialogResult result = DialogResult.Retry;
            while (result == DialogResult.Retry)
            {
                try
                {
                    File.WriteAllText(fileAddress, string.Empty);
                    result = DialogResult.Cancel;
                }
                catch (IOException e)
                {
                    //MessageBox.Show("source of error: "+e.Source);
                    //MessageBox.Show(e.InnerException.ToString());
                    //MessageBox.Show(e.Message);
                    //MessageBox.Show(e.StackTrace);
                    result = MessageBox.Show("Unable to add name to leaderboard because the file " +
                        "is in use by another program, please close it and try " +
                        "again", "Sudoku", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (result == DialogResult.Cancel)
                    {
                        MessageBox.Show("Adding to leaderboard aborted", "Sudoku");
                    }
                    //result = DialogResult.Cancel;
                }
            }
            // Remove the last entry in the leaderboard
            currentLeaderboard.RemoveAt(9);

            // Make new leaderboard
            List<List<string>> newLeaderboard = new List<List<string>>();

            // Copy over entries which remain higher than our new entry (if any)
            for (int i = 0; i < newScorePos - 1; i++)
            {
                newLeaderboard.Add(currentLeaderboard[i]);
                pos++;
            }

            // At the point where we now need to add our new entry
            List<string> newEntry = new List<string>();
            newEntry.Add(submittedName);
            newEntry.Add(date);
            switch (difficulty)
            {
                case 0:
                    newEntry.Add("Easy");
                    break;
                case 1:
                    newEntry.Add("Medium");
                    break;
                case 2:
                    newEntry.Add("Hard");
                    break;
            }            

            // Do some manipulation to put time in readable mins:secs format
            int mins = 0;
            int secs = timeInSecs;
            if (timeInSecs > 59)
            {
                while (secs > 59)
                {
                    secs -= 60;
                    mins++;
                }
            }
            string minsAsString = mins.ToString();
            string secsAsString = secs.ToString();
            if (minsAsString.Length == 1)
            {
                minsAsString = "0" + minsAsString;
            }
            if (secsAsString.Length == 1)
            {
                secsAsString = "0" + secsAsString;
            }
            string timeResult = minsAsString + ":" + secsAsString;
            newEntry.Add(timeResult);
            newEntry.Add(newScore.ToString());
            newLeaderboard.Add(newEntry);
            pos++;

            // Now add all scores below our new entry (if any)
            for (int i = pos; i < 10; i++)
            {
                newLeaderboard.Add(currentLeaderboard[i - 1]);
                pos++;
            }

            // At this point we have our new leaderboard, now need to write to file
            writeNewLeaderboardToFile(newLeaderboard);

            //Aborted:
            //    WelcomeWindow ww = new WelcomeWindow();
            //    ww.FormClosed += (s, args) => Close();
            //    ww.Show();
            //    Hide();

        }

        private void writeNewLeaderboardToFile(List<List<string>> newLeaderboard)
        {
            string res = "";
            for (int i = 0; i < 10; i++)
            {
                res = newLeaderboard[i][0] + "," + newLeaderboard[i][1] + "," + newLeaderboard[i][2] + "," + newLeaderboard[i][3] + ","
                    + newLeaderboard[i][4];
                //MessageBox.Show("About to append: " + res);
                File.AppendAllText(path + name, res + Environment.NewLine);
            }
            WelcomeWindow ww = new WelcomeWindow();
            welcomeWindow = ww;
            ww.Show();
            //File.WriteAllText(path + name, res + Environment.NewLine);
        }

        //private void startWelcomeWindow(object state)
        //{
        //    WelcomeWindow ww = new WelcomeWindow();
        //    ww.Show();
        //}
    }

}
