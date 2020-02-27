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
        public static bool isMono = false;

        public Form1()
        {
            InitializeComponent();
        }
        private static void IsRunningOnMono()
        {
            if (Type.GetType("Mono.Runtime") != null)
            {
                isMono = true;
            }
        }
        // when application is loading
        private void Form1_OnLoad(object sender, EventArgs e)
        {
            // start time measurement
            var now = DateTime.Now;


            // check if running mono
            IsRunningOnMono();

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

                dataGridView1.Columns[2].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[4].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[7].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[8].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[9].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[10].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[14].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[15].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[16].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[17].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[18].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[19].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[22].DefaultCellStyle.Format = "%h\\:mm";

                // workaround for mono-framework
                if (isMono)
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

                // ignore the old sum-row
                for (int i = 0; i < Flights.Count; i++)
                {
                    if (Flights[i].FlightDate.HasValue)
                    {
                        if (Flights[i].FlightDate.Value.Year > 9000)
                        {
                            Flights.RemoveAt(i);
                        }
                    }
                }
                iFlights.Add(Summarize(Flights));
                iFlights.Sort("FlightDate", ListSortDirection.Ascending);
                iFlights.ResetBindings();
                dataGridView1.Refresh();

                // scroll down to last row
                if (dataGridView1.Rows.Count > 1)
                {
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 2;
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[0];
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
                    iFlights.Sort("FlightDate", ListSortDirection.Ascending);
                    iFlights.ResetBindings();
                    MarkAllCellsEditable();
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
                // do not save the sum-row
                for (int i = 0; i < Flights.Count; i++)
                {
                    if (Flights[i].FlightDate.HasValue)
                    {
                        if (Flights[i].FlightDate.Value.Year > 9000)
                        {
                            Flights.RemoveAt(i);
                        }
                    }
                }
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
        // this is a workaround because of a M$ bug? anyway if ibindinglist is assigned as datasource editing cells is initially not possible
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
            toolStripStatusLabel1.Text = "   adding row...";
            EnableControls(false);
            if (dataGridView1.CurrentCell.RowIndex >= 0)
            {
                // ibindinglist does not accept null value Flight
                Flights.Insert(dataGridView1.CurrentCell.RowIndex + 1, new Flight(DateTime.MinValue, null, "", null, "", "", "", TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, DateTime.MinValue, "", TimeSpan.Zero, "", false));
            }
            else
            {
                // ibindinglist does not accept null value Flight
                Flights.Add(new Flight(DateTime.MinValue, null, "", null, "", "", "", TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, DateTime.MinValue, "", TimeSpan.Zero, "", false));
            }
            iFlights.ResetBindings();   // refresh the ibindinglist
            dataGridView1.Refresh();    // refresh the datagridview  
            if (dataGridView1.CurrentRow.Index < dataGridView1.RowCount - 1)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index + 1].Cells[dataGridView1.CurrentCell.ColumnIndex];
            }
            EnableControls(true);
            toolStripStatusLabel1.Text = "   row added";
        }
        // - button
        private void Button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.RowCount > 1 && dataGridView1.CurrentRow.Index < dataGridView1.RowCount - 1)
            {
                toolStripStatusLabel1.Text = "   deleting row...";
                EnableControls(false);
                Flights.RemoveAt(dataGridView1.CurrentCell.RowIndex);
                iFlights.ResetBindings();
                dataGridView1.Refresh();
                toolStripStatusLabel1.Text = "   row deleted";
            }
            else
            {
                if (dataGridView1.CurrentRow.Index == dataGridView1.RowCount - 1)
                {
                    toolStripStatusLabel1.Text = "   sum-row can not be deleted!";
                    EnableControls(false);
                    var normal = dataGridView1.DefaultCellStyle.SelectionBackColor;
                    dataGridView1.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightGray;
                    dataGridView1.Refresh();
                    Thread.Sleep(100);
                    dataGridView1.DefaultCellStyle.SelectionBackColor = normal;
                    dataGridView1.Refresh();
                    Thread.Sleep(100);
                    dataGridView1.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightGray;
                    dataGridView1.Refresh();
                    Thread.Sleep(100);
                    dataGridView1.DefaultCellStyle.SelectionBackColor = normal;
                    dataGridView1.Refresh();
                    Thread.Sleep(100);
                    dataGridView1.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightGray;
                    dataGridView1.Refresh();
                    Thread.Sleep(100);
                    dataGridView1.DefaultCellStyle.SelectionBackColor = normal;
                    dataGridView1.Refresh();
                }
            }
            EnableControls(true);
        }
        // save button
        private void Button5_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "   saving...";
            EnableControls(false);
            SaveTable();
            toolStripStatusLabel1.Text = "   saved.";
            EnableControls(true);
        }
        // end button
        private void Button6_Click(object sender, EventArgs e)
        {
            EnableControls(false);
            Application.Exit();
        }
        // new database from menu
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "   resetting database...";
            EnableControls(false);
            Flights.Clear();
            Flights.Add(new Flight(DateTime.MinValue, null, "", null, "", "", "", TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, DateTime.MinValue, "", TimeSpan.Zero, "", false));
            iFlights.ResetBindings();
            iFlights.Add(Summarize(Flights));
            EnableControls(true);
            toolStripStatusLabel1.Text = "   new database!";
        }
        // save from menu
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "   saving...";
            EnableControls(false);
            SaveTable();
            toolStripStatusLabel1.Text = "   saved.";
            EnableControls(true);
        }
        // exit from menu
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableControls(false);
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

        private void EnableControls(bool value)
        {
            dataGridView1.Enabled = value;
            button1.Enabled = value;
            button2.Enabled = value;
            button5.Enabled = value;
            button6.Enabled = value;
            fileToolStripMenuItem.Enabled = value;
            importToolStripMenuItem.Enabled = value;
            exportToolStripMenuItem.Enabled = value;
            helpToolStripMenuItem.Enabled = value;
        }

        // finished editing -> see if other cells can be filled with value
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AutoFillCellValue(e.RowIndex, e.ColumnIndex);
            if (isMono)
            {
                DataGridView1_CellParsing(sender, new DataGridViewCellParsingEventArgs(e.RowIndex, e.ColumnIndex, dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, typeof(TimeSpan), dataGridView1.DefaultCellStyle));
            }
        }

        // auto calculate totalflighttime
        // mono implementation is missing
        private void AutoFillCellValue(int rowIndex, int columnIndex)
        {
            //  filter out columnheader clicks
            if (rowIndex < 0 || columnIndex < 0)
            {
                return;
            }
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
            if (!isMono)
            {
                PopulateDataOnClick(e.RowIndex, e.ColumnIndex);
                AutoFillCellValue(e.RowIndex, e.ColumnIndex);
            }
        }
        // mono on singleclick we try to fill the cell with value
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (isMono)
            {
                PopulateDataOnClick(e.RowIndex, e.ColumnIndex);
                AutoFillCellValue(e.RowIndex, e.ColumnIndex);
            }
        }

        // to easily populate with data
        private void PopulateDataOnClick(int rowIndex, int columnIndex)
        {
            //  filter out columnheader clicks
            if (rowIndex < 0 || columnIndex < 0)
            {
                return;
            }
            try
            {
                if (isMono)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[columnIndex] != null)
                    {
                        if (columnIndex == 0)
                        {
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null)
                            {
                                dataGridView1.Rows[rowIndex].Cells[0].Value = DateTime.Now;
                                dataGridView1.RefreshEdit();
                            }
                        }
                        if (columnIndex == 2 || columnIndex == 4)
                        {
                            dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                            dataGridView1.RefreshEdit();
                        }
                        if (columnIndex == 7 || columnIndex == 8 || columnIndex == 9 || columnIndex == 14 || columnIndex == 15 || columnIndex == 16 || columnIndex == 17 || columnIndex == 18 || columnIndex == 19)
                        {
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null && dataGridView1.Rows[rowIndex].Cells[10].Value != null)
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = dataGridView1.Rows[rowIndex].Cells[10].Value;
                                dataGridView1.RefreshEdit();
                            }
                            else
                            {
                                if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value != null)
                                {
                                    dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = null;
                                    dataGridView1.RefreshEdit();
                                }
                            }
                        }
                        if (columnIndex == 12 || columnIndex == 13)
                        {
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null)
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = 1;
                                dataGridView1.RefreshEdit();
                            }
                            else
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = 0;
                                dataGridView1.RefreshEdit();
                            }
                        }
                        if (columnIndex == 20)
                        {
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null)
                            {
                                dataGridView1.Rows[rowIndex].Cells[20].Value = DateTime.Now;
                                dataGridView1.RefreshEdit();
                            }
                        }
                        if (columnIndex == 22)
                        {
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null)
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = new TimeSpan(4, 0, 0);
                                dataGridView1.RefreshEdit();
                            }
                        }
                    }

                }
                else
                {
                    if (dataGridView1.Rows[rowIndex].Cells[columnIndex] != null)
                    {
                        if (columnIndex == 0)
                        {
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null)
                            {
                                dataGridView1.Rows[rowIndex].Cells[0].Value = DateTime.Now;
                                dataGridView1.RefreshEdit();
                                dataGridView1.EndEdit();
                            }
                        }
                        if (columnIndex == 2 || columnIndex == 4)
                        {

                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null)
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                                dataGridView1.RefreshEdit();
                                dataGridView1.EndEdit();
                            }

                        }
                        if (columnIndex == 7 || columnIndex == 8 || columnIndex == 9 || columnIndex == 14 || columnIndex == 15 || columnIndex == 16 || columnIndex == 17 || columnIndex == 18 || columnIndex == 19)
                        {
                            if ((dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null) && (dataGridView1.Rows[rowIndex].Cells[10] != null))
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = dataGridView1.Rows[rowIndex].Cells[10].Value;
                                dataGridView1.RefreshEdit();
                                dataGridView1.EndEdit();
                            }
                            else
                            {
                                if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value != null)
                                {
                                    dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = null;
                                    dataGridView1.RefreshEdit();
                                    dataGridView1.EndEdit();
                                }
                            }
                        }
                        if (columnIndex == 12 || columnIndex == 13)
                        {
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null)
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = 1;
                                dataGridView1.RefreshEdit();
                                dataGridView1.EndEdit();
                            }
                            else
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = 0;
                                dataGridView1.RefreshEdit();
                                dataGridView1.EndEdit();
                            }
                        }
                        if (columnIndex == 20)
                        {
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null)
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = DateTime.Now;
                                dataGridView1.RefreshEdit();
                                dataGridView1.EndEdit();
                            }
                        }
                        if (columnIndex == 22)
                        {
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null)
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = new TimeSpan(4, 0, 0);
                                dataGridView1.RefreshEdit();
                                dataGridView1.EndEdit();
                            }
                            else
                            {
                                if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value != null)
                                {
                                    dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = null;
                                    dataGridView1.RefreshEdit();
                                    dataGridView1.EndEdit();
                                }
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
        private Flight Summarize(List<Flight> flights)
        {
            TimeSpan SEPTime = new TimeSpan(0);
            TimeSpan MEPTime = new TimeSpan(0);
            TimeSpan multiPilotFlightTime = new TimeSpan(0);
            TimeSpan totalFlightTime = new TimeSpan(0);
            int dayLdgs = 0;
            int nightLdgs = 0;
            TimeSpan nightTime = new TimeSpan();
            TimeSpan ifrTime = new TimeSpan();
            TimeSpan PICTime = new TimeSpan();
            TimeSpan CopiTime = new TimeSpan();
            TimeSpan DualTime = new TimeSpan();
            TimeSpan InstructorTime = new TimeSpan();
            TimeSpan SimTime = new TimeSpan();
            foreach (Flight tempFlight in flights)
            {
                if (tempFlight.SEPTime.HasValue)
                {
                    SEPTime = SEPTime.Add(tempFlight.SEPTime.Value);
                }
                if (tempFlight.MEPTime.HasValue)
                {
                    MEPTime = MEPTime.Add(tempFlight.MEPTime.Value);
                }
                if (tempFlight.MultiPilotTime.HasValue)
                {
                    multiPilotFlightTime = multiPilotFlightTime.Add(tempFlight.MultiPilotTime.Value);
                }
                if (tempFlight.TotalTimeOfFlight.HasValue)
                {
                    totalFlightTime = totalFlightTime.Add(tempFlight.TotalTimeOfFlight.Value);
                }
                if (tempFlight.DayLandings.HasValue)
                {
                    dayLdgs += tempFlight.DayLandings.Value;
                }
                if (tempFlight.NightLandings.HasValue)
                {
                    nightLdgs += tempFlight.NightLandings.Value;
                }
                if (tempFlight.NightTime.HasValue)
                {
                    nightTime = nightTime.Add(tempFlight.NightTime.Value);
                }
                if (tempFlight.IFRTime.HasValue)
                {
                    ifrTime = ifrTime.Add(tempFlight.IFRTime.Value);
                }
                if (tempFlight.PICTime.HasValue)
                {
                    PICTime = PICTime.Add(tempFlight.PICTime.Value);
                }
                if (tempFlight.CopilotTime.HasValue)
                {
                    CopiTime = CopiTime.Add(tempFlight.CopilotTime.Value);
                }
                if (tempFlight.DualTime.HasValue)
                {
                    DualTime = DualTime.Add(tempFlight.DualTime.Value);
                }
                if (tempFlight.InstructorTime.HasValue)
                {
                    InstructorTime = InstructorTime.Add(tempFlight.InstructorTime.Value);
                }
                if (tempFlight.SimTime.HasValue)
                {
                    SimTime = SimTime.Add(tempFlight.SimTime.Value);
                }
            }
            return new Flight(DateTime.MaxValue, null, null, null, null, null, null, SEPTime, MEPTime, multiPilotFlightTime, totalFlightTime, null, dayLdgs, nightLdgs, nightTime, ifrTime, PICTime, CopiTime, DualTime, InstructorTime, null, null, SimTime, "sum of all flights", false);
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
                            if (isMono)
                            {
                                TimeSpan test;
                                if (TimeSpan.TryParse(e.Value.ToString().Substring(0, 2) + ":" + e.Value.ToString().Substring(2, 2), out test))
                                {
                                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = test;
                                    e.ParsingApplied = true;
                                }
                            }
                            else
                            {
                                if (e.Value.ToString().Length > 3 && e.Value.ToString().Length < 5 && !e.Value.ToString().Contains(":"))
                                {
                                    e.Value = TimeSpan.Parse(e.Value.ToString().Substring(0, 2) + ":" + e.Value.ToString().Substring(2, 2));
                                    e.ParsingApplied = true;
                                }
                                if (e.Value.ToString().Length > 2 && e.Value.ToString().Length < 4 && !e.Value.ToString().Contains(":"))
                                {
                                    e.Value = TimeSpan.Parse(e.Value.ToString().Substring(0, 1) + ":" + e.Value.ToString().Substring(1, 2));
                                    e.ParsingApplied = true;
                                }
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

        // update sum row
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (iFlights.Count > 0)
            {
                // remove the sum-row
                for (int i = 0; i < Flights.Count; i++)
                {
                    if (Flights[i].FlightDate.HasValue)
                    {
                        if (Flights[i].FlightDate.Value.Year > 9000)
                        {
                            Flights.RemoveAt(i);
                        }
                    }
                }
                iFlights.Add(Summarize(Flights));
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                // show totalhours to make it possible in sumrow to show more than 24hrs in one cell
                if (e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 14 || e.ColumnIndex == 15 || e.ColumnIndex == 16 || e.ColumnIndex == 17 || e.ColumnIndex == 18 || e.ColumnIndex == 19 || e.ColumnIndex == 22)
                {
                    TimeSpan temp = (TimeSpan)e.Value;
                    if (temp.TotalHours > 24)
                    {
                        e.Value = (int)temp.TotalHours + ":" + temp.Minutes;
                        e.FormattingApplied = true;
                    }
                }
                // sum row
                if (e.ColumnIndex == 0 && e.Value.Equals(DateTime.MaxValue))
                {
                    e.Value = "∑:";
                    e.FormattingApplied = true;
                }
                // previous experience row
                if (e.ColumnIndex == 0 && e.Value.Equals(DateTime.MinValue))
                {
                    e.Value = "α:";
                    e.FormattingApplied = true;
                }
            }
        }
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e != null && dataGridView1.CurrentRow != null && dataGridView1.Rows[e.RowIndex].Cells[0].Value != null)
            {
                if (dataGridView1.CurrentRow.Cells[0].Value.Equals(DateTime.MaxValue))
                {
                    e.Cancel = true;
                }
                else
                {
                    if (dataGridView1.CurrentRow.Cells[0].Value.Equals(DateTime.MinValue) && (e.ColumnIndex < 7 || e.ColumnIndex == 11 || e.ColumnIndex == 20 || e.ColumnIndex == 21 || e.ColumnIndex == 23))
                    {
                        e.Cancel = true;
                    }
                }
            }
        }
        private void previousExpecienceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iFlights.Insert(0, new Flight(DateTime.MinValue, null, "", null, "", "", "", TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, DateTime.MinValue, "", TimeSpan.Zero, "previous experience", false));
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Form1.ActiveForm.WindowState = FormWindowState.Maximized;
        }
    }
}
