using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace Sudoku
{
    public class SudokuGrid
    {
        private List<List<int>> grid;
        private List<List<int>> playerGuessGrid;
        private List<List<int>> placementOrderTopLeft;
        private List<List<int>> placementOrderTopMiddle;
        private List<List<int>> placementOrderTopRight;
        private List<List<int>> placementOrderMiddleLeft;
        private List<List<int>> placementOrderMiddleMiddle;
        private List<List<int>> placementOrderMiddleRight;
        private List<List<int>> placementOrderBottomLeft;
        private List<List<int>> placementOrderBottomMiddle;
        private List<List<int>> placementOrderBottomRight;
        private string path;
        private string file;
        private Stopwatch stopWatch;

        /* CONSTRUCTOR FOR GENERATING A GRID */
        // Generates and then stores the grid, performing validity checks
        public SudokuGrid(string pathName, string fileName)
        {
            path = pathName;
            file = fileName;
            stopWatch = Stopwatch.StartNew();
            grid = createEmptyGrid();
        }

        /* CONSTRUCTOR FOR LOADING A GRID */
        public SudokuGrid(List<int> gridList)
        {
            List<List<int>> result = new List<List<int>>();
            List<int> col1 = new List<int>();
            List<int> col2 = new List<int>();
            List<int> col3 = new List<int>();
            List<int> col4 = new List<int>();
            List<int> col5 = new List<int>();
            List<int> col6 = new List<int>();
            List<int> col7 = new List<int>();
            List<int> col8 = new List<int>();
            List<int> col9 = new List<int>();

            for (int i = 0; i < 73; i+=9)
            {
                col1.Add(gridList[i]);
            }

            for (int i = 1; i < 74; i += 9)
            {
                col2.Add(gridList[i]);
            }

            for (int i = 2; i < 75; i += 9)
            {
                col3.Add(gridList[i]);
            }

            for (int i = 3; i < 76; i += 9)
            {
                col4.Add(gridList[i]);
            }

            for (int i = 4; i < 77; i += 9)
            {
                col5.Add(gridList[i]);
            }

            for (int i = 5; i < 78; i += 9)
            {
                col6.Add(gridList[i]);
            }

            for (int i = 6; i < 79; i += 9)
            {
                col7.Add(gridList[i]);
            }

            for (int i = 7; i < 80; i += 9)
            {
                col8.Add(gridList[i]);
            }

            for (int i = 8; i < 81; i += 9)
            {
                col9.Add(gridList[i]);
            }

            result.Add(col1);
            result.Add(col2);
            result.Add(col3);
            result.Add(col4);
            result.Add(col5);
            result.Add(col6);
            result.Add(col7);
            result.Add(col8);
            result.Add(col9);
            grid = result;
            playerGuessGrid = createEmptyGrid();
            //printGrid();
        }

        public int getValueOfButtonInGrid(string nameOfButton)
        {
            char[] nameSplit = nameOfButton.ToCharArray();
            string row = nameSplit[0].ToString();
            string col = nameSplit[1].ToString();
            return grid[int.Parse(row)][int.Parse(col)];
        }

        // Creates a 9x9 grid of 0's
        private List<List<int>> createEmptyGrid()
        {
            List<List<int>> blankGrid = new List<List<int>>();
            for (int i = 0; i < 9; i++)
            {
                List<int> column = new List<int>();
                for (int j = 0; j < 9; j++)
                {
                    column.Add(0);
                }
                blankGrid.Add(column);
            }
            return blankGrid;
        }

        private List<List<int>> blank3x3Box()
        {
            List<List<int>> miniBox = new List<List<int>>();
            for (int iBox = 0; iBox < 3; iBox++)
            {
                List<int> column = new List<int>();
                for (int jBox = 0; jBox < 3; jBox++)
                {
                    column.Add(0);
                }
                miniBox.Add(column);
            }
            return miniBox;
        }

        public void setUpPlacementOrders()
        {
            // First create 9 blank placement orders of -1, 9 times
            placementOrderTopLeft = createBlankPlacementOrder();
            placementOrderTopMiddle = createBlankPlacementOrder();
            placementOrderTopRight = createBlankPlacementOrder();
            placementOrderMiddleLeft = createBlankPlacementOrder();
            placementOrderMiddleMiddle = createBlankPlacementOrder();
            placementOrderMiddleRight = createBlankPlacementOrder();
            placementOrderBottomLeft = createBlankPlacementOrder();
            placementOrderBottomMiddle = createBlankPlacementOrder();
            placementOrderBottomRight = createBlankPlacementOrder();
        }

        public void doInitialPlacementFill(int boxPos)
        {
            int sleepTime = 150;
            Thread oThread = new Thread(() => placementFill(boxPos));
            oThread.Start();
            while (!oThread.IsAlive) ;
            Thread.Sleep(sleepTime);
            oThread.Abort();
        }

        public void doSinglePlacementFill(int boxPos, int posInBox)
        {
            int sleepTime = 50;
            Thread oThread = new Thread(() => singlePlacementFill(boxPos, posInBox));
            oThread.Start();
            while (!oThread.IsAlive) ;
            Thread.Sleep(sleepTime);
            oThread.Abort();
        }

        public void singlePlacementFill(int boxPos, int posInBox)
        {
            List<int> list1 = new List<int>();
            for (int i = 0; i < 9; i++)
                list1.Add(i);
            Random rnd = new Random();
            int count = 9;
            while (count > 1)
            {
                count--;
                int k = rnd.Next(count + 1);
                int value = list1[k];
                list1[k] = list1[count];
                list1[count] = value;
            }

            switch (boxPos)
            {
                case 0:
                    placementOrderTopLeft[posInBox] = list1;
                    break;
                case 1:
                    placementOrderTopMiddle[posInBox] = list1;
                    break;
                case 2:
                    placementOrderTopRight[posInBox] = list1;
                    break;
                case 3:
                    placementOrderMiddleLeft[posInBox] = list1;
                    break;
                case 4:
                    placementOrderMiddleMiddle[posInBox] = list1;
                    break;
                case 5:
                    placementOrderMiddleRight[posInBox] = list1;
                    break;
                case 6:
                    placementOrderBottomLeft[posInBox] = list1;
                    break;
                case 7:
                    placementOrderBottomMiddle[posInBox] = list1;
                    break;
                case 8:
                    placementOrderBottomRight[posInBox] = list1;
                    break;
            }
        }

        public void placementFill(int boxPos)
        {
            List<int> list1 = new List<int>();
            for (int i = 0; i < 9; i++)
                list1.Add(i);
            Random rnd = new Random();
            int count = 9;
            while (count > 1)
            {
                count--;
                int k = rnd.Next(count + 1);
                int value = list1[k];
                list1[k] = list1[count];
                list1[count] = value;
            }

            List<int> list2 = new List<int>();
            for (int i = 0; i < 9; i++)
                list2.Add(i);
            count = 9;
            while (count > 1)
            {
                count--;
                int k = rnd.Next(count + 1);
                int value = list2[k];
                list2[k] = list2[count];
                list2[count] = value;
            }

            List<int> list3 = new List<int>();
            for (int i = 0; i < 9; i++)
                list3.Add(i);
            count = 9;
            while (count > 1)
            {
                count--;
                int k = rnd.Next(count + 1);
                int value = list3[k];
                list3[k] = list3[count];
                list3[count] = value;
            }

            List<int> list4 = new List<int>();
            for (int i = 0; i < 9; i++)
                list4.Add(i);
            count = 9;
            while (count > 1)
            {
                count--;
                int k = rnd.Next(count + 1);
                int value = list4[k];
                list4[k] = list4[count];
                list4[count] = value;
            }

            List<int> list5 = new List<int>();
            for (int i = 0; i < 9; i++)
                list5.Add(i);
            count = 9;
            while (count > 1)
            {
                count--;
                int k = rnd.Next(count + 1);
                int value = list5[k];
                list5[k] = list5[count];
                list5[count] = value;
            }

            List<int> list6 = new List<int>();
            for (int i = 0; i < 9; i++)
                list6.Add(i);
            count = 9;
            while (count > 1)
            {
                count--;
                int k = rnd.Next(count + 1);
                int value = list6[k];
                list6[k] = list6[count];
                list6[count] = value;
            }

            List<int> list7 = new List<int>();
            for (int i = 0; i < 9; i++)
                list7.Add(i);
            count = 9;
            while (count > 1)
            {
                count--;
                int k = rnd.Next(count + 1);
                int value = list7[k];
                list7[k] = list7[count];
                list7[count] = value;
            }

            List<int> list8 = new List<int>();
            for (int i = 0; i < 9; i++)
                list8.Add(i);
            count = 9;
            while (count > 1)
            {
                count--;
                int k = rnd.Next(count + 1);
                int value = list8[k];
                list8[k] = list8[count];
                list8[count] = value;
            }

            List<int> list9 = new List<int>();
            for (int i = 0; i < 9; i++)
                list9.Add(i);
            count = 9;
            while (count > 1)
            {
                count--;
                int k = rnd.Next(count + 1);
                int value = list9[k];
                list9[k] = list9[count];
                list9[count] = value;
            }

            switch (boxPos)
            {
                case 0:
                    placementOrderTopLeft[0] = list1;
                    placementOrderTopLeft[1] = list2;
                    placementOrderTopLeft[2] = list3;
                    placementOrderTopLeft[3] = list4;
                    placementOrderTopLeft[4] = list5;
                    placementOrderTopLeft[5] = list6;
                    placementOrderTopLeft[6] = list7;
                    placementOrderTopLeft[7] = list8;
                    placementOrderTopLeft[8] = list9;
                    break;
                case 1:
                    placementOrderTopMiddle[0] = list1;
                    placementOrderTopMiddle[1] = list2;
                    placementOrderTopMiddle[2] = list3;
                    placementOrderTopMiddle[3] = list4;
                    placementOrderTopMiddle[4] = list5;
                    placementOrderTopMiddle[5] = list6;
                    placementOrderTopMiddle[6] = list7;
                    placementOrderTopMiddle[7] = list8;
                    placementOrderTopMiddle[8] = list9;
                    break;
                case 2:
                    placementOrderTopRight[0] = list1;
                    placementOrderTopRight[1] = list2;
                    placementOrderTopRight[2] = list3;
                    placementOrderTopRight[3] = list4;
                    placementOrderTopRight[4] = list5;
                    placementOrderTopRight[5] = list6;
                    placementOrderTopRight[6] = list7;
                    placementOrderTopRight[7] = list8;
                    placementOrderTopRight[8] = list9;
                    break;
                case 3:
                    placementOrderMiddleLeft[0] = list1;
                    placementOrderMiddleLeft[1] = list2;
                    placementOrderMiddleLeft[2] = list3;
                    placementOrderMiddleLeft[3] = list4;
                    placementOrderMiddleLeft[4] = list5;
                    placementOrderMiddleLeft[5] = list6;
                    placementOrderMiddleLeft[6] = list7;
                    placementOrderMiddleLeft[7] = list8;
                    placementOrderMiddleLeft[8] = list9;
                    break;
                case 4:
                    placementOrderMiddleMiddle[0] = list1;
                    placementOrderMiddleMiddle[1] = list2;
                    placementOrderMiddleMiddle[2] = list3;
                    placementOrderMiddleMiddle[3] = list4;
                    placementOrderMiddleMiddle[4] = list5;
                    placementOrderMiddleMiddle[5] = list6;
                    placementOrderMiddleMiddle[6] = list7;
                    placementOrderMiddleMiddle[7] = list8;
                    placementOrderMiddleMiddle[8] = list9;
                    break;
                case 5:
                    placementOrderMiddleRight[0] = list1;
                    placementOrderMiddleRight[1] = list2;
                    placementOrderMiddleRight[2] = list3;
                    placementOrderMiddleRight[3] = list4;
                    placementOrderMiddleRight[4] = list5;
                    placementOrderMiddleRight[5] = list6;
                    placementOrderMiddleRight[6] = list7;
                    placementOrderMiddleRight[7] = list8;
                    placementOrderMiddleRight[8] = list9;
                    break;
                case 6:
                    placementOrderBottomLeft[0] = list1;
                    placementOrderBottomLeft[1] = list2;
                    placementOrderBottomLeft[2] = list3;
                    placementOrderBottomLeft[3] = list4;
                    placementOrderBottomLeft[4] = list5;
                    placementOrderBottomLeft[5] = list6;
                    placementOrderBottomLeft[6] = list7;
                    placementOrderBottomLeft[7] = list8;
                    placementOrderBottomLeft[8] = list9;
                    break;
                case 7:
                    placementOrderBottomMiddle[0] = list1;
                    placementOrderBottomMiddle[1] = list2;
                    placementOrderBottomMiddle[2] = list3;
                    placementOrderBottomMiddle[3] = list4;
                    placementOrderBottomMiddle[4] = list5;
                    placementOrderBottomMiddle[5] = list6;
                    placementOrderBottomMiddle[6] = list7;
                    placementOrderBottomMiddle[7] = list8;
                    placementOrderBottomMiddle[8] = list9;
                    break;
                case 8:
                    placementOrderBottomRight[0] = list1;
                    placementOrderBottomRight[1] = list2;
                    placementOrderBottomRight[2] = list3;
                    placementOrderBottomRight[3] = list4;
                    placementOrderBottomRight[4] = list5;
                    placementOrderBottomRight[5] = list6;
                    placementOrderBottomRight[6] = list7;
                    placementOrderBottomRight[7] = list8;
                    placementOrderBottomRight[8] = list9;
                    break;
            }
        }

        private List<int> fillList(int start, int finish)
        {
            List<int> ourList = new List<int>();
            for (int i = start; i <= finish; i++)
            {
                ourList.Add(i);
            }
            return ourList;
        }

        private List<List<int>> createBlankPlacementOrder()
        {
            List<List<int>> result = new List<List<int>>();
            for (int i = 0; i < 9; i++)
            {
                List<int> nullPlacementOrdersForOneNumber = new List<int>();
                for (int j = 0; j < 9; j++)
                {
                    nullPlacementOrdersForOneNumber.Add(-1);
                }
                result.Add(nullPlacementOrdersForOneNumber);
            }
            return result;
        }

        public void showPlacementOrdersForBox(int boxPos)
        {
            string result = "";
            result += "*-* Box: " + boxPos + " *-*\n";
            switch (boxPos)
            {
                case 0:
                    for (int i = 0; i < 9; i++)
                    {
                        result += "Num " + i + ": [";
                        for (int j = 0; j < 9; j++)
                        {
                            result += placementOrderTopLeft[i][j] + " ";
                        }
                        result += "]\n";
                    }
                    break;
                case 1:
                    for (int i = 0; i < 9; i++)
                    {
                        result += "Num " + i + ": [";
                        for (int j = 0; j < 9; j++)
                        {
                            result += placementOrderTopMiddle[i][j] + " ";
                        }
                        result += "]\n";
                    }
                    break;
                case 2:
                    for (int i = 0; i < 9; i++)
                    {
                        result += "Num " + i + ": [";
                        for (int j = 0; j < 9; j++)
                        {
                            result += placementOrderTopRight[i][j] + " ";
                        }
                        result += "]\n";
                    }
                    break;
                case 3:
                    for (int i = 0; i < 9; i++)
                    {
                        result += "Num " + i + ": [";
                        for (int j = 0; j < 9; j++)
                        {
                            result += placementOrderMiddleLeft[i][j] + " ";
                        }
                        result += "]\n";
                    }
                    break;
                case 4:
                    for (int i = 0; i < 9; i++)
                    {
                        result += "Num " + i + ": [";
                        for (int j = 0; j < 9; j++)
                        {
                            result += placementOrderMiddleMiddle[i][j] + " ";
                        }
                        result += "]\n";
                    }
                    break;
                case 5:
                    for (int i = 0; i < 9; i++)
                    {
                        result += "Num " + i + ": [";
                        for (int j = 0; j < 9; j++)
                        {
                            result += placementOrderMiddleRight[i][j] + " ";
                        }
                        result += "]\n";
                    }
                    break;
                case 6:
                    for (int i = 0; i < 9; i++)
                    {
                        result += "Num " + i + ": [";
                        for (int j = 0; j < 9; j++)
                        {
                            result += placementOrderBottomLeft[i][j] + " ";
                        }
                        result += "]\n";
                    }
                    break;
                case 7:
                    for (int i = 0; i < 9; i++)
                    {
                        result += "Num " + i + ": [";
                        for (int j = 0; j < 9; j++)
                        {
                            result += placementOrderBottomMiddle[i][j] + " ";
                        }
                        result += "]\n";
                    }
                    break;
                case 8:
                    for (int i = 0; i < 9; i++)
                    {
                        result += "Num " + i + ": [";
                        for (int j = 0; j < 9; j++)
                        {
                            result += placementOrderBottomRight[i][j] + " ";
                        }
                        result += "]\n";
                    }
                    break;
            }
            MessageBox.Show(result);
        }

        public void populate(WelcomeWindow ui)
        {
            /* Logic for "else": if we can't make it work in the box we are working on, then we need to edit a previous
             * box placement, meaning we will need to work backwards on the previous box (9 -> 1), and generate new
             * placement attempts for the box we just left
             * Also note: Each time we cannot place a number in a square and have to change the previous number location,
             * we must generate a new placement attempt */
            try
            {
                popTopLeft();
                ui.updateProgressBar();
                Debug.WriteLine("[SudokuGrid] PopTopLeft - Done (" + stopWatch.Elapsed + "s)");
                popTopMid();
                ui.updateProgressBar();
                Debug.WriteLine("[SudokuGrid] PopTopMid - Done (" + stopWatch.Elapsed + "s)");
                popTopRight();
                ui.updateProgressBar();
                Debug.WriteLine("[SudokuGrid] PopTopRight - Done (" + stopWatch.Elapsed + "s)");
                popMidLeft();
                ui.updateProgressBar();
                Debug.WriteLine("[SudokuGrid] PopMidLeft - Done (" + stopWatch.Elapsed + "s)");
                popMidMid();
                ui.updateProgressBar();
                Debug.WriteLine("[SudokuGrid] PopMidMid - Done (" + stopWatch.Elapsed + "s)");
                popMidRight();
                ui.updateProgressBar();
                Debug.WriteLine("[SudokuGrid] PopMidRight - Done (" + stopWatch.Elapsed + "s)");
                popBottomLeft();
                ui.updateProgressBar();
                Debug.WriteLine("[SudokuGrid] PopBottomLeft - Done (" + stopWatch.Elapsed + "s)");
                popBottomMiddle();
                ui.updateProgressBar();
                Debug.WriteLine("[SudokuGrid] PopBottomMiddle - Done (" + stopWatch.Elapsed + "s)");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            int gridAttempts = 1, maxGridAttempts = 10;
            // If we can populate bottom right then we have found a solution, append to text file
            bool success = false;
            if (popBottomRight())
            {
                Debug.WriteLine("[SudokuGrid] PopBottomRight - Done (" + stopWatch.Elapsed + "s)");
                success = true;
            }
            else // Must try previous box, set new placement orders for BottomMiddle
            {
                try
                {
                    while (maxGridAttempts > 0)
                    {
                        Debug.WriteLine("[SudokuGrid] Failure on grid attempt " + gridAttempts + "  - Re-attempting (" + stopWatch.Elapsed + "s)");
                        resetBoxToCompleteZero(8);
                        resetBoxToCompleteZero(7);
                        gridAttempts++;
                        popBottomMiddle();

                        // If we can populate bottom right then we have found a solution, append to text file
                        if (popBottomRight())
                        {
                            Debug.WriteLine("[SudokuGrid] PopBottomRight - Done (" + stopWatch.Elapsed + "s)");
                            success = true;
                            break;
                        }
                        else
                        {
                            maxGridAttempts--;
                        }
                        ui.updateProgressBar();
                    }
                    maxGridAttempts = 10;
                    if (!success)
                    {
                        while (maxGridAttempts > 0)
                        {
                            Debug.WriteLine("[SudokuGrid] Failure on grid attempt " + gridAttempts + "  - Re-attempting (" + stopWatch.Elapsed + "s)");
                            resetBoxToCompleteZero(8);
                            resetBoxToCompleteZero(7);
                            resetBoxToCompleteZero(6);
                            gridAttempts++;
                            popBottomLeft();
                            popBottomMiddle();

                            // If we can populate bottom right then we have found a solution, append to text file
                            if (popBottomRight())
                            {
                                Debug.WriteLine("[SudokuGrid] PopBottomRight - Done (" + stopWatch.Elapsed + "s)");
                                success = true;
                                break;
                            }
                            else
                            {
                                maxGridAttempts--;
                            }
                            ui.updateProgressBar();
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: " + e.Message);
                }
            }
            if (success)
            {
                appendPuzzleToSaveFile(path, file);
                ui.completeProgressBar();
                ui.updateProgressLabel("Success!");
                Debug.WriteLine("[SudokuGrid] Grid created successfully in " + gridAttempts + " attempts! (" + stopWatch.Elapsed + "s)");
            }
            else
            {
                ui.completeProgressBar();
                ui.updateProgressLabel("Grid failure! Try running again?");
            }
        }

        private bool popTopLeft()
        {
            bool result = true;

            List<List<int>> topLeftBox = blank3x3Box();
            int numToPopulate = 1;
            int posInBoxToTry = 0;

            try
            {
                for (int i = 0; i < 9; i++)
                {
                    posInBoxToTry = placementOrderTopLeft[i][0];
                    placementOrderTopLeft[i].RemoveAt(0);
                    int row = getRowFromBoxPos(0, posInBoxToTry);
                    int col = getColFromBoxPos(0, posInBoxToTry);

                    bool isPlacementOk = placeNumber(numToPopulate, topLeftBox, posInBoxToTry, 0);
                    while (!isPlacementOk)
                    {
                        posInBoxToTry = placementOrderTopLeft[i][0];
                        placementOrderTopLeft[i].RemoveAt(0);
                        row = getRowFromBoxPos(0, posInBoxToTry);
                        col = getColFromBoxPos(0, posInBoxToTry);
                        isPlacementOk = placeNumber(numToPopulate, topLeftBox, posInBoxToTry, 0);
                    }
                    topLeftBox[row % 3][col % 3] = numToPopulate;

                    numToPopulate++;
                    addBoxWeHavebeenWorkingOnToGrid(topLeftBox, 1);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return result;
        }

        private bool popTopMid()
        {
            bool result = true;

            List<List<int>> topMidBox = blank3x3Box();
            int numToPopulate = 1;
            int posInBoxToTry = 0;

            try
            {
                for (int i = 0; i < 9; i++)
                {
                    // On first iteration numToPopulate-1 == 0
                    posInBoxToTry = placementOrderTopMiddle[numToPopulate - 1][0];
                    placementOrderTopMiddle[numToPopulate - 1].RemoveAt(0);

                    result = placeNumber(numToPopulate, topMidBox, posInBoxToTry, 1);
                    while (!result)
                    {
                        //MessageBox.Show("going round again: placing " + numToPopulate + " in " + posInBoxToTry);
                        // Cannot continue placing numbers, must refer back to previous box
                        if (numToPopulate - 1 < 0)
                        {
                            //MessageBox.Show("MUST REFER TO PREVIOUS BOX");
                            return false;
                        }
                        // We can continue to place (or re-place) numbers
                        if (placementOrderTopMiddle[numToPopulate - 1].Count >= 1)
                        {
                            posInBoxToTry = placementOrderTopMiddle[numToPopulate - 1][0];
                            placementOrderTopMiddle[numToPopulate - 1].RemoveAt(0);
                            result = placeNumber(numToPopulate, topMidBox, posInBoxToTry, 1);

                            // Able to place numToPopulate in location
                            if (result)
                            {
                                // Was a re-placement, so add it back in and try placing the next number up again
                                if (numToPopulate - 1 != i)
                                {
                                    int row = getRowFromBoxPos(1, posInBoxToTry);
                                    int col = getColFromBoxPos(1, posInBoxToTry);
                                    //MessageBox.Show("REPLACEMENT: Populating replacement no: " + numToPopulate + " in [" + row + "," + col + "]");
                                    removeNumFromBox(topMidBox, numToPopulate);
                                    topMidBox[row % 3][col % 3] = numToPopulate;
                                    numToPopulate++;
                                    result = false;
                                } // if
                                // Wasn't a re-placement, place number and advance in for loop
                                else
                                {
                                    //int row = getRowFromBoxPos(0, posInBoxToTry);
                                    //int col = getColFromBoxPos(0, posInBoxToTry);
                                    //topMidBox[row % 3][col % 3] = numToPopulate;
                                    //numToPopulate++;
                                    result = true;
                                    break;
                                } // else
                            } // if
                            // Unable to place numToPopulate in location
                            else
                            {
                                //MessageBox.Show("cannae place no here");
                                result = false;
                            }
                        } // if                    
                        // We can't place our new number anywhere, try re-placing the previous number
                        else if (numToPopulate >= 2)
                        {
                            // Remove any placement of number we couldn't place
                            removeNumFromBox(topMidBox, numToPopulate);

                            // Provide number we tried to place with a new placement order
                            doSinglePlacementFill(1, numToPopulate - 1);

                            // Reduce numberToPopulate, to begin placing previous number again
                            numToPopulate--;

                            // Try placing previous number, with previous number's placement order
                            result = false;
                        } // else if
                        // CODE HERE SHOULD NOW CHECK BACK THE PREVIOUS BOX, AND RE-WRITE IT
                        // AT THE MOMENT IT JUST EXITS
                        else
                        {
                            if (System.Windows.Forms.Application.MessageLoop)
                            {
                                // WinForms app
                                System.Windows.Forms.Application.Exit();
                            } // if
                            else
                            {
                                // Console app
                                System.Environment.Exit(1);
                            } // else
                        } // else
                    } // while   
                    int row2 = getRowFromBoxPos(1, posInBoxToTry);
                    int col2 = getColFromBoxPos(1, posInBoxToTry);
                    //MessageBox.Show("Populating a non-replacement no: " + numToPopulate + " at [" + row2 + "," + col2 + "]"); 
                    topMidBox[row2 % 3][col2 % 3] = numToPopulate;
                    numToPopulate++;
                } // for
                addBoxWeHavebeenWorkingOnToGrid(topMidBox, 2);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return result;
        } // popTopMid

        private bool popTopRight()
        {
            bool result = true;

            List<List<int>> topRightBox = blank3x3Box();
            int numToPopulate = 1;
            int posInBoxToTry = 0;

            try
            {
                for (int i = 0; i < 9; i++)
                {
                    // On first iteration numToPopulate-1 == 0
                    posInBoxToTry = placementOrderTopRight[numToPopulate - 1][0];
                    placementOrderTopRight[numToPopulate - 1].RemoveAt(0);

                    result = placeNumber(numToPopulate, topRightBox, posInBoxToTry, 2);
                    while (!result)
                    {
                        // Cannot continue placing numbers, must refer back to previous box
                        if (numToPopulate - 1 < 0)
                        {
                            return false;
                        }
                        // We can continue to place (or re-place) numbers
                        if (placementOrderTopRight[numToPopulate - 1].Count >= 1)
                        {
                            posInBoxToTry = placementOrderTopRight[numToPopulate - 1][0];
                            placementOrderTopRight[numToPopulate - 1].RemoveAt(0);
                            result = placeNumber(numToPopulate, topRightBox, posInBoxToTry, 2);

                            // Able to place numToPopulate in location
                            if (result)
                            {
                                // Was a re-placement, so add it back in and try placing the next number up again
                                if (numToPopulate - 1 != i)
                                {
                                    int row = getRowFromBoxPos(2, posInBoxToTry);
                                    int col = getColFromBoxPos(2, posInBoxToTry);
                                    removeNumFromBox(topRightBox, numToPopulate);
                                    topRightBox[row % 3][col % 3] = numToPopulate;
                                    numToPopulate++;
                                    result = false;
                                } // if
                                // Wasn't a re-placement, place number and advance in for loop
                                else
                                {
                                    result = true;
                                    break;
                                } // else
                            } // if
                            // Unable to place numToPopulate in location
                            else
                            {
                                result = false;
                            }
                        } // if                    
                        // We can't place our new number anywhere, try re-placing the previous number
                        else if (numToPopulate >= 2)
                        {
                            // Remove any placement of number we couldn't place
                            removeNumFromBox(topRightBox, numToPopulate);

                            // Provide number we tried to place with a new placement order
                            doSinglePlacementFill(2, numToPopulate - 1);

                            // Reduce numberToPopulate, to begin placing previous number again
                            numToPopulate--;

                            // Try placing previous number, with previous number's placement order
                            result = false;
                        } // else if
                        // CODE HERE SHOULD NOW CHECK BACK THE PREVIOUS BOX, AND RE-WRITE IT
                        // AT THE MOMENT IT JUST EXITS
                        else
                        {
                            if (System.Windows.Forms.Application.MessageLoop)
                            {
                                // WinForms app
                                System.Windows.Forms.Application.Exit();
                            } // if
                            else
                            {
                                // Console app
                                System.Environment.Exit(1);
                            } // else
                        } // else
                    } // while   
                    int row2 = getRowFromBoxPos(2, posInBoxToTry);
                    int col2 = getColFromBoxPos(2, posInBoxToTry);
                    topRightBox[row2 % 3][col2 % 3] = numToPopulate;
                    numToPopulate++;
                } // for
                addBoxWeHavebeenWorkingOnToGrid(topRightBox, 3);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return result;
        } // popTopRight

        private bool popMidLeft()
        {
            bool result = true;

            List<List<int>> midLeftBox = blank3x3Box();
            int numToPopulate = 1;
            int posInBoxToTry = 0;

            try
            {
                for (int i = 0; i < 9; i++)
                {
                    // On first iteration numToPopulate-1 == 0
                    posInBoxToTry = placementOrderMiddleLeft[numToPopulate - 1][0];
                    placementOrderMiddleLeft[numToPopulate - 1].RemoveAt(0);

                    result = placeNumber(numToPopulate, midLeftBox, posInBoxToTry, 3);
                    while (!result)
                    {
                        // Cannot continue placing numbers, must refer back to previous box
                        if (numToPopulate - 1 < 0)
                        {
                            return false;
                        }
                        // We can continue to place (or re-place) numbers
                        if (placementOrderMiddleLeft[numToPopulate - 1].Count >= 1)
                        {
                            posInBoxToTry = placementOrderMiddleLeft[numToPopulate - 1][0];
                            placementOrderMiddleLeft[numToPopulate - 1].RemoveAt(0);
                            result = placeNumber(numToPopulate, midLeftBox, posInBoxToTry, 3);

                            // Able to place numToPopulate in location
                            if (result)
                            {
                                // Was a re-placement, so add it back in and try placing the next number up again
                                if (numToPopulate - 1 != i)
                                {
                                    int row = getRowFromBoxPos(3, posInBoxToTry);
                                    int col = getColFromBoxPos(3, posInBoxToTry);
                                    removeNumFromBox(midLeftBox, numToPopulate);
                                    midLeftBox[row % 3][col % 3] = numToPopulate;
                                    numToPopulate++;
                                    result = false;
                                } // if
                                // Wasn't a re-placement, place number and advance in for loop
                                else
                                {
                                    result = true;
                                    break;
                                } // else
                            } // if
                            // Unable to place numToPopulate in location
                            else
                            {
                                result = false;
                            }
                        } // if                    
                        // We can't place our new number anywhere, try re-placing the previous number
                        else if (numToPopulate >= 2)
                        {
                            // Remove any placement of number we couldn't place
                            removeNumFromBox(midLeftBox, numToPopulate);

                            // Provide number we tried to place with a new placement order
                            doSinglePlacementFill(3, numToPopulate - 1);

                            // Reduce numberToPopulate, to begin placing previous number again
                            numToPopulate--;

                            // Try placing previous number, with previous number's placement order
                            result = false;
                        } // else if
                        // CODE HERE SHOULD NOW CHECK BACK THE PREVIOUS BOX, AND RE-WRITE IT
                        // AT THE MOMENT IT JUST EXITS
                        else
                        {
                            if (System.Windows.Forms.Application.MessageLoop)
                            {
                                // WinForms app
                                System.Windows.Forms.Application.Exit();
                            } // if
                            else
                            {
                                // Console app
                                System.Environment.Exit(1);
                            } // else
                        } // else
                    } // while   
                    int row2 = getRowFromBoxPos(3, posInBoxToTry);
                    int col2 = getColFromBoxPos(3, posInBoxToTry);
                    midLeftBox[row2 % 3][col2 % 3] = numToPopulate;
                    numToPopulate++;
                } // for
                addBoxWeHavebeenWorkingOnToGrid(midLeftBox, 4);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return result;
        } // popMidLeft

        private bool popMidMid()
        {
            bool result = true;

            List<List<int>> midMidBox = blank3x3Box();
            int numToPopulate = 1;
            int posInBoxToTry = 0;

            try
            {
                for (int i = 0; i < 9; i++)
                {
                    // On first iteration numToPopulate-1 == 0
                    posInBoxToTry = placementOrderMiddleMiddle[numToPopulate - 1][0];
                    placementOrderMiddleMiddle[numToPopulate - 1].RemoveAt(0);

                    result = placeNumber(numToPopulate, midMidBox, posInBoxToTry, 4);
                    while (!result)
                    {
                        // Cannot continue placing numbers, must refer back to previous box
                        if (numToPopulate - 1 < 0)
                        {
                            return false;
                        }
                        // We can continue to place (or re-place) numbers
                        if (placementOrderMiddleMiddle[numToPopulate - 1].Count >= 1)
                        {
                            posInBoxToTry = placementOrderMiddleMiddle[numToPopulate - 1][0];
                            placementOrderMiddleMiddle[numToPopulate - 1].RemoveAt(0);
                            result = placeNumber(numToPopulate, midMidBox, posInBoxToTry, 4);

                            // Able to place numToPopulate in location
                            if (result)
                            {
                                // Was a re-placement, so add it back in and try placing the next number up again
                                if (numToPopulate - 1 != i)
                                {
                                    int row = getRowFromBoxPos(4, posInBoxToTry);
                                    int col = getColFromBoxPos(4, posInBoxToTry);
                                    removeNumFromBox(midMidBox, numToPopulate);
                                    midMidBox[row % 3][col % 3] = numToPopulate;
                                    numToPopulate++;
                                    result = false;
                                } // if
                                // Wasn't a re-placement, place number and advance in for loop
                                else
                                {
                                    result = true;
                                    break;
                                } // else
                            } // if
                            // Unable to place numToPopulate in location
                            else
                            {
                                result = false;
                            }
                        } // if                    
                        // We can't place our new number anywhere, try re-placing the previous number
                        else if (numToPopulate >= 2)
                        {
                            // Remove any placement of number we couldn't place
                            removeNumFromBox(midMidBox, numToPopulate);

                            // Provide number we tried to place with a new placement order
                            doSinglePlacementFill(4, numToPopulate - 1);

                            // Reduce numberToPopulate, to begin placing previous number again
                            numToPopulate--;

                            // Try placing previous number, with previous number's placement order
                            result = false;
                        } // else if
                        // CODE HERE SHOULD NOW CHECK BACK THE PREVIOUS BOX, AND RE-WRITE IT
                        // AT THE MOMENT IT JUST EXITS
                        else
                        {
                            if (System.Windows.Forms.Application.MessageLoop)
                            {
                                // WinForms app
                                System.Windows.Forms.Application.Exit();
                            } // if
                            else
                            {
                                // Console app
                                System.Environment.Exit(1);
                            } // else
                        } // else
                    } // while   
                    int row2 = getRowFromBoxPos(4, posInBoxToTry);
                    int col2 = getColFromBoxPos(4, posInBoxToTry);
                    midMidBox[row2 % 3][col2 % 3] = numToPopulate;
                    numToPopulate++;
                } // for
                addBoxWeHavebeenWorkingOnToGrid(midMidBox, 5);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return result;
        } // popMidMid

        private bool popMidRight()
        {
            bool result = true;

            List<List<int>> midRightBox = blank3x3Box();
            int numToPopulate = 1;
            int posInBoxToTry = 0;

            try
            {
                for (int i = 0; i < 9; i++)
                {
                    // On first iteration numToPopulate-1 == 0
                    posInBoxToTry = placementOrderMiddleRight[numToPopulate - 1][0];
                    placementOrderMiddleRight[numToPopulate - 1].RemoveAt(0);

                    result = placeNumber(numToPopulate, midRightBox, posInBoxToTry, 5);
                    while (!result)
                    {
                        // Cannot continue placing numbers, must refer back to previous box
                        if (numToPopulate - 1 < 0)
                        {
                            return false;
                        }
                        // We can continue to place (or re-place) numbers
                        if (placementOrderMiddleRight[numToPopulate - 1].Count >= 1)
                        {
                            posInBoxToTry = placementOrderMiddleRight[numToPopulate - 1][0];
                            placementOrderMiddleRight[numToPopulate - 1].RemoveAt(0);
                            result = placeNumber(numToPopulate, midRightBox, posInBoxToTry, 5);

                            // Able to place numToPopulate in location
                            if (result)
                            {
                                // Was a re-placement, so add it back in and try placing the next number up again
                                if (numToPopulate - 1 != i)
                                {
                                    int row = getRowFromBoxPos(5, posInBoxToTry);
                                    int col = getColFromBoxPos(5, posInBoxToTry);
                                    removeNumFromBox(midRightBox, numToPopulate);
                                    midRightBox[row % 3][col % 3] = numToPopulate;
                                    numToPopulate++;
                                    result = false;
                                } // if
                                // Wasn't a re-placement, place number and advance in for loop
                                else
                                {
                                    result = true;
                                    break;
                                } // else
                            } // if
                            // Unable to place numToPopulate in location
                            else
                            {
                                result = false;
                            }
                        } // if                    
                        // We can't place our new number anywhere, try re-placing the previous number
                        else if (numToPopulate >= 2)
                        {
                            // Remove any placement of number we couldn't place
                            removeNumFromBox(midRightBox, numToPopulate);

                            // Provide number we tried to place with a new placement order
                            doSinglePlacementFill(5, numToPopulate - 1);

                            // Reduce numberToPopulate, to begin placing previous number again
                            numToPopulate--;

                            // Try placing previous number, with previous number's placement order
                            result = false;
                        } // else if
                        // CODE HERE SHOULD NOW CHECK BACK THE PREVIOUS BOX, AND RE-WRITE IT
                        // AT THE MOMENT IT JUST EXITS
                        else
                        {
                            if (System.Windows.Forms.Application.MessageLoop)
                            {
                                // WinForms app
                                System.Windows.Forms.Application.Exit();
                            } // if
                            else
                            {
                                // Console app
                                System.Environment.Exit(1);
                            } // else
                        } // else
                    } // while   
                    int row2 = getRowFromBoxPos(5, posInBoxToTry);
                    int col2 = getColFromBoxPos(5, posInBoxToTry);
                    midRightBox[row2 % 3][col2 % 3] = numToPopulate;
                    numToPopulate++;
                } // for
                addBoxWeHavebeenWorkingOnToGrid(midRightBox, 6);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return result;
        } // popMidRight

        private bool popBottomLeft()
        {
            bool result = true;

            List<List<int>> bottomLeftBox = blank3x3Box();
            int numToPopulate = 1;
            int posInBoxToTry = 0;

            try
            {
                for (int i = 0; i < 9; i++)
                {
                    // On first iteration numToPopulate-1 == 0
                    posInBoxToTry = placementOrderBottomLeft[numToPopulate - 1][0];
                    placementOrderBottomLeft[numToPopulate - 1].RemoveAt(0);

                    result = placeNumber(numToPopulate, bottomLeftBox, posInBoxToTry, 6);
                    while (!result)
                    {
                        // Cannot continue placing numbers, must refer back to previous box
                        if (numToPopulate - 1 < 0)
                        {
                            return false;
                        }
                        // We can continue to place (or re-place) numbers
                        if (placementOrderBottomLeft[numToPopulate - 1].Count >= 1)
                        {
                            posInBoxToTry = placementOrderBottomLeft[numToPopulate - 1][0];
                            placementOrderBottomLeft[numToPopulate - 1].RemoveAt(0);
                            result = placeNumber(numToPopulate, bottomLeftBox, posInBoxToTry, 6);

                            // Able to place numToPopulate in location
                            if (result)
                            {
                                // Was a re-placement, so add it back in and try placing the next number up again
                                if (numToPopulate - 1 != i)
                                {
                                    int row = getRowFromBoxPos(6, posInBoxToTry);
                                    int col = getColFromBoxPos(6, posInBoxToTry);
                                    removeNumFromBox(bottomLeftBox, numToPopulate);
                                    bottomLeftBox[row % 3][col % 3] = numToPopulate;
                                    numToPopulate++;
                                    result = false;
                                } // if
                                // Wasn't a re-placement, place number and advance in for loop
                                else
                                {
                                    result = true;
                                    break;
                                } // else
                            } // if
                            // Unable to place numToPopulate in location
                            else
                            {
                                result = false;
                            }
                        } // if                    
                        // We can't place our new number anywhere, try re-placing the previous number
                        else if (numToPopulate >= 2)
                        {
                            // Remove any placement of number we couldn't place
                            removeNumFromBox(bottomLeftBox, numToPopulate);

                            // Provide number we tried to place with a new placement order
                            doSinglePlacementFill(6, numToPopulate - 1);

                            // Reduce numberToPopulate, to begin placing previous number again
                            numToPopulate--;

                            // Try placing previous number, with previous number's placement order
                            result = false;
                        } // else if
                        // CODE HERE SHOULD NOW CHECK BACK THE PREVIOUS BOX, AND RE-WRITE IT
                        // AT THE MOMENT IT JUST EXITS
                        else
                        {
                            if (System.Windows.Forms.Application.MessageLoop)
                            {
                                // WinForms app
                                System.Windows.Forms.Application.Exit();
                            } // if
                            else
                            {
                                // Console app
                                System.Environment.Exit(1);
                            } // else
                        } // else
                    } // while   
                    int row2 = getRowFromBoxPos(6, posInBoxToTry);
                    int col2 = getColFromBoxPos(6, posInBoxToTry);
                    bottomLeftBox[row2 % 3][col2 % 3] = numToPopulate;
                    numToPopulate++;
                } // for
                addBoxWeHavebeenWorkingOnToGrid(bottomLeftBox, 7);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return result;
        } // popBottomLeft

        private bool popBottomMiddle()
        {
            bool result = true;

            List<List<int>> bottomMiddleBox = blank3x3Box();
            int numToPopulate = 1;
            int posInBoxToTry = 0;

            try
            {
                for (int i = 0; i < 9; i++)
                {
                    // On first iteration numToPopulate-1 == 0
                    posInBoxToTry = placementOrderBottomMiddle[numToPopulate - 1][0];
                    placementOrderBottomMiddle[numToPopulate - 1].RemoveAt(0);

                    result = placeNumber(numToPopulate, bottomMiddleBox, posInBoxToTry, 7);
                    while (!result)
                    {
                        // Cannot continue placing numbers, must refer back to previous box
                        if (numToPopulate - 1 < 0)
                        {
                            return false;
                        }
                        // We can continue to place (or re-place) numbers
                        if (placementOrderBottomMiddle[numToPopulate - 1].Count >= 1)
                        {
                            posInBoxToTry = placementOrderBottomMiddle[numToPopulate - 1][0];
                            placementOrderBottomMiddle[numToPopulate - 1].RemoveAt(0);
                            result = placeNumber(numToPopulate, bottomMiddleBox, posInBoxToTry, 7);

                            // Able to place numToPopulate in location
                            if (result)
                            {
                                // Was a re-placement, so add it back in and try placing the next number up again
                                if (numToPopulate - 1 != i)
                                {
                                    int row = getRowFromBoxPos(7, posInBoxToTry);
                                    int col = getColFromBoxPos(7, posInBoxToTry);
                                    removeNumFromBox(bottomMiddleBox, numToPopulate);
                                    bottomMiddleBox[row % 3][col % 3] = numToPopulate;
                                    numToPopulate++;
                                    result = false;
                                } // if
                                // Wasn't a re-placement, place number and advance in for loop
                                else
                                {
                                    result = true;
                                    break;
                                } // else
                            } // if
                            // Unable to place numToPopulate in location
                            else
                            {
                                result = false;
                            }
                        } // if                    
                        // We can't place our new number anywhere, try re-placing the previous number
                        else if (numToPopulate >= 2)
                        {
                            // Remove any placement of number we couldn't place
                            removeNumFromBox(bottomMiddleBox, numToPopulate);

                            // Provide number we tried to place with a new placement order
                            doSinglePlacementFill(7, numToPopulate - 1);

                            // Reduce numberToPopulate, to begin placing previous number again
                            numToPopulate--;

                            // Try placing previous number, with previous number's placement order
                            result = false;
                        } // else if
                        // CODE HERE SHOULD NOW CHECK BACK THE PREVIOUS BOX, AND RE-WRITE IT
                        // AT THE MOMENT IT JUST EXITS
                        else
                        {
                            if (System.Windows.Forms.Application.MessageLoop)
                            {
                                // WinForms app
                                System.Windows.Forms.Application.Exit();
                            } // if
                            else
                            {
                                // Console app
                                System.Environment.Exit(1);
                            } // else
                        } // else
                    } // while   
                    int row2 = getRowFromBoxPos(7, posInBoxToTry);
                    int col2 = getColFromBoxPos(7, posInBoxToTry);
                    bottomMiddleBox[row2 % 3][col2 % 3] = numToPopulate;
                    numToPopulate++;
                } // for
                addBoxWeHavebeenWorkingOnToGrid(bottomMiddleBox, 8);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return result;
        } // popBottomLeft

        private bool popBottomRight()
        {
            bool result = true;

            List<List<int>> bottomRightBox = blank3x3Box();
            int numToPopulate = 1;
            int posInBoxToTry = 0;

            try
            {
                for (int i = 0; i < 9; i++)
                {
                    // On first iteration numToPopulate-1 == 0
                    posInBoxToTry = placementOrderBottomRight[numToPopulate - 1][0];
                    placementOrderBottomRight[numToPopulate - 1].RemoveAt(0);

                    result = placeNumber(numToPopulate, bottomRightBox, posInBoxToTry, 8);
                    while (!result)
                    {
                        // Cannot continue placing numbers, must refer back to previous box
                        if (numToPopulate - 1 < 0)
                        {
                            return false;
                        }
                        // We can continue to place (or re-place) numbers
                        if (placementOrderBottomRight[numToPopulate - 1].Count >= 1)
                        {
                            posInBoxToTry = placementOrderBottomRight[numToPopulate - 1][0];
                            placementOrderBottomRight[numToPopulate - 1].RemoveAt(0);
                            result = placeNumber(numToPopulate, bottomRightBox, posInBoxToTry, 8);

                            // Able to place numToPopulate in location
                            if (result)
                            {
                                // Was a re-placement, so add it back in and try placing the next number up again
                                if (numToPopulate - 1 != i)
                                {
                                    int row = getRowFromBoxPos(8, posInBoxToTry);
                                    int col = getColFromBoxPos(8, posInBoxToTry);
                                    removeNumFromBox(bottomRightBox, numToPopulate);
                                    bottomRightBox[row % 3][col % 3] = numToPopulate;
                                    numToPopulate++;
                                    result = false;
                                } // if
                                // Wasn't a re-placement, place number and advance in for loop
                                else
                                {
                                    result = true;
                                    break;
                                } // else
                            } // if
                            // Unable to place numToPopulate in location
                            else
                            {
                                result = false;
                            }
                        } // if                    
                        // We can't place our new number anywhere, try re-placing the previous number
                        else if (numToPopulate >= 2)
                        {
                            // Remove any placement of number we couldn't place
                            removeNumFromBox(bottomRightBox, numToPopulate);

                            // Provide number we tried to place with a new placement order
                            doSinglePlacementFill(8, numToPopulate - 1);

                            // Reduce numberToPopulate, to begin placing previous number again
                            numToPopulate--;

                            // Try placing previous number, with previous number's placement order
                            result = false;
                        } // else if
                        // CODE HERE SHOULD NOW CHECK BACK THE PREVIOUS BOX, AND RE-WRITE IT
                        // AT THE MOMENT IT JUST EXITS
                        else
                        {
                            return false;
                        } // else
                    } // while   
                    int row2 = getRowFromBoxPos(8, posInBoxToTry);
                    int col2 = getColFromBoxPos(8, posInBoxToTry);
                    bottomRightBox[row2 % 3][col2 % 3] = numToPopulate;
                    numToPopulate++;
                } // for
                addBoxWeHavebeenWorkingOnToGrid(bottomRightBox, 9);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            return result;
        } // popBottomLeft

        private void appendPuzzleToSaveFile(string pathName, string fileName)
        {
            // Write our completed solution to a text file so we can randomly select it for play in future
            string res = "";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    res += grid[i][j] + ",";
                }
            }
            res = res.Remove(res.Length - 1);

            File.AppendAllText(pathName + "" + fileName, res + Environment.NewLine);
           
        }

        private void resetBoxToCompleteZero(int boxPos)
        {
            switch (boxPos)
            {
                case 0:
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                            grid[i][j] = 0;
                    break;
                case 1:
                    for (int i = 0; i < 3; i++)
                        for (int j = 3; j < 6; j++)
                            grid[i][j] = 0;
                    break;
                case 2:
                    for (int i = 0; i < 3; i++)
                        for (int j = 6; j < 9; j++)
                            grid[i][j] = 0;
                    break;
                case 3:
                    for (int i = 3; i < 6; i++)
                        for (int j = 0; j < 3; j++)
                            grid[i][j] = 0;
                    break;
                case 4:
                    for (int i = 3; i < 6; i++)
                        for (int j = 3; j < 6; j++)
                            grid[i][j] = 0;
                    break;
                case 5:
                    for (int i = 3; i < 6; i++)
                        for (int j = 6; j < 9; j++)
                            grid[i][j] = 0;
                    break;
                case 6:
                    for (int i = 6; i < 9; i++)
                        for (int j = 0; j < 3; j++)
                            grid[i][j] = 0;
                    break;
                case 7:
                    for (int i = 6; i < 9; i++)
                        for (int j = 3; j < 6; j++)
                            grid[i][j] = 0;
                    break;
                case 8:
                    for (int i = 6; i < 9; i++)
                        for (int j = 6; j < 9; j++)
                            grid[i][j] = 0;
                    break;
            } // switch
            doInitialPlacementFill(boxPos);
        }

        private void removeNumFromBox(List<List<int>> boxTocheck, int numToRemove)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (boxTocheck[i][j] == numToRemove)
                        boxTocheck[i][j] = 0;
                }
        }

        private bool checkBoxValidity(int boxPos)
        {
            bool result = true;
            int[] numCount = new int[9];
            for (int i = 0; i < numCount.Length; i++)
                numCount[i] = 0;
            switch (boxPos)
            {
                case 0:
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                        {
                            numCount[grid[i % 3][j % 3] - 1]++;
                        }
                    break;
                case 1:
                    for (int i = 0; i < 3; i++)
                        for (int j = 3; j < 6; j++)
                        {
                            numCount[grid[i % 3][j % 3] - 1]++;
                        }
                    break;
                case 2:
                    for (int i = 0; i < 3; i++)
                        for (int j = 6; j < 9; j++)
                        {
                            numCount[grid[i % 3][j % 3] - 1]++;
                        }
                    break;
                case 3:
                    for (int i = 3; i < 6; i++)
                        for (int j = 0; j < 3; j++)
                        {
                            numCount[grid[i % 3][j % 3] - 1]++;
                        }
                    break;
                case 4:
                    for (int i = 3; i < 6; i++)
                        for (int j = 3; j < 6; j++)
                        {
                            numCount[grid[i % 3][j % 3] - 1]++;
                        }
                    break;
                case 5:
                    for (int i = 3; i < 6; i++)
                        for (int j = 6; j < 9; j++)
                        {
                            numCount[grid[i % 3][j % 3] - 1]++;
                        }
                    break;
                case 6:
                    for (int i = 6; i < 9; i++)
                        for (int j = 0; j < 3; j++)
                        {
                            numCount[grid[i % 3][j % 3] - 1]++;
                        }
                    break;
                case 7:
                    for (int i = 6; i < 9; i++)
                        for (int j = 3; j < 6; j++)
                        {
                            numCount[grid[i % 3][j % 3] - 1]++;
                        }
                    break;
                case 8:
                    for (int i = 6; i < 9; i++)
                        for (int j = 6; j < 9; j++)
                        {
                            numCount[grid[i % 3][j % 3] - 1]++;
                        }
                    break;
            }

            for (int i = 0; i < numCount.Length; i++)
            {
                if (numCount[i] != 1)
                    result = false;
            }
            return result;
        }

        private bool placeNumber(int numToPlace, List<List<int>> currentBox, int posInBox, int boxPosInGrid)
        {
            /** Idea here is that we attempt to place a number at a given position in a grid, in a given box
             * If we are unable to do so, we return false and the caller method will call this method again,
             * with either a different posInBox or a different numToPlace **/
            int row = getRowFromBoxPos(boxPosInGrid, posInBox);
            int col = getColFromBoxPos(boxPosInGrid, posInBox);

            bool result = true;

            if (!validPlacement(col, row, numToPlace, currentBox))
                result = false;

            return result;
        }

        private bool validPlacement(int col, int row, int searchNumber, List<List<int>> box)
        {
            // Scan along col and row to see if searchNumber exists
            // Return false if it does, else true
            bool result = true;

            // First check that we are in an empty box
            if (box[row % 3][col % 3] != 0)
            {
                result = false;
                return result;
            }

            for (int i = 0; i < 9; i++)
            {
                if (grid[row][i] == searchNumber)
                {
                    result = false;
                    break;
                }
                if (grid[i][col] == searchNumber)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }        

        private void addBoxWeHavebeenWorkingOnToGrid(List<List<int>> boxWeHaveBeenWorkingOn, int boxPos)
        {
            switch (boxPos)
            {
                case 1:
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                            grid[i][j] = boxWeHaveBeenWorkingOn[i][j];
                    break;
                case 2:
                    for (int i = 0; i < 3; i++)
                        for (int j = 3; j < 6; j++)
                            grid[i][j] = boxWeHaveBeenWorkingOn[i][j - 3];
                    break;
                case 3:
                    for (int i = 0; i < 3; i++)
                        for (int j = 6; j < 9; j++)
                            grid[i][j] = boxWeHaveBeenWorkingOn[i][j - 6];
                    break;
                case 4:
                    for (int i = 3; i < 6; i++)
                        for (int j = 0; j < 3; j++)
                            grid[i][j] = boxWeHaveBeenWorkingOn[i - 3][j];
                    break;
                case 5:
                    for (int i = 3; i < 6; i++)
                        for (int j = 3; j < 6; j++)
                            grid[i][j] = boxWeHaveBeenWorkingOn[i - 3][j - 3];
                    break;
                case 6:
                    for (int i = 3; i < 6; i++)
                        for (int j = 6; j < 9; j++)
                            grid[i][j] = boxWeHaveBeenWorkingOn[i - 3][j - 6];
                    break;
                case 7:
                    for (int i = 6; i < 9; i++)
                        for (int j = 0; j < 3; j++)
                            grid[i][j] = boxWeHaveBeenWorkingOn[i - 6][j];
                    break;
                case 8:
                    for (int i = 6; i < 9; i++)
                        for (int j = 3; j < 6; j++)
                            grid[i][j] = boxWeHaveBeenWorkingOn[i - 6][j - 3];
                    break;
                case 9:
                    for (int i = 6; i < 9; i++)
                        for (int j = 6; j < 9; j++)
                            grid[i][j] = boxWeHaveBeenWorkingOn[i - 6][j - 6];
                    break;
            } // switch
        }

        public void showNumbersToScreen(PlayWindow window, int boxPos)
        {
            string concatName = "";
            switch (boxPos)
            {
                case 0:
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            concatName = i + "" + j;
                            Control[] controls = window.Controls.Find(concatName, true);
                            if (controls.Length == 1)
                            {
                                Button b = controls[0] as Button;
                                b.Enabled = false;
                                b.Text = grid[i][j].ToString();
                            }
                        }
                    }
                    break;
                case 1:
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 3; j < 6; j++)
                        {
                            concatName = i + "" + j;
                            Control[] controls = window.Controls.Find(concatName, true);
                            if (controls.Length == 1)
                            {
                                Button b = controls[0] as Button;
                                b.Enabled = false;
                                b.Text = grid[i][j].ToString();
                            }
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 6; j < 9; j++)
                        {
                            concatName = i + "" + j;
                            Control[] controls = window.Controls.Find(concatName, true);
                            if (controls.Length == 1)
                            {
                                Button b = controls[0] as Button;
                                b.Enabled = false;
                                b.Text = grid[i][j].ToString();
                            }
                        }
                    }
                    break;
                case 3:
                    for (int i = 3; i < 6; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            concatName = i + "" + j;
                            Control[] controls = window.Controls.Find(concatName, true);
                            if (controls.Length == 1)
                            {
                                Button b = controls[0] as Button;
                                b.Enabled = false;
                                b.Text = grid[i][j].ToString();
                            }
                        }
                    }
                    break;
                case 4:
                    for (int i = 3; i < 6; i++)
                    {
                        for (int j = 3; j < 6; j++)
                        {
                            concatName = i + "" + j;
                            Control[] controls = window.Controls.Find(concatName, true);
                            if (controls.Length == 1)
                            {
                                Button b = controls[0] as Button;
                                b.Enabled = false;
                                b.Text = grid[i][j].ToString();
                            }
                        }
                    }
                    break;
                case 5:
                    for (int i = 3; i < 6; i++)
                    {
                        for (int j = 6; j < 9; j++)
                        {
                            concatName = i + "" + j;
                            Control[] controls = window.Controls.Find(concatName, true);
                            if (controls.Length == 1)
                            {
                                Button b = controls[0] as Button;
                                b.Enabled = false;
                                b.Text = grid[i][j].ToString();
                            }
                        }
                    }
                    break;
                case 6:
                    for (int i = 6; i < 9; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            concatName = i + "" + j;
                            Control[] controls = window.Controls.Find(concatName, true);
                            if (controls.Length == 1)
                            {
                                Button b = controls[0] as Button;
                                b.Enabled = false;
                                b.Text = grid[i][j].ToString();
                            }
                        }
                    }
                    break;
                case 7:
                    for (int i = 6; i < 9; i++)
                    {
                        for (int j = 3; j < 6; j++)
                        {
                            concatName = i + "" + j;
                            Control[] controls = window.Controls.Find(concatName, true);
                            if (controls.Length == 1)
                            {
                                Button b = controls[0] as Button;
                                b.Enabled = false;
                                b.Text = grid[i][j].ToString();
                            }
                        }
                    }
                    break;
                case 8:
                    for (int i = 6; i < 9; i++)
                    {
                        for (int j = 6; j < 9; j++)
                        {
                            concatName = i + "" + j;
                            Control[] controls = window.Controls.Find(concatName, true);
                            if (controls.Length == 1)
                            {
                                Button b = controls[0] as Button;
                                b.Enabled = false;
                                b.Text = grid[i][j].ToString();
                            }
                        }
                    }
                    break;
            }
        }

        // Retrieve a list (9 entries long) of all the entries for that particular box
        private List<int> retrieveBoxEntries(int boxPos)
        {
            List<int> boxBuilder = new List<int>();
            switch (boxPos)
            {
                case 1:
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                            boxBuilder.Add(grid[i][j]);
                    break;
                case 2:
                    for (int i = 3; i < 6; i++)
                        for (int j = 0; j < 3; j++)
                            boxBuilder.Add(grid[i][j]);
                    break;
                case 3:
                    for (int i = 6; i < 9; i++)
                        for (int j = 0; j < 3; j++)
                            boxBuilder.Add(grid[i][j]);
                    break;
                case 4:
                    for (int i = 0; i < 3; i++)
                        for (int j = 3; j < 6; j++)
                            boxBuilder.Add(grid[i][j]);
                    break;
                case 5:
                    for (int i = 3; i < 6; i++)
                        for (int j = 3; j < 6; j++)
                            boxBuilder.Add(grid[i][j]);
                    break;
                case 6:
                    for (int i = 6; i < 9; i++)
                        for (int j = 3; j < 6; j++)
                            boxBuilder.Add(grid[i][j]);
                    break;
                case 7:
                    for (int i = 0; i < 3; i++)
                        for (int j = 6; j < 9; j++)
                            boxBuilder.Add(grid[i][j]);
                    break;
                case 8:
                    for (int i = 3; i < 6; i++)
                        for (int j = 6; j < 9; j++)
                            boxBuilder.Add(grid[i][j]);
                    break;
                case 9:
                    for (int i = 6; i < 9; i++)
                        for (int j = 6; j < 9; j++)
                            boxBuilder.Add(grid[i][j]);
                    break;
            } // switch
            return boxBuilder;
        }

        private int getRowFromBoxPos(int thisBoxLocation, int positionInBox)
        {
            int rowResult = 0;
            if (thisBoxLocation == 3 || thisBoxLocation == 4 || thisBoxLocation == 5)
                rowResult += 3;
            if (thisBoxLocation == 6 || thisBoxLocation == 7 || thisBoxLocation == 8)
                rowResult += 6;
            if (positionInBox == 3 || positionInBox == 4 || positionInBox == 5)
                rowResult += 1;
            if (positionInBox == 6 || positionInBox == 7 || positionInBox == 8)
                rowResult += 2;
            return rowResult;
        }

        private int getColFromBoxPos(int thisBoxLocation, int positionInBox)
        {
            int colResult = 0;
            if (thisBoxLocation == 1 || thisBoxLocation == 4 || thisBoxLocation == 7)
                colResult += 3;
            if (thisBoxLocation == 2 || thisBoxLocation == 5 || thisBoxLocation == 8)
                colResult += 6;
            if (positionInBox == 1 || positionInBox == 4 || positionInBox == 7)
                colResult += 1;
            if (positionInBox == 2 || positionInBox == 5 || positionInBox == 8)
                colResult += 2;
            return colResult;
        }             

        public void printGrid()
        {
            string result = "";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result += grid[i][j] + " ";
                }
                result += "\n";
            }
            MessageBox.Show(result);

        }
    }
}
