using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public static SortableBindingList<Flight> iFlights = new SortableBindingList<Flight>(Flights);
        public static WaitForm _WaitForm = new WaitForm();

        public Form1()
        {
            InitializeComponent();
        }
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
        // when application is loading
        private void Form1_OnLoad(object sender, EventArgs e)
        {
            // start time measurement
            var now = DateTime.Now;

            // waitform is using the same thread at the moment
            using (WaitForm _waitForm = new WaitForm())
            {
                _waitForm.Show();
                _waitForm.Update();

                // because of EASA logging rules
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

                // this makes scrolling through the datagridview much faster, but can slowdown in a terminal session
                if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
                {
                    Type dgvType = dataGridView1.GetType();
                    PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                    pi.SetValue(dataGridView1, true, null);
                }

                // prepare the DataGridView
                // to make behaviour the same as with mono, the "newrow" has caused too many problems
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AutoGenerateColumns = false;

                iFlights.AllowEdit = true;
                dataGridView1.DataSource = iFlights;

                dataGridView1.Columns[2].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[4].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[7].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[8].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[9].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[10].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[14].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[15].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[16].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[17].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[18].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[19].DefaultCellStyle.Format = "hh\\:mm";
                dataGridView1.Columns[22].DefaultCellStyle.Format = "hh\\:mm";

                // workaround for mono-framework
                if (IsRunningOnMono())
                {
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                }

                LoadDB();

                // if database empty or not existing
                if (dataGridView1.Rows.Count < 1)
                {
                    Flights.Add(new Flight(DateTime.MinValue, TimeSpan.Zero, "", TimeSpan.Zero, "", "", "", TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, DateTime.MinValue, "", TimeSpan.Zero, "", false));
                    iFlights.ResetBindings();   // refresh the ibindinglist
                    dataGridView1.Refresh();
                }

                // scroll down to last row
                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
                }

                toolStripStatusLabel1.Text = "finished loading, it took: " + Math.Round((DateTime.Now.Subtract(now).TotalSeconds)).ToString() + " second(s).";
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
                    //Flights.Sort();
                    iFlights.Sort("FlightDate", ListSortDirection.Ascending);
                    iFlights.ResetBindings();
                    MarkAllCellsEditable();
                    //dataGridView1.Refresh();
                }
                catch (Exception exc)
                {
                    File.AppendAllText("_easa_errorlog.txt", DateTime.Now.ToString() + " LoadDB:\n" + exc.ToString() + "\n");
                }
            }
        }
        // save the table via EASA export filter
        private void SaveTable()
        {
            try
            {
                iFlights.Sort("FlightDate", ListSortDirection.Ascending);
                List<Flight> temp = iFlights.GetFlights();
                Export_EASA_CSV export = new Export_EASA_CSV(temp);
                File.WriteAllText("EASALogbook.csv", export.GetCSV());
            }
            catch (Exception e)
            {
                File.AppendAllText("_easa_errorlog.txt", DateTime.Now.ToString() + " SaveTable_3:\n" + e.ToString() + "\n");
            }
        }
        // this is a workaround because of a M$ bug?
        private void MarkAllCellsEditable()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                foreach (DataGridViewCell cell in dataGridView1.Rows[i].Cells)
                {
                    cell.ReadOnly = false;
                }
            }
        }

        // + button
        private void Button1_Click(object sender, EventArgs e)
        {
            // ibindinglist does not accept null value Flight
            Flights.Add(new Flight(DateTime.MinValue, null, "", null, "", "", "", TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, DateTime.MinValue, "", TimeSpan.Zero, "", false));
            iFlights.ResetBindings();   // refresh the ibindinglist
            dataGridView1.Refresh();    // refresh the datagridview
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0];
            }
        }
        // - button
        private void Button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int row = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Index;
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                iFlights.ResetBindings();

                if (dataGridView1.RowCount > 0 && row > 0)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[row - 1].Cells[0];
                    dataGridView1.Rows[row - 1].Selected = true;
                }
            }
        }
        // save button
        private void Button5_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "saving...";
            button5.Enabled = false;
            Form1.ActiveForm.Enabled = false;
            SaveTable();
            toolStripStatusLabel1.Text = "";
            Form1.ActiveForm.Enabled = true;
            button5.Enabled = true;
        }
        // end button
        private void Button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        // new database from menu
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Flights.Clear();
            iFlights.ResetBindings();
        }
        // save from menu
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "saving...";
            saveToolStripMenuItem.Enabled = false;
            Form1.ActiveForm.Enabled = false;
            SaveTable();
            toolStripStatusLabel1.Text = "";
            Form1.ActiveForm.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
        }
        // exit from menu
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        // move all if form is resized
        private void Form1_OnResize(object sender, EventArgs e)
        {
            // nullcheck for mono-framework
            if (Form1.ActiveForm != null)
            {
                dataGridView1.Width = Form1.ActiveForm.Width - 40;
                dataGridView1.Height = Form1.ActiveForm.Height - 119;
                button5.Left = Form1.ActiveForm.Width - 184;
                button6.Left = Form1.ActiveForm.Width - 103;
                button1.Top = Form1.ActiveForm.Height - 87;
                button2.Top = Form1.ActiveForm.Height - 87;
                button5.Top = Form1.ActiveForm.Height - 87;
                button6.Top = Form1.ActiveForm.Height - 87;
            }
        }
        // before exiting app ask if user wants to save
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("do you want to save before closing?", "good bye", MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
            {
                SaveTable();
            }
        }


        // finished editing -> see if other cells can be filled with value
        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            AutoFillCellValue(e.RowIndex, e.ColumnIndex);
        }
        // finished editing -> see if other cells can be filled with value
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AutoFillCellValue(e.RowIndex, e.ColumnIndex);
        }
        // finished editing -> see if other cells can be filled with value
        private void DataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            AutoFillCellValue(e.RowIndex, e.ColumnIndex);
        }

        // auto calculate totalflighttime
        // mono implementation is missing
        private void AutoFillCellValue(int rowIndex, int columnIndex)
        {
            try
            {
                if (dataGridView1.Rows[rowIndex].Cells[0] != null && dataGridView1.Rows[rowIndex].Cells[2] != null && dataGridView1.Rows[rowIndex].Cells[4] != null)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[0].Value != null && dataGridView1.Rows[rowIndex].Cells[2].Value != null && dataGridView1.Rows[rowIndex].Cells[4].Value != null)
                    {
                        TimeSpan begin = (TimeSpan)dataGridView1.Rows[rowIndex].Cells[2].Value;
                        TimeSpan end = (TimeSpan)dataGridView1.Rows[rowIndex].Cells[4].Value;
                        if (end.Ticks < begin.Ticks)
                        {
                            end = end.Add(TimeSpan.FromHours(24));
                        }
                        dataGridView1.Rows[rowIndex].Cells[10].Value = end.Subtract(begin);
                        dataGridView1.Refresh();
                    }
                }
            }
            catch (Exception e)
            {
                File.AppendAllText("_easa_errorlog.txt", DateTime.Now.ToString() + " autoFillCellValue: " + rowIndex + "x" + columnIndex + "\n" + e.ToString() + "\n");
            }
        }
        // on doubleclick we try to fill the cell with value
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            PopulateDataOnClick(e.RowIndex, e.ColumnIndex);
            AutoFillCellValue(e.RowIndex, e.ColumnIndex);
        }
        // to easily populate with data
        // mono implementation missing
        private void PopulateDataOnClick(int rowIndex, int columnIndex)
        {
            try
            {
                if (dataGridView1.Rows[rowIndex].Cells[columnIndex] != null)
                {
                    if (columnIndex == 0)
                    {
                        if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                        {
                            dataGridView1.Rows[rowIndex].Cells[0].Value = DateTime.Now;
                            dataGridView1.EndEdit();
                        }
                    }
                    if (columnIndex == 2 || columnIndex == 4)
                    {
                        if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                        {
                            dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                            dataGridView1.EndEdit();
                        }
                    }
                    if (columnIndex == 7 || columnIndex == 8 || columnIndex == 9 || columnIndex == 14 || columnIndex == 15 || columnIndex == 16 || columnIndex == 17 || columnIndex == 18 || columnIndex == 19)
                    {
                        if ((dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null) && (dataGridView1.Rows[rowIndex].Cells[10] != null))
                        {
                            dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = dataGridView1.Rows[rowIndex].Cells[10].Value;
                            dataGridView1.EndEdit();
                        }
                        else
                        {
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value != null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = null;
                            }
                        }
                    }
                    if (columnIndex == 12 || columnIndex == 13)
                    {
                        if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                        {
                            dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = "1";
                            dataGridView1.EndEdit();
                        }
                        else
                        {
                            dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = "";
                            dataGridView1.EndEdit();
                        }
                    }
                    if (columnIndex == 20)
                    {
                        if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                        {
                            dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = DateTime.Now;
                            dataGridView1.EndEdit();
                        }
                    }
                    if (columnIndex == 22)
                    {
                        if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                        {
                            dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = new TimeSpan(4, 0, 0);
                            dataGridView1.EndEdit();
                        }
                        else
                        {
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value != null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString().Length < 1)
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = null;
                                dataGridView1.EndEdit();
                            }
                        }
                    }
                }

            }
            catch (Exception y)
            {
                File.AppendAllText("_easa_errorlog.txt", DateTime.Now.ToString() + " populateDataOnClick: " + rowIndex + "x" + columnIndex + "\n" + y.ToString() + "\n");
            }
        }

        // restrict edit on certain cells
        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Cell_KeyPress_Allow_Digits_and_Separators);
            e.Control.KeyPress -= new KeyPressEventHandler(Cell_KeyPress_Allow_Digits_only);

            // cells with digits and separators
            if (dataGridView1.CurrentCell.ColumnIndex == 0 || dataGridView1.CurrentCell.ColumnIndex == 2 || dataGridView1.CurrentCell.ColumnIndex == 4 || dataGridView1.CurrentCell.ColumnIndex == 7 || dataGridView1.CurrentCell.ColumnIndex == 8 || dataGridView1.CurrentCell.ColumnIndex == 9 || dataGridView1.CurrentCell.ColumnIndex == 10 || dataGridView1.CurrentCell.ColumnIndex == 14 || dataGridView1.CurrentCell.ColumnIndex == 15 || dataGridView1.CurrentCell.ColumnIndex == 16 || dataGridView1.CurrentCell.ColumnIndex == 17 || dataGridView1.CurrentCell.ColumnIndex == 18 || dataGridView1.CurrentCell.ColumnIndex == 19 || dataGridView1.CurrentCell.ColumnIndex == 20 || dataGridView1.CurrentCell.ColumnIndex == 21 || dataGridView1.CurrentCell.ColumnIndex == 22)
            {
                if (e.Control is TextBox tb)
                {
                    tb.KeyPress += new KeyPressEventHandler(Cell_KeyPress_Allow_Digits_and_Separators);
                }
            }
            // cells with only digits
            if (dataGridView1.CurrentCell.ColumnIndex == 12 || dataGridView1.CurrentCell.ColumnIndex == 13)
            {
                if (e.Control is TextBox tb)
                {
                    tb.KeyPress += new KeyPressEventHandler(Cell_KeyPress_Allow_Digits_only);
                }
            }
        }
        // only allow digits and separators
        private void Cell_KeyPress_Allow_Digits_and_Separators(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ':' && e.KeyChar != '/')
            {
                e.Handled = true;
            }
        }
        // only allow digits
        private void Cell_KeyPress_Allow_Digits_only(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // this paints a number on the columnheader of each row
        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
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
        // summarize flight times
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
                multiPilotFlightTime = multiPilotFlightTime.Add(a.MultiPilotTime.Value);
                totalFligtTime = totalFligtTime.Add(a.TotalTimeOfFlight.Value);
                dayLdgs += a.DayLandings.Value;
                nightLdgs += a.NightLandings.Value;
                nightTime = nightTime.Add(a.NightTime.Value);
                ifrTime = ifrTime.Add(a.IFRTime.Value);
                PICTime = PICTime.Add(a.PICTime.Value);
                CopiTime = CopiTime.Add(a.CopilotTime.Value);
                DualTime = DualTime.Add(a.DualTime.Value);
                InstructorTime = InstructorTime.Add(a.InstructorTime.Value);
                SimTime = SimTime.Add(a.SimTime.Value);
            }

            return "∑:   multiPilot: " + ((int)multiPilotFlightTime.TotalHours).ToString() + ":" + multiPilotFlightTime.Minutes.ToString() + "   total: " + ((int)totalFligtTime.TotalHours).ToString() + ":" + totalFligtTime.Minutes.ToString() + "   DayLDG: " + dayLdgs.ToString() + "   NightLDG: " + nightLdgs.ToString() + "   Night: " + ((int)nightTime.TotalHours).ToString() + ":" + nightTime.Minutes.ToString() + "   IFR: " + ((int)ifrTime.TotalHours).ToString() + ":" + ifrTime.Minutes.ToString() + "   PIC: " + ((int)PICTime.TotalHours).ToString() + ":" + PICTime.Minutes.ToString() + "   Copi: " + ((int)CopiTime.TotalHours).ToString() + ":" + CopiTime.Minutes.ToString() + "   Dual: " + ((int)DualTime.TotalHours).ToString() + ":" + DualTime.Minutes.ToString() + "   Instructor: " + ((int)InstructorTime.TotalHours).ToString() + ":" + InstructorTime.Minutes.ToString() + "   Sim: " + ((int)SimTime.TotalHours).ToString() + ":" + SimTime.Minutes.ToString();

        }
        // allows the user to only enter some digits and become a timespan
        private void DataGridView1_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e != null)
            {
                if (e.Value != null)
                {
                    try
                    {
                        if (dataGridView1.CurrentCell.ColumnIndex == 2 || dataGridView1.CurrentCell.ColumnIndex == 4 || dataGridView1.CurrentCell.ColumnIndex == 7 || dataGridView1.CurrentCell.ColumnIndex == 8 || dataGridView1.CurrentCell.ColumnIndex == 9 || dataGridView1.CurrentCell.ColumnIndex == 10 || dataGridView1.CurrentCell.ColumnIndex == 14 || dataGridView1.CurrentCell.ColumnIndex == 15 || dataGridView1.CurrentCell.ColumnIndex == 16 || dataGridView1.CurrentCell.ColumnIndex == 17 || dataGridView1.CurrentCell.ColumnIndex == 18 || dataGridView1.CurrentCell.ColumnIndex == 19 || dataGridView1.CurrentCell.ColumnIndex == 22)
                        {
                            if (e.Value.ToString().Length > 3 && e.Value.ToString().Length < 5 && !e.Value.ToString().Contains(":"))
                            {
                                e.Value = TimeSpan.Parse(e.Value.ToString().Substring(0, 2) + ":" + e.Value.ToString().Substring(2, 2));
                                e.ParsingApplied = true;
                            }
                        }
                    }
                    catch (FormatException)
                    {
                        e.ParsingApplied = false;
                    }
                }
            }
        }
    }
}
