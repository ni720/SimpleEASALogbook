using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add(dataGridView1.Rows.Count.ToString());
        }

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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
            // this makes scrolling through the datagridview much faster, but can slowdown in a terminal session
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type dgvType = dataGridView1.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dataGridView1, true, null);
            }

            LoadDB();

            // workaround for mono-framework
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // add rownumber to row #1
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = dataGridView1.Rows.Count.ToString();

        }
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
                button3.Top = Form1.ActiveForm.Height - 87;
                button4.Top = Form1.ActiveForm.Height - 87;
                button5.Top = Form1.ActiveForm.Height - 87;
                button6.Top = Form1.ActiveForm.Height - 87;
            }

        }
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
                        dataGridView1.Rows.Add(dataGridView1.Rows.Count.ToString(), flight.getDateString(), flight.getDepartureString(), flight.getOffBlockTimeString(), flight.getDestinationString(), flight.getOnBlockTimeString(), flight.getTypeOfAircraftString(), flight.getRegistrationString(), flight.getSEPTimeString(), flight.getMEPTimeString(), flight.getMultiPilotTimeString(), flight.getTotalTimeString(), flight.getPICNameString(), flight.getDayLDGString(), flight.getNightLDGString(), flight.getNightTimeString(), flight.getIFRTimeString(), flight.getPICTimeString(), flight.getCopilotTimeString(), flight.getDualTimeString(), flight.getInstructorTimeString(), flight.getSimDateString(), flight.getSimTypeString(), flight.getSimTimeString(), flight.getRemarksString());

                        if (flight.hasPageBreak())
                        {
                            dataGridView1.Rows.Add(dataGridView1.Rows.Count.ToString(), "pagebreak", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*");
                        }

                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(), "ERROR");
                }
            }
        }

        private void SaveTable()
        {
            Flights = new List<Flight>();
            string stringBuilder = "";
            int i = 0, j = 0;
            try
            {
                for (i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    for (j = 1; j < dataGridView1.Columns.Count; j++)
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
            catch
            {

            }

            try
            {
                Import_EASA_CSV import = new Import_EASA_CSV(stringBuilder);
                Flights.AddRange(import.getFlightList());
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString(), "ERROR");
            }
            try
            {
                Export_EASA_CSV export = new Export_EASA_CSV(Flights);
                File.WriteAllText("EASALogbook.csv", export.GetCSV());
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString(), "ERROR");
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
        private void validateCells(int rowIndex, int columnIndex)
        {
            //TODO Cell validation
            var row = dataGridView1.Rows[rowIndex];
            if (null != row)
            {
                var cell = row.Cells[columnIndex];
                if (null != cell)
                {
                    if (columnIndex == 1 || columnIndex == 21)
                    {
                        object value = cell.Value;
                        if (null != value && !value.Equals(string.Empty))
                        {
                            // Do your test here in combination with columnindex etc
                            if (!DateTime.TryParse(value.ToString(), out DateTime dummy))
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].ErrorText = "DateTime must be dd.mm.yyyy";
                                dataGridView1.Rows[rowIndex].ErrorText = "error, entry will not be saved";
                            }
                            else
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].ErrorText = "";
                            }
                        }
                    }
                    if (columnIndex == 3 || columnIndex == 5 || columnIndex == 8 || columnIndex == 9 || columnIndex == 10 || columnIndex == 11 || columnIndex == 15 || columnIndex == 16 || columnIndex == 17 || columnIndex == 18 || columnIndex == 19 || columnIndex == 20 || columnIndex == 23)
                    {
                        object value = cell.Value;
                        if (null != value && !value.Equals(string.Empty))
                        {
                            // Do your test here in combination with columnindex etc
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
                    if (columnIndex == 13 || columnIndex == 14)
                    {
                        object value = cell.Value;
                        if (null != value && !value.Equals(string.Empty))
                        {
                            // Do your test here in combination with columnindex etc
                            if (!int.TryParse(value.ToString(), out int dummy))
                            {
                                dataGridView1.Rows[rowIndex].Cells[columnIndex].ErrorText = "must be a digit from 1 to 9, will not be saved otherwise";
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 1 && !dataGridView1.Rows[dataGridView1.Rows.Count - 1].Equals(dataGridView1.SelectedRows[0]))
            {
                toolStripStatusLabel1.Text = "deleting row...";
                Form1.ActiveForm.Enabled = false;
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                toolStripStatusLabel1.Text = "";
                Form1.ActiveForm.Enabled = true;
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
            // add rownumber to each new row
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = dataGridView1.Rows.Count.ToString();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            validateCells(e.RowIndex, e.ColumnIndex);
            validateRows(e.RowIndex);
            autoFillCellValue(e.RowIndex, e.ColumnIndex);
        }

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

        private void autoFillCellValue(int rowIndex, int columnIndex)
        {
            // auto calculate totalflighttime
            try
            {
                if (dataGridView1.Rows[rowIndex].Cells[11].Value == null)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[1].Value != null && dataGridView1.Rows[rowIndex].Cells[3].Value != null && dataGridView1.Rows[rowIndex].Cells[5].Value != null)
                    {
                        TimeSpan.TryParse(dataGridView1.Rows[rowIndex].Cells[3].Value.ToString(), out TimeSpan begin);
                        TimeSpan.TryParse(dataGridView1.Rows[rowIndex].Cells[5].Value.ToString(), out TimeSpan end);
                        if (end.Hours < begin.Hours)
                        {
                            end = end.Add(TimeSpan.FromHours(24));
                        }
                        dataGridView1.Rows[rowIndex].Cells[11].Value = end.Subtract(begin).ToString().Substring(0, 5);
                    }
                }
            }
            catch (Exception e)
            {
                // ingore compilermessage
                e.ToString();
            }

        }

        // to easily populate with data
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[1].Value = DateTime.Now.ToShortDateString();
                    }
                }
                if (e.ColumnIndex == 3)
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[3].Value = DateTime.Now.ToShortTimeString();
                    }
                }
                if (e.ColumnIndex == 5)
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[5].Value = DateTime.Now.ToShortTimeString();
                    }
                }
                if (e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 15 || e.ColumnIndex == 16 || e.ColumnIndex == 17 || e.ColumnIndex == 18 || e.ColumnIndex == 19 || e.ColumnIndex == 20)
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null && dataGridView1.Rows[e.RowIndex].Cells[11].Value != null && dataGridView1.Rows[e.RowIndex].Cells[11].ErrorText.Length<1)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridView1.Rows[e.RowIndex].Cells[11].Value;
                    }else
                    {
                        if(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                        {
                            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                        }
                    }
                }
                if (e.ColumnIndex == 21)
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[21].Value = DateTime.Now.ToShortDateString();
                    }
                }
            }
            catch (Exception y)
            {
                // ingore compilermessage
                y.ToString();
            }

        }

        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            // Try to sort based on the cells in the current column.
            e.SortResult = System.String.Compare(
                e.CellValue1.ToString(), e.CellValue2.ToString());

            // If the cells are equal, sort based on the ID column.
            if (e.SortResult == 0 && e.Column.Index==1)
            {
                e.SortResult = System.String.Compare(
                    dataGridView1.Rows[e.RowIndex1].Cells[3].Value.ToString(),
                    dataGridView1.Rows[e.RowIndex2].Cells[3].Value.ToString());
            }
            e.Handled = true;
        }
    }
}
