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
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = dataGridView1.Rows.Count.ToString();

        }
        private void Form1_OnResize(object sender, EventArgs e)
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
                        if(flight.DateOfSim.Equals(DateTime.MinValue))
                        {
                            dataGridView1.Rows.Add(dataGridView1.Rows.Count.ToString(), flight.OffBlockTime.ToShortDateString(), flight.DepartureAirport, flight.getOffBlockTimeString(), flight.DestinationAirport, flight.getOnBlockTimeString(), flight.TypeOfAircraft, flight.AircraftRegistration, flight.getSEPTimeString(), flight.getMEPTimeString(), flight.getMultiPilotTimeString(), flight.getTotalTimeString(), flight.PilotInCommand, flight.getDayLDGString(), flight.getNightLDGString(), flight.getNightTimeString(), flight.getIFRTimeString(), flight.getPICTimeString(), flight.getCopilotTimeString(), flight.getDualTimeString(), flight.getInstructorTimeString(), "", "", "", flight.Remarks);
                        }else
                        {
                            dataGridView1.Rows.Add(dataGridView1.Rows.Count.ToString(), flight.OffBlockTime.ToShortDateString(), flight.DepartureAirport, flight.getOffBlockTimeString(), flight.DestinationAirport, flight.getOnBlockTimeString(), flight.TypeOfAircraft, flight.AircraftRegistration, flight.getSEPTimeString(), flight.getMEPTimeString(), flight.getMultiPilotTimeString(), flight.getTotalTimeString(), flight.PilotInCommand, flight.getDayLDGString(), flight.getNightLDGString(), flight.getNightTimeString(), flight.getIFRTimeString(), flight.getPICTimeString(), flight.getCopilotTimeString(), flight.getDualTimeString(), flight.getInstructorTimeString(), flight.DateOfSim.ToShortDateString(), flight.TypeOfSim, flight.SimTime.ToString().Substring(0, 5), flight.Remarks);
                        }
                        if (flight.nextpageafter)
                        {
                            dataGridView1.Rows.Add(dataGridView1.Rows.Count.ToString(), "pagebreak", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*", "*") ;
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
            int i=0, j=0;
            try
            {
                for (i = 0; i < dataGridView1.Rows.Count-1; i++)
                {
                    for (j = 1; j < dataGridView1.Columns.Count; j++)
                    {
                       if(dataGridView1.Rows[i].Cells[j].Value == null)
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

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //TODO Cell validation
            var row = dataGridView1.Rows[e.RowIndex];
            if (null != row)
            {
                var cell = row.Cells[e.ColumnIndex];
                if (null != cell)
                {
                    object value = cell.Value;
                    if (null != value)
                    {
                        // Do your test here in combination with columnindex etc
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count>1 && !dataGridView1.Rows[dataGridView1.Rows.Count-1].Equals(dataGridView1.SelectedRows[0]))
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
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = dataGridView1.Rows.Count.ToString();
        }
    }
}
