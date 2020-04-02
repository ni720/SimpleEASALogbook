using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleEASALogbook
{
    class Import_MCC_CSV
    {
        DateTime StartDate = DateTime.MinValue;
        TimeSpan Starttime = TimeSpan.Zero;
        string FROM = "";
        TimeSpan Endtime = TimeSpan.Zero;
        string TO = "";
        string Aircraft = "";
        string Type = "";
        //TimeSpan SETime = TimeSpan.Zero;  // MCC Pilotlog does not correctly export SEP Time
        //TimeSpan METime = TimeSpan.Zero;  // MCC Pilotlog does not correctly export MEP Time
        TimeSpan MultiPilotTime = TimeSpan.Zero;
        TimeSpan TotalTimeOfFlight = TimeSpan.Zero;
        string PIC = "";
        int DayLanding = 0;
        int NightLanding = 0;
        TimeSpan NightTime = TimeSpan.Zero;
        TimeSpan IFRTime = TimeSpan.Zero;
        TimeSpan PICTime = TimeSpan.Zero;
        TimeSpan CopilotTime = TimeSpan.Zero;
        TimeSpan DualTime = TimeSpan.Zero;
        TimeSpan InstructorTime = TimeSpan.Zero;
        DateTime date_of_sim = DateTime.MinValue;
        string Type_of_sim = "";
        TimeSpan sim_time = TimeSpan.Zero;
        string remarks = "";
        bool nextpageafter = false;
        List<Flight> Flights = new List<Flight>();

        public Import_MCC_CSV(string mcccsv)
        {

            bool isOldMCCPilotLog = false;
            if (mcccsv.Contains("TIME_DEPSCH"))
            {
                Console.WriteLine("sensed MCCPilotLog_v3");
                isOldMCCPilotLog = true;
            }
            else
            {
                Console.WriteLine("sensed MCCPilotLog_v4");
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
                        if (isOldMCCPilotLog)
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
                        Console.WriteLine("error parsing, skipping line: " + i.ToString());
                        Console.WriteLine(ey.ToString());
                    }

                    i++;
                }

            }
        }
        Flight CreateFlightMCCv4(string[] csvline)
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
            nextpageafter = false;

            DateTime.TryParse(csvline[0], out StartDate);

            FROM = csvline[4];
            TimeSpan.TryParse(csvline[5], out Starttime);
            TO = csvline[6];
