using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleEASALogbook
{
    internal class Import_MCC_CSV
    {
        private bool _ErrorOccured = false;
        private bool _MEPTime = false;
        private bool _SEPTime = false;
        private string Aircraft = "";
        private TimeSpan CopilotTime = TimeSpan.Zero;
        private DateTime date_of_sim = DateTime.MinValue;
        private int DayLanding = 0;
        private TimeSpan DualTime = TimeSpan.Zero;
        private TimeSpan Endtime = TimeSpan.Zero;
        private List<Flight> Flights = new List<Flight>();
        private string FROM = "";
        private TimeSpan IFRTime = TimeSpan.Zero;
        private TimeSpan InstructorTime = TimeSpan.Zero;
        private string MCCVersion = "4";

        //TimeSpan SETime = TimeSpan.Zero;  // MCC Pilotlog does not correctly export SEP Time
        //TimeSpan METime = TimeSpan.Zero;  // MCC Pilotlog does not correctly export MEP Time
        private TimeSpan MultiPilotTime = TimeSpan.Zero;

        private int NightLanding = 0;
        private TimeSpan NightTime = TimeSpan.Zero;
        private bool pagebreak = false;
        private string PIC = "";
        private TimeSpan PICTime = TimeSpan.Zero;
        private string remarks = "";
        private TimeSpan sim_time = TimeSpan.Zero;
        private DateTime StartDate = DateTime.MinValue;
        private TimeSpan Starttime = TimeSpan.Zero;
        private string TO = "";
        private TimeSpan TotalTimeOfFlight = TimeSpan.Zero;
        private string Type = "";
        private string Type_of_sim = "";

        public Import_MCC_CSV(string mcccsv)
        {
            if (mcccsv.Contains("TIME_DEPSCH"))
            {
                MCCVersion = "3";
            }

            using (var reader = new StringReader(mcccsv))
            {
                int i = 0;
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    try
                    {
                        string[] csvline = new string[65];
                        csvline = line.Split(';');
                        if (MCCVersion.Equals("3"))
                        {
                            Flights.Add(CreateFlightMCCv3(csvline));
                        }
                        else
                        {
                            Flights.Add(CreateFlightMCCv4(csvline));
                        }
                    }
                    catch (Exception ey)
                    {
                        _ErrorOccured = true;

                        File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " MCC version: " + MCCVersion + " error parsing, skipping line: " + i.ToString() + "\n Import_MCC_pilotLog_CSV:\n" + ey.ToString() + "\n");
                    }
                    i++;
                }
                if (Flights.Count < 1)
                {
                    _ErrorOccured = true;
                    File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " Import_MCC_CSV: found no Flights to parse.\n");
                }
            }
        }

        public bool GetError()
        {
            return _ErrorOccured;
        }

        public List<Flight> GetFlightList()
        {
            return Flights;
        }

        private Flight CreateFlightMCCv3(string[] csvline)
        {
            StartDate = DateTime.MinValue;
            Starttime = TimeSpan.Zero;
            FROM = "";
            Endtime = TimeSpan.Zero;
            TO = "";
            Aircraft = "";
            Type = "";
            MultiPilotTime = TimeSpan.Zero;
            TotalTimeOfFlight = TimeSpan.Zero;
            PIC = "";
            DayLanding = 0;
            NightLanding = 0;
            NightTime = TimeSpan.Zero;
            IFRTime = TimeSpan.Zero;
            PICTime = TimeSpan.Zero;
            CopilotTime = TimeSpan.Zero;
            DualTime = TimeSpan.Zero;
            InstructorTime = TimeSpan.Zero;
            date_of_sim = DateTime.MinValue;
            Type_of_sim = "";
            sim_time = TimeSpan.Zero;
            remarks = "";
            pagebreak = false;
            _SEPTime = false;
            _MEPTime = false;

            StartDate = new DateTime(int.Parse(csvline[0].Substring(6, 4)), int.Parse(csvline[0].Substring(3, 2)), int.Parse(csvline[0].Substring(0, 2)));

            FROM = csvline[4];

            if (!TimeSpan.TryParse(csvline[5], out Starttime))
            {
                Starttime = TimeSpan.Zero;
            }

            TO = csvline[7];

            if (!TimeSpan.TryParse(csvline[8], out Endtime))
            {
                Endtime = TimeSpan.Zero;
            }

            Type = csvline[10];

            Aircraft = csvline[11];

            if (csvline[31].Length > 0)
            {
                MultiPilotTime = TimeSpan.FromMinutes(int.Parse(csvline[31]));
            }

            if (csvline[28].Length > 0)
            {
                TotalTimeOfFlight = TimeSpan.FromMinutes(int.Parse(csvline[28]));
            }

            PIC = csvline[13];

            if (!int.TryParse(csvline[44], out DayLanding) || csvline[41].Contains("false"))
            {
                DayLanding = 0;
            }

            if (!int.TryParse(csvline[45], out NightLanding) || csvline[41].Contains("false"))
            {
                NightLanding = 0;
            }

            if (csvline[35].Length > 0)
            {
                NightTime = TimeSpan.FromMinutes(int.Parse(csvline[35]));
            }

            if (csvline[37].Length > 0)
            {
                IFRTime = TimeSpan.FromMinutes(int.Parse(csvline[37]));
            }

            if (csvline[29].Length > 0)
            {
                PICTime = TimeSpan.FromMinutes(int.Parse(csvline[29]));
            }

            if (csvline[31].Length > 0)
            {
                CopilotTime = TimeSpan.FromMinutes(int.Parse(csvline[31]));
            }

            if (csvline[32].Length > 0)
            {
                DualTime = TimeSpan.FromMinutes(int.Parse(csvline[32]));
            }

            if (csvline[33].Length > 0)
            {
                InstructorTime = TimeSpan.FromMinutes(int.Parse(csvline[33]));
            }

            if (!DateTime.TryParse(csvline[0], out date_of_sim) || !csvline[2].Contains("sim"))
            {
                date_of_sim = DateTime.MinValue;
            }
            if (csvline[28].Length > 0 && csvline[2].Contains("sim"))
            {
                sim_time = TotalTimeOfFlight;
            }
            if (csvline[2].Contains("sim"))
            {
                Type_of_sim = csvline[10];
            }
            remarks = csvline[50];
            // PICUS TIME
            if (csvline[30].Length > 0)
            {
                int i = 0;
                int.TryParse(csvline[30], out i);
                if (i > 0)
                {
                    PICTime = TimeSpan.FromMinutes(int.Parse(csvline[30]));
                    DualTime = TimeSpan.FromMinutes(int.Parse(csvline[30]));
                }
            }

            // SIM
            if (csvline[2].Contains("sim"))
            {
                return new Flight(StartDate, "", TimeSpan.Zero, "", TimeSpan.Zero, "", "", _SEPTime, _MEPTime, TimeSpan.Zero, TimeSpan.Zero, "", DayLanding, NightLanding, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, date_of_sim, Type_of_sim, sim_time, remarks, pagebreak);
            }
            // No SIM
            else
            {
                return new Flight(StartDate, FROM, Starttime, TO, Endtime, Type, Aircraft, _SEPTime, _MEPTime, MultiPilotTime, TotalTimeOfFlight, PIC, DayLanding, NightLanding, NightTime, IFRTime, PICTime, CopilotTime, DualTime, InstructorTime, DateTime.MinValue, "", TimeSpan.Zero, remarks, pagebreak);
            }
        }

        private Flight CreateFlightMCCv4(string[] csvline)
        {
            StartDate = DateTime.MinValue;
            Starttime = TimeSpan.Zero;
            FROM = "";
            Endtime = TimeSpan.Zero;
            TO = "";
            Aircraft = "";
            Type = "";
            MultiPilotTime = TimeSpan.Zero;
            TotalTimeOfFlight = TimeSpan.Zero;
            PIC = "";
            DayLanding = 0;
            NightLanding = 0;
            NightTime = TimeSpan.Zero;
            IFRTime = TimeSpan.Zero;
            PICTime = TimeSpan.Zero;
            CopilotTime = TimeSpan.Zero;
            DualTime = TimeSpan.Zero;
            InstructorTime = TimeSpan.Zero;
            date_of_sim = DateTime.MinValue;
            Type_of_sim = "";
            sim_time = TimeSpan.Zero;
            remarks = "";
            pagebreak = false;
            _SEPTime = false;
            _MEPTime = false;

            DateTime.TryParse(csvline[0], out StartDate);
            FROM = csvline[4];
            TimeSpan.TryParse(csvline[5], out Starttime);
            TO = csvline[6];
            TimeSpan.TryParse(csvline[7], out Endtime);
            Type = csvline[8];
            Aircraft = csvline[9];
            if (csvline[29].Length > 0)
            {
                MultiPilotTime = TimeSpan.FromMinutes(int.Parse(csvline[29]));
            }
            if (csvline[26].Length > 0)
            {
                TotalTimeOfFlight = TimeSpan.FromMinutes(int.Parse(csvline[26]));
            }
            PIC = csvline[11];
            if (!int.TryParse(csvline[42], out DayLanding) || csvline[40].Contains("FALSE"))
            {
                DayLanding = 0;
            }
            if (!int.TryParse(csvline[43], out NightLanding) || csvline[40].Contains("FALSE"))
            {
                NightLanding = 0;
            }
            if (csvline[33].Length > 0)
            {
                NightTime = TimeSpan.FromMinutes(int.Parse(csvline[33]));
            }
            if (csvline[35].Length > 0)
            {
                IFRTime = TimeSpan.FromMinutes(int.Parse(csvline[35]));
            }
            if (csvline[27].Length > 0)
            {
                PICTime = TimeSpan.FromMinutes(int.Parse(csvline[27]));
            }
            if (csvline[29].Length > 0)
            {
                CopilotTime = TimeSpan.FromMinutes(int.Parse(csvline[29]));
            }
            if (csvline[30].Length > 0)
            {
                DualTime = TimeSpan.FromMinutes(int.Parse(csvline[30]));
            }
            if (csvline[31].Length > 0)
            {
                InstructorTime = TimeSpan.FromMinutes(int.Parse(csvline[31]));
            }
            if (!DateTime.TryParse(csvline[0], out date_of_sim) || csvline[2].Contains("FALSE"))
            {
                date_of_sim = DateTime.MinValue;
            }
            if (csvline[26].Length > 0)
            {
                sim_time = TimeSpan.FromMinutes(int.Parse(csvline[26]));
            }
            if (csvline[2].Contains("TRUE"))
            {
                Type_of_sim = csvline[8];
            }
            remarks = csvline[48];
            // PICUS TIME
            if (csvline[28].Length > 0)
            {
                int i = 0;
                int.TryParse(csvline[28], out i);
                if (i > 0)
                {
                    PICTime = TimeSpan.FromMinutes(int.Parse(csvline[28]));
                    DualTime = TimeSpan.FromMinutes(int.Parse(csvline[28]));
                }
            }
            // SIM
            if (csvline[2].Contains("TRUE"))
            {
                return new Flight(StartDate, "", null, "", null, "", "", _SEPTime, _MEPTime, TimeSpan.Zero, TimeSpan.Zero, "", DayLanding, NightLanding, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, date_of_sim, Type_of_sim, sim_time, remarks, pagebreak);
            }
            // No SIM
            else
            {
                return new Flight(StartDate, FROM, Starttime, TO, Endtime, Type, Aircraft, _SEPTime, _MEPTime, MultiPilotTime, TotalTimeOfFlight, PIC, DayLanding, NightLanding, NightTime, IFRTime, PICTime, CopilotTime, DualTime, InstructorTime, null, "", TimeSpan.MinValue, remarks, pagebreak);
            }
        }
    }
}