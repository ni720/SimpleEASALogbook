﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SimpleEASALogbook
{
    public partial class Form1 : Form
    {
        public static List<Flight> Flights = new List<Flight>();

        public Form1()
        {
            InitializeComponent();
        }
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        // add rows for now.. but should be detail view add of rows
        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
        }

        // save button
        private void button5_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "saving...";
            button5.Enabled = false;
            Form1.ActiveForm.Enabled = false;
            SaveTable();
            toolStripStatusLabel1.Text = "";
            Form1.ActiveForm.Enabled = true;
            button5.Enabled = true;
        }

        private void eASALogToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Form1_OnLoad(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");


            // this makes scrolling through the datagridview much faster, but can slowdown in a terminal session
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type dgvType = dataGridView1.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dataGridView1, true, null);
            }
                       
            // to make behaviour the same as with mono, the "newrow" has caused too many problems
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;

            // workaround for mono-framework
            if (IsRunningOnMono())
            {
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            }

            // loading takes place in the form_shown method to show the user, there is loading in progress

        }
        private void Form1_OnResize(object sender, EventArgs e)
        {
            // nullcheck for mono-framework
            if (Form1.ActiveForm != null)
            {
                dataGridView1.Width = Form1.ActiveForm.Width - 40;
                dataGridView1.Height = Form1.ActiveForm.Height - 141;
                button5.Left = Form1.ActiveForm.Width - 184;
                button6.Left = Form1.ActiveForm.Width - 103;
                label1.Width = Form1.ActiveForm.Width - 40;
                label1.Top = Form1.ActiveForm.Height - 112;
                button1.Top = Form1.ActiveForm.Height - 87;
                button2.Top = Form1.ActiveForm.Height - 87;
                button3.Top = Form1.ActiveForm.Height - 87;
                button4.Top = Form1.ActiveForm.Height - 87;
                button5.Top = Form1.ActiveForm.Height - 87;
                button6.Top = Form1.ActiveForm.Height - 87;
            }
        }

        // load table from "database" file
        private void LoadDB()
        {
            if (File.Exists("EASALogbook.csv"))
            {
                try
                {
                    Import_EASA_CSV import = new Import_EASA_CSV(File.ReadAllText("EASALogbook.csv").ToString());
                    Flights.AddRange(import.getFlightList());

                    foreach (Flight flight in Flights)
                    {
                        dataGridView1.Rows.Add(flight.getDateString(), flight.getDepartureString(), flight.getOffBlockTimeString(), flight.getDestinationString(), flight.getOnBlockTimeString(), flight.getTypeOfAircraftString(), flight.getRegistrationString(), flight.getSEPTimeString(), flight.getMEPTimeString(), flight.getMultiPilotTimeString(), flight.getTotalTimeString(), flight.getPICNameString(), flight.getDayLDGString(), flight.getNightLDGString(), flight.getNightTimeString(), flight.getIFRTimeString(), flight.getPICTimeString(), flight.getCopilotTimeString(), flight.getDualTimeString(), flight.getInstructorTimeString(), flight.getSimDateString(), flight.getSimTypeString(), flight.getSimTimeString(), flight.getRemarksString());

                        if (flight.hasPageBreak())
                        {
                            dataGridView1.Rows.Add(dataGridView1.Rows.Count.ToString(), "pagebreak", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*");
                        }

                    }
                }
                catch (Exception exc)
                {
                    File.AppendAllText(".logfile", DateTime.Now.ToString() + " LoadDB:\n" + exc.ToString() + "\n");
                }
            }
        }

        // save the table by parsing it into flights and writing the flights. all via EASA import&export filter
        private void SaveTable()
        {
            Flights = new List<Flight>();
            string stringBuilder = "";
            int i = 0, j = 0;
            try
            {
                for (i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        if (dataGridView1.Rows[i].Cells[j].Value == null)
                        {
                            stringBuilder += ";";
                        }
                        else
                        {
                            stringBuilder += dataGridView1.Rows[i].Cells[j].Value.ToString() + ";";
                        }
                    }

                    if (dataGridView1.Rows[i + 1].Cells[1].Value != null)
                    {
                        if (dataGridView1.Rows[i + 1].Cells[1].Value.Equals("pagebreak"))
                        {
                            stringBuilder += ";pagebreak;";
                            i++;
                        }
                    }

                    stringBuilder += "\n";
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(".logfile", DateTime.Now.ToString() + " SaveTable_1:\n" + e.ToString() + "\n");
            }

            try
            {
                Import_EASA_CSV import = new Import_EASA_CSV(stringBuilder);
                Flights.AddRange(import.getFlightList());
            }
            catch (Exception e)
            {
                File.AppendAllText(".logfile", DateTime.Now.ToString() + " SaveTable_2:\n" + e.ToString() + "\n");
            }
            try
            {
                Export_EASA_CSV export = new Export_EASA_CSV(Flights);
                File.WriteAllText("EASALogbook.csv", export.GetCSV());
            }
            catch (Exception e)
            {
                File.AppendAllText(".logfile", DateTime.Now.ToString() + " SaveTable_3:\n" + e.ToString() + "\n");
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            Flights = new List<Flight>();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "saving...";
            saveToolStripMenuItem.Enabled = false;
            Form1.ActiveForm.Enabled = false;
            SaveTable();
            toolStripStatusLabel1.Text = "";
            Form1.ActiveForm.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // shows error icon if input was invalid
        private void validateRows(int rowIndex)
        {
            bool error = false;
            foreach (DataGridViewCell cell in dataGridView1.Rows[rowIndex].Cells)
            {
                if (cell != null)
                {
                    if (cell.ErrorText.Length > 1)
                    {
                        error = true;
                    }
                }
            }
            if (!error)
            {
                dataGridView1.Rows[rowIndex].ErrorText = "";
            }
        }

        // shows error icon if input was invalid
        private void validateCells(int rowIndex, int columnIndex)
        {

            var row = dataGridView1.Rows[rowIndex];
            if (row != null)
            {
                var cell = row.Cells[columnIndex];
                if (cell != null)
                {
                    if (columnIndex == 0 || columnIndex == 20)
                    {
                        object value = cell.Value;
                        if (value != null && !value.Equals(string.Empty))
                        {
                            if (!DateTime.TryParse(value.ToString(), out DateTime dummy))
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].ErrorText = "DateTime must be dd.mm.yyyy";
                                dataGridView1.Rows[rowIndex].ErrorText = "error, entry may not be saved";
                            }
                            else
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].ErrorText = "";
                            }
                        }
                    }
                    if (columnIndex == 2 || columnIndex == 4 || columnIndex == 7 || columnIndex == 8 || columnIndex == 9 || columnIndex == 10 || columnIndex == 14 || columnIndex == 15 || columnIndex == 16 || columnIndex == 17 || columnIndex == 18 || columnIndex == 19 || columnIndex == 22)
                    {
                        object value = cell.Value;
                        if (value != null && !value.Equals(string.Empty))
                        {
                            if (!TimeSpan.TryParse(value.ToString(), out TimeSpan dummy))
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].ErrorText = "Time must be hh:mm";
                            }
                            else
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].ErrorText = "";
                            }
                        }
                    }
                    if (columnIndex == 12 || columnIndex == 13)
                    {
                        object value = cell.Value;
                        if (value != null && !value.Equals(string.Empty))
                        {
                            if (!int.TryParse(value.ToString(), out int dummy))
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].ErrorText = "must be a digit from 1 to 9, may not be saved otherwise";
                            }
                            else
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].ErrorText = "";
                            }
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            validateCells(e.RowIndex, e.ColumnIndex);
            validateRows(e.RowIndex);
            autoFillCellValue(e.RowIndex, e.ColumnIndex);
        }

        // add row
        private void button2_Click(object sender, EventArgs e)
        {

            if (dataGridView1.Rows.Count > 0)
            {
                if (dataGridView1.SelectedRows[0].Index != dataGridView1.NewRowIndex)
                {
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);

                }
                else
                {
                    dataGridView1.Rows[dataGridView1.NewRowIndex].SetValues("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // workaround to make mono double click better
            dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1];
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            validateCells(e.RowIndex, e.ColumnIndex);
            validateRows(e.RowIndex);
            autoFillCellValue(e.RowIndex, e.ColumnIndex);
        }

        // ask if user wants to save
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("do you want to save before closing?", "good bye", MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
            {
                SaveTable();
            }
        }

        private void lHPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void easaLogbookCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            validateCells(e.RowIndex, e.ColumnIndex);
            validateRows(e.RowIndex);
            autoFillCellValue(e.RowIndex, e.ColumnIndex);
        }

        // auto calculate totalflighttime
        private void autoFillCellValue(int rowIndex, int columnIndex)
        {

            try
            {
                if (dataGridView1.Rows[rowIndex].Cells[10].Value == null)
                {
                    if (IsRunningOnMono())
                    {
                        // value.tostring.length > 0 because of mono
                        if (dataGridView1.Rows[rowIndex].Cells[0].Value.ToString().Length > 0 && dataGridView1.Rows[rowIndex].Cells[2].Value.ToString().Length > 0 && dataGridView1.Rows[rowIndex].Cells[4].Value.ToString().Length > 0)
                        {
                            TimeSpan.TryParse(dataGridView1.Rows[rowIndex].Cells[2].Value.ToString(), out TimeSpan begin);
                            TimeSpan.TryParse(dataGridView1.Rows[rowIndex].Cells[4].Value.ToString(), out TimeSpan end);
                            if (end.Ticks < begin.Ticks)
                            {
                                end = end.Add(TimeSpan.FromHours(24));
                            }
                            dataGridView1.Rows[rowIndex].Cells[10].Value = end.Subtract(begin).ToString().Substring(0, 5);
                            dataGridView1.RefreshEdit(); // workaround for mono to display cell values
                        }

                    }
                    else
                    { // value.tostring.length > 0 because of mono
                        if (dataGridView1.Rows[rowIndex].Cells[0].Value != null && dataGridView1.Rows[rowIndex].Cells[2].Value != null && dataGridView1.Rows[rowIndex].Cells[4].Value != null)
                        {
                            TimeSpan.TryParse(dataGridView1.Rows[rowIndex].Cells[2].Value.ToString(), out TimeSpan begin);
                            TimeSpan.TryParse(dataGridView1.Rows[rowIndex].Cells[4].Value.ToString(), out TimeSpan end);
                            if (end.Ticks < begin.Ticks)
                            {
                                end = end.Add(TimeSpan.FromHours(24));
                            }
                            dataGridView1.Rows[rowIndex].Cells[10].Value = end.Subtract(begin).ToString().Substring(0, 5);
                        }

                    }

                }
            }
            catch (Exception e)
            {
                File.AppendAllText(".logfile", DateTime.Now.ToString() + " autoFillCellValue: " + rowIndex + "x" + columnIndex + "\n" + e.ToString() + "\n");
            }

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            populateDataOnClick(e.RowIndex, e.ColumnIndex);
        }

        // to easily populate with data
        private void populateDataOnClick(int rowIndex, int columnIndex)
        {

            // Value.ToString().Length<1 because of mono
            // dataGridView1.RefreshEdit(); // workaround for mono to display cell values because of mono
            try
            {
                if (columnIndex == 0)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                    {
                        dataGridView1.Rows[rowIndex].Cells[0].Value = DateTime.Now.ToShortDateString().Substring(0, 8);
                        dataGridView1.RefreshEdit(); // workaround for mono to display cell values
                        dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[1]; // workaround for mono for better cell doubleclick
                    }
                }
                if (columnIndex == 2)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                    {
                        dataGridView1.Rows[rowIndex].Cells[2].Value = DateTime.Now.ToShortTimeString();
                        dataGridView1.RefreshEdit(); // workaround for mono to display cell values
                        dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[1]; // workaround for mono for better cell doubleclick
                    }
                }
                if (columnIndex == 4)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                    {
                        dataGridView1.Rows[rowIndex].Cells[4].Value = DateTime.Now.ToShortTimeString();
                        dataGridView1.RefreshEdit(); // workaround for mono to display cell values
                        dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[1]; // workaround for mono for better cell doubleclick
                    }
                }
                if (columnIndex == 7 || columnIndex == 8 || columnIndex == 9 || columnIndex == 14 || columnIndex == 15 || columnIndex == 16 || columnIndex == 17 || columnIndex == 18 || columnIndex == 19)
                {
                    if ((dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1) && (dataGridView1.Rows[rowIndex].Cells[10].Value != null || dataGridView1.Rows[rowIndex].Cells[10].Value.ToString().Length > 0) && dataGridView1.Rows[rowIndex].Cells[10].ErrorText.Length < 1)
                    {
                        dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = dataGridView1.Rows[rowIndex].Cells[10].Value;
                        dataGridView1.RefreshEdit(); // workaround for mono to display cell values
                        dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[1]; // workaround for mono for better cell doubleclick
                    }
                    else
                    {
                        if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value != null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                        {
                            dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = null;
                            dataGridView1.RefreshEdit(); // workaround for mono to display cell values
                            dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[1]; // workaround for mono for better cell doubleclick
                        }
                    }
                }
                if (columnIndex == 12 || columnIndex == 13)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                    {
                        dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = "1";
                        dataGridView1.RefreshEdit(); // workaround for mono to display cell values
                        dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[1]; // workaround for mono for better cell doubleclick
                    }
                    else
                    {
                        dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = "";
                        dataGridView1.RefreshEdit(); // workaround for mono to display cell values
                        dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[1]; // workaround for mono for better cell doubleclick
                    }
                }
                if (columnIndex == 20)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                    {
                        dataGridView1.Rows[rowIndex].Cells[20].Value = DateTime.Now.ToShortDateString().Substring(0, 8);
                        dataGridView1.RefreshEdit(); // workaround for mono to display cell values
                        dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[1]; // workaround for mono for better cell doubleclick
                    }
                }
            }
            catch (Exception y)
            {
                File.AppendAllText(".logfile", DateTime.Now.ToString() + " populateDataOnClick: " + rowIndex + "x" + columnIndex + "\n" + y.ToString() + "\n");
            }
        }

        // includes offblocktime into date to compare date cells
        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            // Try to sort based on the cells in the current column.
            e.SortResult = System.String.Compare(
                e.CellValue1.ToString(), e.CellValue2.ToString());

            // If the cells are equal, sort based on the ID column.
            if (e.SortResult == 0 && e.Column.Index == 0)
            {
                e.SortResult = System.String.Compare(
                    dataGridView1.Rows[e.RowIndex1].Cells[2].Value.ToString(),
                    dataGridView1.Rows[e.RowIndex2].Cells[2].Value.ToString());
            }
            e.Handled = true;
        }

        private void brusselsPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        // todo add : and /
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dataGridView1.CurrentCell.ColumnIndex == 0) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);

                }
            }
        }

        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ':' && e.KeyChar != '/')
            {
                e.Handled = true;
            }
        }

        private void menuStrip1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {

        }


        // this paints a number on the columnheader of each row
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new System.Drawing.StringFormat()
            {
                Alignment = System.Drawing.StringAlignment.Center,
                LineAlignment = System.Drawing.StringAlignment.Center
            };

            var headerBounds = new System.Drawing.Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, System.Drawing.SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            var now = DateTime.Now;

            LoadDB();

            if (dataGridView1.Rows.Count < 1)
            {
                dataGridView1.Rows.Add();
            }

            label1.Text=Summarize(Flights);

            toolStripStatusLabel1.Text = "finished loading, it took: " + Math.Round((DateTime.Now.Subtract(now).TotalSeconds)).ToString() + " second(s).";

        }

        static string Summarize(List<Flight> flights)
        {
            TimeSpan multiPilotFlightTime = new TimeSpan(0);
            TimeSpan totalFligtTime = new TimeSpan(0);
            int dayLdgs = 0;
            int nightLdgs = 0;
            TimeSpan nightTime = new TimeSpan();
            TimeSpan ifrTime = new TimeSpan();
            TimeSpan PICTime = new TimeSpan();
            TimeSpan CopiTime = new TimeSpan();
            TimeSpan DualTime = new TimeSpan();
            TimeSpan InstructorTime = new TimeSpan();
            TimeSpan SimTime = new TimeSpan();
            foreach (Flight a in flights)
            {
                multiPilotFlightTime = multiPilotFlightTime.Add(a.getMultiPilotTime);
                totalFligtTime = totalFligtTime.Add(a.getTotalTimeOfFlight);
                dayLdgs += a.getDayLandings;
                nightLdgs += a.getNightLandings;
                nightTime = nightTime.Add(a.getNightTime);
                ifrTime = ifrTime.Add(a.getIFRTime);
                PICTime = PICTime.Add(a.getPICTime);
                CopiTime = CopiTime.Add(a.getCopilotTime);
                DualTime = DualTime.Add(a.getDualTime);
                InstructorTime = InstructorTime.Add(a.getInstructorTime);
                SimTime = SimTime.Add(a.getSimTime);
            }
            
            return "∑:   multiPilot: " + ((int)multiPilotFlightTime.TotalHours).ToString() + ":" + multiPilotFlightTime.Minutes.ToString()  +"   total: " + ((int)totalFligtTime.TotalHours).ToString() + ":" + totalFligtTime.Minutes.ToString() + "   DayLDG: " + dayLdgs.ToString() + "   NightLDG: " + nightLdgs.ToString() + "   Night: " + ((int)nightTime.TotalHours).ToString() + ":" + nightTime.Minutes.ToString() + "   IFR: " + ((int)ifrTime.TotalHours).ToString() + ":" + ifrTime.Minutes.ToString() + "   PIC: " + ((int)PICTime.TotalHours).ToString() + ":" + PICTime.Minutes.ToString() + "   Copi: " + ((int)CopiTime.TotalHours).ToString() + ":" + CopiTime.Minutes.ToString() + "   Dual: " + ((int)DualTime.TotalHours).ToString() + ":" + DualTime.Minutes.ToString() + "   Instructor: " + ((int)InstructorTime.TotalHours).ToString() + ":" + InstructorTime.Minutes.ToString() + "   Sim: " + ((int)SimTime.TotalHours).ToString() + ":" + SimTime.Minutes.ToString();

        }
    }
}