TimeSpan.TryParse(csvline[7], out Endtime);
                                 
            Type = csvline[8];
            Aircraft = csvline[9];
            PIC = csvline[11];
            if (csvline[26].Length > 0)
            {
                TotalTimeOfFlight = TimeSpan.FromMinutes(int.Parse(csvline[26]));
            }
            if (csvline[27].Length > 0)
            {
                PICTime = TimeSpan.FromMinutes(int.Parse(csvline[27]));
            }
            if (csvline[30].Length > 0)
            {
                DualTime = TimeSpan.FromMinutes(int.Parse(csvline[30]));
            }
            if (csvline[29].Length > 0)
            {
                CopilotTime = TimeSpan.FromMinutes(int.Parse(csvline[29]));
            }
            if (csvline[31].Length > 0)
            {
                InstructorTime = TimeSpan.FromMinutes(int.Parse(csvline[31]));
            }
            if (csvline[33].Length > 0)
            {
                NightTime = TimeSpan.FromMinutes(int.Parse(csvline[33]));
            }
            if (csvline[35].Length > 0)
            {
                IFRTime = TimeSpan.FromMinutes(int.Parse(csvline[35]));
            }
            if (!int.TryParse(csvline[42], out DayLanding) || csvline[40].Contains("FALSE"))
            {
                DayLanding = 0;
            }
            if (!int.TryParse(csvline[43], out NightLanding) || csvline[40].Contains("FALSE"))
            {
                NightLanding = 0;
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
            if (csvline[48].Length < 1 && csvline[2].Contains("TRUE"))
            {
                remarks += csvline[10];
            }
            if (csvline[29].Length > 0)
            {
                MultiPilotTime = TimeSpan.FromMinutes(int.Parse(csvline[29]));
            }

            // SIM
            if (csvline[2].Contains("TRUE"))
            {
                return new Flight(StartDate, "", null, "", null, "", "", TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, date_of_sim, Type_of_sim, sim_time, remarks, nextpageafter);
            }
            // No SIM
            else
            {
                return new Flight(StartDate, FROM, Starttime, TO, Endtime, Type, Aircraft, TimeSpan.Zero, TimeSpan.Zero, MultiPilotTime, TotalTimeOfFlight, PIC, DayLanding, NightLanding, NightTime, IFRTime, PICTime, CopilotTime, DualTime, InstructorTime, null, "", TimeSpan.MinValue, remarks, nextpageafter);
            }
        }
        Flight CreateFlightMCCv3(string[] csvline)
        {
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

            PIC = csvline[13];

            if (csvline[28].Length > 0)
            {
                TotalTimeOfFlight = TimeSpan.FromMinutes(int.Parse(csvline[28]));
            }

            if (csvline[30].Length > 0)
            {
                PICTime = TimeSpan.FromMinutes(int.Parse(csvline[30]));
            }

            if (csvline[32].Length > 0)
            {
                DualTime = TimeSpan.FromMinutes(int.Parse(csvline[32]));
            }

            if (csvline[31].Length > 0)
            {
                CopilotTime = TimeSpan.FromMinutes(int.Parse(csvline[31]));
            }

            if (csvline[33].Length > 0)
            {
                InstructorTime = TimeSpan.FromMinutes(int.Parse(csvline[33]));
            }

            if (csvline[35].Length > 0)
            {
                NightTime = TimeSpan.FromMinutes(int.Parse(csvline[35]));
            }

            if (csvline[37].Length > 0)
            {
                IFRTime = TimeSpan.FromMinutes(int.Parse(csvline[37]));
            }

            if (!int.TryParse(csvline[44], out DayLanding) || csvline[41].Contains("false"))
            {
                DayLanding = 0;
            }

            if (!int.TryParse(csvline[45], out NightLanding) || csvline[41].Contains("false"))
            {
                NightLanding = 0;
            }

            if (!DateTime.TryParse(csvline[0], out date_of_sim) || !csvline[2].Contains("sim"))
            {
                date_of_sim = DateTime.MinValue;
            }
            if (csvline[28].Length > 0 && !csvline[2].Contains("sim"))
            {
                sim_time = TimeSpan.FromMinutes(int.Parse(csvline[28]));
            }
            if (csvline[2].Contains("sim"))
            {
                Type_of_sim = csvline[10];
            }

            remarks = csvline[50];

            if (csvline[50].Length < 1 && csvline[2].Contains("sim"))
            {
                remarks += csvline[12];
            }

            //----
            if (csvline[31].Length > 0)
            {
                MultiPilotTime = TimeSpan.FromMinutes(int.Parse(csvline[31]));
            }

            // ---

            if (csvline.Length > 63 && csvline[63].Equals("pagebreak"))
            {
                nextpageafter = true;
            }
            else
            {
                nextpageafter = false;
            }
            // SIM
            if (csvline[2].Contains("sim"))
            {
                return new Flight(StartDate, "", TimeSpan.MinValue, "", TimeSpan.MinValue, "", "", TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, date_of_sim, Type_of_sim, sim_time, remarks, nextpageafter);
            }
            // No SIM
            else
            {
                return new Flight(StartDate, FROM, Starttime, TO, Endtime, Type, Aircraft, TimeSpan.Zero, TimeSpan.Zero, MultiPilotTime, TotalTimeOfFlight, PIC, DayLanding, NightLanding, NightTime, IFRTime, PICTime, CopilotTime, DualTime, InstructorTime, DateTime.MinValue, "", TimeSpan.Zero, remarks, nextpageafter);
            }
        }
        public List<Flight> GetFlightList()
        {
            return Flights;
        }
    }
}
