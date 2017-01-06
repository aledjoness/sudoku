using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class NameRequest : Form
    {
        private Leaderboard leaderboard;
        private int _difficulty;
        private int _time;
        private int _score;

        public NameRequest(Leaderboard lb, int difficulty, int time, int score)
        {
            InitializeComponent();
            leaderboard = lb;
            _difficulty = difficulty;
            _time = time;
            _score = score;
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            if (nameBox.TextLength < 1)
            {
                MessageBox.Show("Name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string submittedName = nameBox.Text;
                int day = DateTime.Now.Day;
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                string date = day + "/" + month + "/" + year;
                leaderboard.FormClosed += (s, args) => Close();
                Hide();
                leaderboard.addToLeaderboard(submittedName, date, _difficulty, _time, _score, this);
            }
        }
    }
}
