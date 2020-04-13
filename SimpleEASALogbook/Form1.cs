using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SimpleEASALogbook
{
    public partial class Form1 : Form
    {
        public static WaitForm _WaitForm = new WaitForm();
        public static SortableBindingList<Flight> BindedFlightList = new SortableBindingList<Flight>();
        public static List<Flight> FlightList = new List<Flight>();
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

        // open About form
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
            about.Focus();
            about.BringToFront();
        }

        // auto calculate totalflighttime
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
                File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " autoFillCellValue: " + rowIndex + "x" + columnIndex + "\n" + e.ToString() + "\n");
            }
        }

        // Import Brussels PDF
        private void BrusselsPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableControls(false);
            MessageBox.Show("please note:\nimporters import flights incomplete or wrong even if they think everything is normal. please thoroughly check if everything was correctly imported.\n\nLimitations of the Brussels PDF importer:\nPIC times and Instructor times are not imported due to lack of testdata. please submit testdata to the developers if you have them.\nAll times are assumed to be copilot times.\n\n\nyou can choose multiple files at once to import", "DISCLAIMER", MessageBoxButtons.OK, MessageBoxIcon.Information);
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "Brussels conform PDF export|*.pdf";
            string PDFToTextPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "pdftotext.exe");
            bool _ErrorOccured = false;

            if (isMono)
            {
                if (!File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "pdftotext")))
                {
                    MessageBox.Show("pdftotext binary has to be placed in the folder of SimpleEASALogbook. Download commandline tools from: http://www.xpdfreader.com/download.html", "Error!");
                    return;
                }
                else
                {
                    PDFToTextPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "pdftotext");
                }
            }
            else
            {
                if (!File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "pdftotext.exe")))
                {
                    MessageBox.Show("pdftotext.exe has to be placed in the folder of SimpleEASALogbook. Download commandline tools from: http://www.xpdfreader.com/download.html", "Error!");
                    return;
                }
            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string FilePathName in openFileDialog1.FileNames)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = PDFToTextPath, Arguments = "-table " + FilePathName + " " + System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "temp_pdf_to_text.txt"), };
                        Process proc = new Process() { StartInfo = startInfo, };
                        proc.Start();
                        proc.WaitForExit();
                        Import_Brussels_PDF import = new Import_Brussels_PDF(File.ReadAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "temp_pdf_to_text.txt")).ToString());
                        if (import.GetError())
                        {
                            _ErrorOccured = true;
                        }
                        File.Delete(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "temp_pdf_to_text.txt"));
                        FlightList.AddRange(import.GetFlightList());
                    }
                    RenewSumRow();
                    BindedFlightList.Sort("FlightDate", ListSortDirection.Ascending);
                    dataGridView1.RowCount = BindedFlightList.Count;
                    MarkAllCellsEditable();
                    if (_ErrorOccured)
                    {
                        MessageBox.Show("at least one error occured during import. the import might have been successful nevertheless. if in doubt please check the \"_easa_errorlog.txt\" ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("import seems to be successful", "success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception exc)
                {
                    EnableControls(true);
                    File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " Import_Brussels_PDF:\n" + exc.ToString() + "\n");
                    MessageBox.Show("An error occured during import, please check the \"_easa_errorlog.txt\" ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            EnableControls(true);
        }

        // + button
        private void Button1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "   adding row...";
            EnableControls(false);
            if (dataGridView1.CurrentRow.Index < dataGridView1.RowCount - 1)
            {
                if (dataGridView1.CurrentCell.RowIndex < 1)
                {
                    FlightList.Insert(dataGridView1.RowCount - 1, new Flight());
                }
                else
                {
                    FlightList.Insert(dataGridView1.CurrentCell.RowIndex + 1, new Flight());
                }
            }
            else
            {
                FlightList.Insert(dataGridView1.CurrentCell.RowIndex, new Flight());
            }
            this.dataGridView1.RowCount = BindedFlightList.Count;
            dataGridView1.Refresh();
            if (dataGridView1.CurrentRow.Index < dataGridView1.RowCount - 1)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index + 1].Cells[dataGridView1.CurrentCell.ColumnIndex];
            }
            if (isMono)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1];
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
                FlightList.RemoveAt(dataGridView1.CurrentCell.RowIndex);
                this.dataGridView1.RowCount = BindedFlightList.Count;
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
                    Thread.Sleep(50);
                    dataGridView1.DefaultCellStyle.SelectionBackColor = normal;
                    dataGridView1.Refresh();
                    Thread.Sleep(50);
                    dataGridView1.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightGray;
                    dataGridView1.Refresh();
                    Thread.Sleep(50);
                    dataGridView1.DefaultCellStyle.SelectionBackColor = normal;
                    dataGridView1.Refresh();
                    Thread.Sleep(50);
                    dataGridView1.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightGray;
                    dataGridView1.Refresh();
                    Thread.Sleep(50);
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
            Form1.ActiveForm.Update();
            SaveTable();
            toolStripStatusLabel1.Text = "   saved.";
            EnableControls(true);
        }

        // end button - saving question will be at Form1_FormClosing event
        private void Button6_Click(object sender, EventArgs e)
        {
            EnableControls(false);
            Application.Exit();
        }

        // only allow digits and separators in cells
        private void Cell_KeyPress_Allow_Digits_and_Separators(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ':' && e.KeyChar != '/')
            {
                e.Handled = true;
            }
        }

        // only allow digits in cells
        private void Cell_KeyPress_Allow_Digits_only(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // exit from menu - saving question will be at Form1_FormClosing event
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableControls(false);
            Application.Exit();
        }

        // disable edit in certain prev exp columns and in sumrow
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
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

        // mono sometimes needs cell-content doubleclick
        private void DataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (isMono)
            {
                PopulateDataOnClick(e.RowIndex, e.ColumnIndex);
                AutoFillCellValue(e.RowIndex, e.ColumnIndex);
            }
        }

        // on doubleclick we try to fill the cell with value
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            PopulateDataOnClick(e.RowIndex, e.ColumnIndex);
            AutoFillCellValue(e.RowIndex, e.ColumnIndex);
        }

        // finished editing -> see if other cells can be filled with value
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AutoFillCellValue(e.RowIndex, e.ColumnIndex);
        }

        // display α & ∑ for prev experience and sum row, and allow timespans greater than 23:59 in sumrow
        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                // show totalhours to make it possible in sumrow to show more than 24hrs in one cell
                if (e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 14 || e.ColumnIndex == 15 || e.ColumnIndex == 16 || e.ColumnIndex == 17 || e.ColumnIndex == 18 || e.ColumnIndex == 19 || e.ColumnIndex == 22)
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

        // update sum row
        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            RenewSumRow();
        }

        // datagridview virtualmode -> get cellvalue from list
        private void DataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (BindedFlightList.Count > e.RowIndex)    // nullcheck for mono - 2do test if works
            {
                switch (e.ColumnIndex)
                {
                    case 0:
                        e.Value = BindedFlightList[e.RowIndex].FlightDate;
                        break;

                    case 1:
                        e.Value = BindedFlightList[e.RowIndex].DepartureAirport;
                        break;

                    case 2:
                        e.Value = BindedFlightList[e.RowIndex].OffBlockTime;
                        break;

                    case 3:
                        e.Value = BindedFlightList[e.RowIndex].DestinationAirport;
                        break;

                    case 4:
                        e.Value = BindedFlightList[e.RowIndex].OnBlockTime;
                        break;

                    case 5:
                        e.Value = BindedFlightList[e.RowIndex].TypeOfAircraft;
                        break;

                    case 6:
                        e.Value = BindedFlightList[e.RowIndex].AircraftRegistration;
                        break;

                    case 7:
                        e.Value = BindedFlightList[e.RowIndex].SEPTime;
                        break;

                    case 8:
                        e.Value = BindedFlightList[e.RowIndex].MEPTime;
                        break;

                    case 9:
                        e.Value = BindedFlightList[e.RowIndex].MultiPilotTime;
                        break;

                    case 10:
                        e.Value = BindedFlightList[e.RowIndex].TotalTimeOfFlight;
                        break;

                    case 11:
                        e.Value = BindedFlightList[e.RowIndex].PilotInCommand;
                        break;

                    case 12:
                        e.Value = BindedFlightList[e.RowIndex].DayLandings;
                        break;

                    case 13:
                        e.Value = BindedFlightList[e.RowIndex].NightLandings;
                        break;

                    case 14:
                        e.Value = BindedFlightList[e.RowIndex].NightTime;
                        break;

                    case 15:
                        e.Value = BindedFlightList[e.RowIndex].IFRTime;
                        break;

                    case 16:
                        e.Value = BindedFlightList[e.RowIndex].PICTime;
                        break;

                    case 17:
                        e.Value = BindedFlightList[e.RowIndex].CopilotTime;
                        break;

                    case 18:
                        e.Value = BindedFlightList[e.RowIndex].DualTime;
                        break;

                    case 19:
                        e.Value = BindedFlightList[e.RowIndex].InstructorTime;
                        break;

                    case 20:
                        e.Value = BindedFlightList[e.RowIndex].DateOfSim;
                        break;

                    case 21:
                        e.Value = BindedFlightList[e.RowIndex].TypeOfSim;
                        break;

                    case 22:
                        e.Value = BindedFlightList[e.RowIndex].SimTime;
                        break;

                    case 23:
                        e.Value = BindedFlightList[e.RowIndex].Remarks;
                        break;

                    case 24:
                        e.Value = BindedFlightList[e.RowIndex].PageBreak;
                        break;
                }
            }
        }

        // datagridview virtualmode -> update list from cellvalue
        private void DataGridView1_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            DateTime tmp_date;
            TimeSpan tmp_time;
            int ldg;

            switch (e.ColumnIndex)
            {
                case 0:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].FlightDate = null;
                    }
                    else
                    {
                        if (TryParseTimeDate(e.Value.ToString(), out tmp_date))
                        {
                            BindedFlightList[e.RowIndex].FlightDate = tmp_date;
                        }
                    }
                    break;

                case 1:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].DepartureAirport = "";
                    }
                    else
                    {
                        BindedFlightList[e.RowIndex].DepartureAirport = e.Value.ToString();
                    }
                    break;

                case 2:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].OffBlockTime = null;
                    }
                    else
                    {
                        if (TryParseTimeSpan(e.Value.ToString(), out tmp_time))
                        {
                            BindedFlightList[e.RowIndex].OffBlockTime = tmp_time;
                        }
                    }
                    break;

                case 3:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].DestinationAirport = "";
                    }
                    else
                    {
                        BindedFlightList[e.RowIndex].DestinationAirport = e.Value.ToString();
                    }
                    break;

                case 4:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].OnBlockTime = null;
                    }
                    else
                    {
                        if (TryParseTimeSpan(e.Value.ToString(), out tmp_time))
                        {
                            BindedFlightList[e.RowIndex].OnBlockTime = tmp_time;
                        }
                    }
                    break;

                case 5:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].TypeOfAircraft = "";
                    }
                    else
                    {
                        BindedFlightList[e.RowIndex].TypeOfAircraft = e.Value.ToString();
                    }
                    break;

                case 6:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].AircraftRegistration = "";
                    }
                    else
                    {
                        BindedFlightList[e.RowIndex].AircraftRegistration = e.Value.ToString();
                    }
                    break;

                case 7:
                    if (bool.TryParse(e.Value.ToString(), out bool SEPTime))
                    {
                        BindedFlightList[e.RowIndex].SEPTime = SEPTime;
                    }
                    break;

                case 8:
                    if (bool.TryParse(e.Value.ToString(), out bool MEPTime))
                    {
                        BindedFlightList[e.RowIndex].MEPTime = MEPTime;
                    }
                    break;

                case 9:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].MultiPilotTime = null;
                    }
                    else
                    {
                        if (TryParseTimeSpan(e.Value.ToString(), out tmp_time))
                        {
                            BindedFlightList[e.RowIndex].MultiPilotTime = tmp_time;
                        }
                    }
                    break;

                case 10:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].TotalTimeOfFlight = null;
                    }
                    else
                    {
                        if (TryParseTimeSpan(e.Value.ToString(), out tmp_time))
                        {
                            BindedFlightList[e.RowIndex].TotalTimeOfFlight = tmp_time;
                        }
                    }
                    break;

                case 11:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].PilotInCommand = "";
                    }
                    else
                    {
                        BindedFlightList[e.RowIndex].PilotInCommand = e.Value.ToString();
                    }
                    break;

                case 12:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].DayLandings = null;
                    }
                    else
                    {
                        if (int.TryParse(e.Value.ToString(), out ldg))
                        {
                            BindedFlightList[e.RowIndex].DayLandings = ldg;
                        }
                    }
                    break;

                case 13:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].NightLandings = null;
                    }
                    else
                    {
                        if (int.TryParse(e.Value.ToString(), out ldg))
                        {
                            BindedFlightList[e.RowIndex].NightLandings = ldg;
                        }
                    }
                    break;

                case 14:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].NightTime = null;
                    }
                    else
                    {
                        if (TryParseTimeSpan(e.Value.ToString(), out tmp_time))
                        {
                            BindedFlightList[e.RowIndex].NightTime = tmp_time;
                        }
                    }
                    break;

                case 15:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].IFRTime = null;
                    }
                    else
                    {
                        if (TryParseTimeSpan(e.Value.ToString(), out tmp_time))
                        {
                            BindedFlightList[e.RowIndex].IFRTime = tmp_time;
                        }
                    }
                    break;

                case 16:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].PICTime = null;
                    }
                    else
                    {
                        if (TryParseTimeSpan(e.Value.ToString(), out tmp_time))
                        {
                            BindedFlightList[e.RowIndex].PICTime = tmp_time;
                        }
                    }
                    break;

                case 17:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].CopilotTime = null;
                    }
                    else
                    {
                        if (TryParseTimeSpan(e.Value.ToString(), out tmp_time))
                        {
                            BindedFlightList[e.RowIndex].CopilotTime = tmp_time;
                        }
                    }
                    break;

                case 18:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].DualTime = null;
                    }
                    else
                    {
                        if (TryParseTimeSpan(e.Value.ToString(), out tmp_time))
                        {
                            BindedFlightList[e.RowIndex].DualTime = tmp_time;
                        }
                    }
                    break;

                case 19:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].InstructorTime = null;
                    }
                    else
                    {
                        if (TryParseTimeSpan(e.Value.ToString(), out tmp_time))
                        {
                            BindedFlightList[e.RowIndex].InstructorTime = tmp_time;
                        }
                    }
                    break;

                case 20:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].DateOfSim = null;
                    }
                    else
                    {
                        if (TryParseTimeDate(e.Value.ToString(), out tmp_date))
                        {
                            BindedFlightList[e.RowIndex].DateOfSim = tmp_date;
                        }
                    }
                    break;

                case 21:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].TypeOfSim = "";
                    }
                    else
                    {
                        BindedFlightList[e.RowIndex].TypeOfSim = e.Value.ToString();
                    }
                    break;

                case 22:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].SimTime = null;
                    }
                    else
                    {
                        if (TryParseTimeSpan(e.Value.ToString(), out tmp_time))
                        {
                            BindedFlightList[e.RowIndex].SimTime = tmp_time;
                        }
                    }
                    break;

                case 23:
                    if (e.Value == null)
                    {
                        BindedFlightList[e.RowIndex].TypeOfSim = "";
                    }
                    else
                    {
                        BindedFlightList[e.RowIndex].Remarks = e.Value.ToString();
                    }
                    break;

                case 24:
                    if (bool.TryParse(e.Value.ToString(), out bool pagebreak))
                    {
                        BindedFlightList[e.RowIndex].PageBreak = pagebreak;
                    }
                    break;
            }
        }

        // restrict characters on certain cells
        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Cell_KeyPress_Allow_Digits_and_Separators);
            e.Control.KeyPress -= new KeyPressEventHandler(Cell_KeyPress_Allow_Digits_only);

            // cells with digits and separators
            if (dataGridView1.CurrentCell.ColumnIndex == 0 || dataGridView1.CurrentCell.ColumnIndex == 2 || dataGridView1.CurrentCell.ColumnIndex == 4 || dataGridView1.CurrentCell.ColumnIndex == 9 || dataGridView1.CurrentCell.ColumnIndex == 10 || dataGridView1.CurrentCell.ColumnIndex == 14 || dataGridView1.CurrentCell.ColumnIndex == 15 || dataGridView1.CurrentCell.ColumnIndex == 16 || dataGridView1.CurrentCell.ColumnIndex == 17 || dataGridView1.CurrentCell.ColumnIndex == 18 || dataGridView1.CurrentCell.ColumnIndex == 19 || dataGridView1.CurrentCell.ColumnIndex == 20 || dataGridView1.CurrentCell.ColumnIndex == 22)
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

        // Export EASA HTML
        private void EASAHTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            EnableControls(false);
            saveFileDialog1.Filter = "EASA conform HTML|*.html";
            saveFileDialog1.FileName = "Simple_EASA_Logbook.html";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (saveFileDialog1.CheckFileExists)
                    {
                        File.Delete(saveFileDialog1.FileName);
                    }
                    BindedFlightList.Sort("FlightDate", ListSortDirection.Ascending);
                    List<Flight> temp = new List<Flight>(BindedFlightList.GetFlights());
                    // do not save the sum-row or empty rows
                    for (int i = 0; i < temp.Count; i++)
                    {
                        if (temp[i].FlightDate.HasValue)
                        {
                            if (temp[i].FlightDate.Value.Year > 9000)
                            {
                                temp.RemoveAt(i);
                            }
                            else
                            {
                                if (temp[i].FlightDate.Value.Year < 1000 && !temp[i].Remarks.Contains("previous experience"))
                                {
                                    temp.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            if (!temp[i].DateOfSim.HasValue)
                            {
                                temp.RemoveAt(i);
                            }
                        }
                    }
                    Export_EASA_HTML export = new Export_EASA_HTML(temp);
                    File.WriteAllText(saveFileDialog1.FileName, export.GetHTML());
                    MessageBox.Show(saveFileDialog1.FileName + " saved successfully", "success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ey)
                {
                    EnableControls(true);
                    File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " ExportTo_EASA_HTML:\n" + ey.ToString() + "\n");
                    MessageBox.Show("An error occured during export, please check the \"_easa_errorlog.txt\" ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            EnableControls(true);
        }

        // Export EASA CSV
        private void EasaLogbookCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            EnableControls(false);
            saveFileDialog1.Filter = "EASA Logbook conform CSV|*.csv";
            saveFileDialog1.FileName = "Simple_EASA_Logbook.csv";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (saveFileDialog1.CheckFileExists)
                    {
                        File.Delete(saveFileDialog1.FileName);
                    }
                    BindedFlightList.Sort("FlightDate", ListSortDirection.Ascending);
                    List<Flight> temp = new List<Flight>(BindedFlightList.GetFlights());
                    Export_EASA_CSV export = new Export_EASA_CSV(temp);
                    string _Header = "Date,DEP,OffBlock,DEST,OnBlock,ACFTType,ACFTReg,SEPTime,MEPTime,MultiPilotTime,TotalTime,NamePIC,LDGDay,LDGNight,NightTime,IFRTime,PICTime,CopilotTime,DualTime,InstructorTime,SIMDate,SIMType,SimTime,Remarks,Pagebreak\n";
                    File.WriteAllText(saveFileDialog1.FileName, _Header + export.GetCSV());
                    MessageBox.Show(saveFileDialog1.FileName + " saved successfully", "success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ey)
                {
                    EnableControls(true);
                    File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " ExportTo_EASA_CSV:\n" + ey.ToString() + "\n");
                    MessageBox.Show("An error occured during export, please check the \"_easa_errorlog.txt\" ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            EnableControls(true);
        }

        // Import EASA CSV
        private void EASALogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableControls(false);
            MessageBox.Show("please note:\nimporters import flights incomplete or wrong even if they think everything is normal. please thoroughly check if everything was correctly imported.", "DISCLAIMER", MessageBoxButtons.OK, MessageBoxIcon.Information);
            openFileDialog1.Multiselect = false;
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "EASA Logbook conform CSV|*.csv";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Import_EASA_CSV import = new Import_EASA_CSV(File.ReadAllText(openFileDialog1.FileName).ToString());
                    FlightList.AddRange(import.GetFlightList());
                    BindedFlightList.Sort("FlightDate", ListSortDirection.Ascending);
                    RenewSumRow();
                    dataGridView1.RowCount = BindedFlightList.Count;
                    MarkAllCellsEditable();
                    if (import.GetError())
                    {
                        MessageBox.Show("at least one error occured during import. the import might have been successful nevertheless. if in doubt please check the \"_easa_errorlog.txt\" ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("import seems to be successful", "success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception exc)
                {
                    EnableControls(true);
                    File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " Import_EASA_CSV:\n" + exc.ToString() + "\n");
                    MessageBox.Show("An error occured during import, please check the \"_easa_errorlog.txt\" ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            EnableControls(true);
        }

        // Export EASA PDF
        private void EASAPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            EnableControls(false);
            saveFileDialog1.Filter = "EASA conform PDF|*.pdf";
            saveFileDialog1.FileName = "Simple_EASA_Logbook.pdf";

            string pathToWKHTML;
            if (isMono)
            {
                if (!File.Exists("/usr/bin/wkhtmltopdf"))
                {
                    MessageBox.Show("No wkhtmltopdf found in /usr/bin, please install it", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    pathToWKHTML = "/usr/bin/wkhtmltopdf";
                }
            }
            else
            {
                if (!File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "wkhtmltopdf.exe")))
                {
                    MessageBox.Show("No wkhtmltopdf.exe found, please put it in the same folder as this program is running form. (downlowad from wkhtmltopdf.org)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    pathToWKHTML = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "wkhtmltopdf.exe");
                }
            }

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (saveFileDialog1.CheckFileExists)
                    {
                        File.Delete(saveFileDialog1.FileName);
                    }
                    BindedFlightList.Sort("FlightDate", ListSortDirection.Ascending);
                    List<Flight> temp = new List<Flight>(BindedFlightList.GetFlights());
                    // do not save the sum-row or empty rows
                    for (int i = 0; i < temp.Count; i++)
                    {
                        if (temp[i].FlightDate.HasValue)
                        {
                            if (temp[i].FlightDate.Value.Year > 9000)
                            {
                                temp.RemoveAt(i);
                            }
                            else
                            {
                                if (temp[i].FlightDate.Value.Year < 1000 && !temp[i].Remarks.Contains("previous experience"))
                                {
                                    temp.RemoveAt(i);
                                }
                            }
                        }
                        else
                        {
                            if (!temp[i].DateOfSim.HasValue)
                            {
                                temp.RemoveAt(i);
                            }
                        }
                    }
                    Export_EASA_HTML export = new Export_EASA_HTML(temp);
                    File.WriteAllText(saveFileDialog1.FileName.Replace("pdf", "htm"), export.GetHTML());
                    ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = pathToWKHTML, Arguments = "-O landscape --print-media-type " + saveFileDialog1.FileName.Replace("pdf", "htm") + " " + saveFileDialog1.FileName };
                    Process proc = new Process() { StartInfo = startInfo, };
                    proc.Start();
                    proc.WaitForExit();
                    File.Delete(saveFileDialog1.FileName.Replace("pdf", "htm"));
                    MessageBox.Show(saveFileDialog1.FileName + " saved successfully", "success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ey)
                {
                    EnableControls(true);
                    File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " ExportTo_EASA_PDF:\n" + ey.ToString() + "\n");
                    MessageBox.Show("An error occured during export, please check the \"_easa_errorlog.txt\" ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            EnableControls(true);
        }

        // toggle enable / disable controls
        private void EnableControls(bool value)
        {
            if (!value)
            {
                Form1.ActiveForm.Refresh();
            }
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

        // before exiting app ask if user wants to save
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            EnableControls(false);
            if (MessageBox.Show("do you want to save before closing?", "good bye", MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
            {
                toolStripStatusLabel1.Text = "   saving...";
                Form1.ActiveForm.Update();
                SaveTable();
                toolStripStatusLabel1.Text = "   saved.";
            }
            EnableControls(true);
        }

        // when application is loading - prepare everything
        private void Form1_OnLoad(object sender, EventArgs e)
        {
            // start time measurement
            var now = DateTime.Now;

            // bind the list of flights to the datasource of datagridview1
            BindedFlightList = new SortableBindingList<Flight>(FlightList);

            // check if running mono
            IsRunningOnMono();

            // waitform is using the same thread at the moment - this is bad but no time to do better now
            using (WaitForm _waitForm = new WaitForm())
            {
                _waitForm.Show();
                _waitForm.Update();
                _waitForm.BringToFront();

                // because of EASA logging rules
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

                // this makes scrolling through the datagridview much faster, but can slowdown in a terminal session
                if (!SystemInformation.TerminalServerSession)
                {
                    Type dgvType = dataGridView1.GetType();
                    PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                    pi.SetValue(dataGridView1, true, null);
                }

                // prepare the DataGridView
                // to make behaviour the same as with mono, the "newrow" has caused too many problems
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AllowUserToResizeColumns = true;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.AllowUserToOrderColumns = false;
                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.VirtualMode = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dataGridView1.AutoSize = false;

                BindedFlightList.AllowEdit = true;

                dataGridView1.Columns[2].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[4].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[9].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[10].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[14].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[15].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[16].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[17].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[18].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[19].DefaultCellStyle.Format = "%h\\:mm";
                dataGridView1.Columns[22].DefaultCellStyle.Format = "%h\\:mm";

                LoadDB();

                // if database empty or not existing
                if (FlightList.Count < 1)
                {
                    FlightList.Add(new Flight());
                    dataGridView1.RowCount = BindedFlightList.Count;
                    dataGridView1.Refresh();    // refresh the dgv for mono
                }

                // ignore the old sum-row
                for (int i = 0; i < FlightList.Count; i++)
                {
                    if (FlightList[i].FlightDate.HasValue)
                    {
                        if (FlightList[i].FlightDate.Value.Year > 9000)
                        {
                            FlightList.RemoveAt(i);
                        }
                    }
                }

                // add sum row
                BindedFlightList.Add(Summarize(FlightList));
                BindedFlightList.Sort("FlightDate", ListSortDirection.Ascending);
                dataGridView1.RowCount = BindedFlightList.Count;
                dataGridView1.Refresh();

                // scroll down to last row
                if (dataGridView1.Rows.Count > 1)
                {
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 2;
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[0];
                    if (isMono)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[1];
                    }
                }

                toolStripStatusLabel1.Text = "finished loading, it took: " + Math.Round((DateTime.Now.Subtract(now).TotalMilliseconds)).ToString() + " millisecond(s).";
            }
        }

        // move all controls if form is resized
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

        // when form is shown, maximize in mono
        private void Form1_Shown(object sender, EventArgs e)
        {
            if (Form1.ActiveForm != null && ActiveForm.CanFocus)    // 2do test can focus if it helps to prevent crash in mono
            {
                Form1.ActiveForm.WindowState = FormWindowState.Maximized;
            }
        }

        // open help for flight logging
        private void HowToLogYourFlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // in Mono simple URL opening sometimes does not work
            if (isMono)
            {
                if (MessageBox.Show("Opening links in unix directly sometimes fails. Do you want to copy the Link to EASA Part FCL PDF to clipboard?", "EASA Recording of flight time", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    Clipboard.SetText("https://www.easa.europa.eu/sites/default/files/dfu/Part-FCL.pdf");
                }
            }
            else
            {
                if (MessageBox.Show("Do you want to visit the EASA Part FCL PDF? (FCL.050 Recording of flight time)", "EASA Recording of flight time", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    Process.Start("https://www.easa.europa.eu/sites/default/files/dfu/Part-FCL.pdf");
                }
            }
        }

        // load table from "database" file
        private void LoadDB()
        {
            // the database relies on the normal easa_csv import/export functions
            if (File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "EASALogbook.csv")))
            {
                try
                {
                    Import_EASA_CSV import = new Import_EASA_CSV(File.ReadAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "EASALogbook.csv")).ToString());
                    FlightList.AddRange(import.GetFlightList());
                    BindedFlightList.Sort("FlightDate", ListSortDirection.Ascending);
                    dataGridView1.RowCount = BindedFlightList.Count;
                    MarkAllCellsEditable();
                }
                catch (Exception exc)
                {
                    File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " LoadDB:\n" + exc.ToString() + "\n");
                }
            }
        }

        // Import LH PDF
        private void LufthansaPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableControls(false);
            MessageBox.Show("please note:\nimporters import flights incomplete or wrong even if they think everything is normal. please thoroughly check if everything was correctly imported.\n\n\nyou can choose multiple files at once to import", "DISCLAIMER", MessageBoxButtons.OK, MessageBoxIcon.Information);
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "Lufthansa conform PDF|*.pdf";
            string PDFToTextPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "pdftotext.exe");
            bool _ErrorOccured = false;
            if (isMono)
            {
                if (!File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "pdftotext")))
                {
                    MessageBox.Show("pdftotext binary has to be placed in the folder of SimpleEASALogbook. Download commandline tools from: http://www.xpdfreader.com/download.html \nCaution the pdftotext version from libpoppler does not work due to the lack of the \"-table\" option", "Error!");
                    return;
                }
                else
                {
                    PDFToTextPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "pdftotext");
                }
            }
            else
            {
                if (!File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "pdftotext.exe")))
                {
                    MessageBox.Show("pdftotext.exe has to be placed in the folder of SimpleEASALogbook. Download commandline tools from: http://www.xpdfreader.com/download.html", "Error!");
                    return;
                }
            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string FilePathName in openFileDialog1.FileNames)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = PDFToTextPath, Arguments = "-raw " + FilePathName + " " + System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "temp_pdf_to_text.txt"), };
                        Process proc = new Process() { StartInfo = startInfo, };
                        proc.Start();
                        proc.WaitForExit();
                        Import_LH_PDF import = new Import_LH_PDF(File.ReadAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "temp_pdf_to_text.txt")).ToString());
                        if (import.GetError())
                        {
                            _ErrorOccured = true;
                        }
                        File.Delete(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "temp_pdf_to_text.txt"));
                        FlightList.AddRange(import.GetFlightList());
                    }
                    RenewSumRow();
                    BindedFlightList.Sort("FlightDate", ListSortDirection.Ascending);
                    dataGridView1.RowCount = BindedFlightList.Count;
                    MarkAllCellsEditable();
                    if (_ErrorOccured)
                    {
                        MessageBox.Show("at least one error occured during import. the import might have been successful nevertheless. if in doubt please check the \"_easa_errorlog.txt\" ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("import seems to be successful", "success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception exc)
                {
                    EnableControls(true);
                    File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " Import_LH_PDF:\n" + exc.ToString() + "\n");
                    MessageBox.Show("An error occured during import, please check the \"_easa_errorlog.txt\" ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            EnableControls(true);
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

        // Import MCCPilotLog CSV
        private void MCCPilotLogCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableControls(false);
            MessageBox.Show("please note:\nimporters import flights incomplete or wrong even if they think everything is normal. please thoroughly check if everything was correctly imported.\n\nLimitations of the MCC-Pilot-Log-Importer:\nSome values are not imported, like Multi-/Singleengine time, 2nd pilot, 3rd pilot, etc..\nPlease check the CSV file and add those values in the remarks section.\n\n\nyou can choose multiple files at once to import", "DISCLAIMER", MessageBoxButtons.OK, MessageBoxIcon.Information);
            bool _ErrorOccured = false;
            openFileDialog1.Filter = "MCC PilotLog conform CSV|*.csv";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string FilePathName in openFileDialog1.FileNames)
                    {
                        Import_MCC_CSV import = new Import_MCC_CSV(File.ReadAllText(FilePathName).ToString());
                        FlightList.AddRange(import.GetFlightList());
                        if (import.GetError())
                        {
                            _ErrorOccured = true;
                        }
                    }
                    RenewSumRow();
                    BindedFlightList.Sort("FlightDate", ListSortDirection.Ascending);
                    dataGridView1.RowCount = BindedFlightList.Count;
                    MarkAllCellsEditable();
                    if (_ErrorOccured)
                    {
                        MessageBox.Show("at least one error occured during import. the import might have been successful nevertheless. if in doubt please check the \"_easa_errorlog.txt\" ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("import seems to be successful", "success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception exc)
                {
                    EnableControls(true);
                    File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " Import_MCC_PilotLog_CSV:\n" + exc.ToString() + "\n");
                    MessageBox.Show("An error occured during import, please check the \"_easa_errorlog.txt\" ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            EnableControls(true);
        }

        // new database from menu
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "   resetting database...";
            EnableControls(false);
            ActiveForm.Update();
            ActiveForm.Refresh();
            FlightList.Clear();
            BindedFlightList.Clear();
            FlightList.Add(new Flight());
            BindedFlightList.Add(Summarize(FlightList));
            dataGridView1.RowCount = BindedFlightList.Count;
            EnableControls(true);
            toolStripStatusLabel1.Text = "   new database!";
        }

        // to easily populate cells with data
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
                            if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null)
                            {
                                // check is necessary to prevent off and onblock times for prev experience line
                                if (dataGridView1.Rows[rowIndex].Cells[0].Value != null)
                                {
                                    if (!dataGridView1.Rows[rowIndex].Cells[0].Value.Equals(DateTime.MinValue))
                                    {
                                        if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.Equals(TimeSpan.MinValue))
                                        {
                                            dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                                            dataGridView1.RefreshEdit();
                                        }
                                    }
                                }
                                else
                                {
                                    if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.Equals(TimeSpan.MinValue))
                                    {
                                        dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                                        dataGridView1.RefreshEdit();
                                    }
                                }
                            }
                        }
                        if (columnIndex == 9 || columnIndex == 14 || columnIndex == 15 || columnIndex == 16 || columnIndex == 17 || columnIndex == 18 || columnIndex == 19)
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
                            // check is necessary to prevent off and onblock times for prev experience line
                            if (dataGridView1.Rows[rowIndex].Cells[0].Value != null)
                            {
                                if (!dataGridView1.Rows[rowIndex].Cells[0].Value.Equals(DateTime.MinValue))
                                {
                                    if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.Equals(TimeSpan.MinValue))
                                    {
                                        dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                                        dataGridView1.RefreshEdit();
                                        dataGridView1.EndEdit();
                                    }
                                }
                            }
                            else
                            {
                                if (dataGridView1.Rows[rowIndex].Cells[columnIndex].Value == null || dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.Equals(TimeSpan.MinValue))
                                {
                                    dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                                    dataGridView1.RefreshEdit();
                                    dataGridView1.EndEdit();
                                }
                            }
                        }
                        if (columnIndex == 9 || columnIndex == 14 || columnIndex == 15 || columnIndex == 16 || columnIndex == 17 || columnIndex == 18 || columnIndex == 19)
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
                RenewSumRow();
                dataGridView1.Refresh();
            }
            catch (Exception y)
            {
                File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " populateDataOnClick: " + rowIndex + "x" + columnIndex + "\n" + y.ToString() + "\n");
            }
        }

        // import prev experience
        private void PreviousExpecienceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableControls(false);
            MessageBox.Show("please note:\nthis is not a real import function, in the first line of the table you simply add manually your added up flight hours", "DISCLAIMER", MessageBoxButtons.OK, MessageBoxIcon.Information);
            BindedFlightList.Insert(0, new Flight(DateTime.MinValue, "", null, "", null, "", "", false, false, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, DateTime.MinValue, "", TimeSpan.Zero, "previous experience", false));
            dataGridView1.RowCount = BindedFlightList.Count;
            EnableControls(true);
        }

        // open problems link
        private void ProblemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // in Mono simple URL opening sometimes does not work
            if (isMono)
            {
                if (MessageBox.Show("Opening links in unix directly sometimes fails. Do you want to copy the link to the GitHub Issues Page to clipboard?", "GitHub Issues", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    Clipboard.SetText("https://github.com/ni720/SimpleEASALogbook/issues");
                }
            }
            else
            {
                if (MessageBox.Show("Do you want to visit the GitHub Issues Page?", "GitHub Issues", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    Process.Start("https://github.com/ni720/SimpleEASALogbook/issues");
                }
            }
        }

        // recalculate the sum row
        private void RenewSumRow()
        {
            if (BindedFlightList.Count > 0)
            {
                // remove the sum-row
                for (int i = 0; i < FlightList.Count; i++)
                {
                    if (FlightList[i].FlightDate.HasValue)
                    {
                        if (FlightList[i].FlightDate.Value.Year > 9000)
                        {
                            FlightList.RemoveAt(i);
                        }
                    }
                }
                BindedFlightList.Add(Summarize(FlightList));
                dataGridView1.RowCount = BindedFlightList.Count;
            }
        }

        // save the table via EASA export CSV filter
        private void SaveTable()
        {
            try
            {
                // do not save the sum-row
                for (int i = 0; i < FlightList.Count; i++)
                {
                    if (FlightList[i].FlightDate.HasValue)
                    {
                        if (FlightList[i].FlightDate.Value.Year > 9000)
                        {
                            FlightList.RemoveAt(i);
                        }
                    }
                }
                BindedFlightList.Sort("FlightDate", ListSortDirection.Ascending);
                List<Flight> temp = BindedFlightList.GetFlights();
                Export_EASA_CSV export = new Export_EASA_CSV(temp);
                File.WriteAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "EASALogbook.csv"), export.GetCSV());
                BindedFlightList.Add(Summarize(FlightList));
                dataGridView1.RowCount = BindedFlightList.Count;
                dataGridView1.Refresh();
            }
            catch (Exception e)
            {
                File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " SaveTable_3:\n" + e.ToString() + "\n");
            }
        }

        // save from menu
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "   saving...";
            EnableControls(false);
            Form1.ActiveForm.Update();
            SaveTable();
            toolStripStatusLabel1.Text = "   saved.";
            EnableControls(true);
        }

        // summarize flight times
        private Flight Summarize(List<Flight> flights)
        {
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
            return new Flight(DateTime.MaxValue, null, null, null, null, null, null, false, false, multiPilotFlightTime, totalFlightTime, null, dayLdgs, nightLdgs, nightTime, ifrTime, PICTime, CopiTime, DualTime, InstructorTime, null, null, SimTime, "sum of all flights", false);
        }

        // allows the user to only enter some digits and become a date - can be misinterpreted
        private bool TryParseTimeDate(string value, out DateTime result)
        {
            if (value.Replace("/", "").Length == 4)
            {
                if (DateTime.TryParse(value.Replace("/", "").Substring(0, 1) + "/" + value.Replace("/", "").Substring(1, 1) + "/" + value.Replace("/", "").Substring(2, 2), out result))
                {
                    return true;
                }
            }
            if (value.Replace("/", "").Length == 6)
            {
                if (DateTime.TryParse(value.Replace("/", "").Substring(0, 2) + "/" + value.Replace("/", "").Substring(2, 2) + "/" + value.Replace("/", "").Substring(4, 2), out result))
                {
                    return true;
                }
            }

            return DateTime.TryParse(value, out result);
        }

        // allows the user to only enter some digits and become a timespan
        private bool TryParseTimeSpan(string value, out TimeSpan result)
        {
            TimeSpan test;

            if (value.Length > 3)
            {
                if (TimeSpan.TryParse(value.Substring(0, 2) + ":" + value.Substring(2, 2), out test))
                {
                    result = test;
                    return true;
                }
                else
                {
                    if (TimeSpan.TryParse(value, out test))
                    {
                        result = test;
                        return true;
                    }
                }
            }
            else
            {
                if (value.Length > 2)
                {
                    if (TimeSpan.TryParse(value.Substring(0, 1) + ":" + value.Substring(1, 2), out test))
                    {
                        result = test;
                        return true;
                    }
                }
                else
                {
                    if (value.Length > 1)
                    {
                        if (TimeSpan.TryParse("0:" + value.Substring(0, 2), out test))
                        {
                            result = test;
                            return true;
                        }
                    }
                    else
                    {
                        if (value.Length == 1)
                        {
                            if (TimeSpan.TryParse("0:0" + value.Substring(0, 1), out test))
                            {
                                result = test;
                                return true;
                            }
                        }
                        else
                        {
                            result = TimeSpan.Zero;
                            return true;
                        }
                    }
                }
            }

            result = TimeSpan.Zero;
            return false;
        }
    }
}