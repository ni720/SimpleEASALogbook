using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleEASALogbook
{
    class Import_EASA_CSV
    {

        DateTime Starttime = DateTime.MinValue;
        string FROM = "";
        DateTime Endtime = DateTime.MinValue;
        string TO = "";
        string Aircraft = "";
        string Type = "";
        TimeSpan SETime = TimeSpan.Zero;
        TimeSpan METime = TimeSpan.Zero;
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
        public Import_EASA_CSV(string stringToParse)
        {
            using (var reader = new StringReader(stringToParse))
            {
                int i = 0;
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    try
                    {
                        string[] csvline = new string[25];
                        csvline = line.Split(';');
                        Flights.Add(CreateFlight(csvline));

                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("error parsing, skipping line: " + i.ToString());
                        File.WriteAllText("logfile.txt", exc.ToString());
                    }

                    i++;
                }
            }
        }
        Flight CreateFlight(string[] csvline)
        {
           /* if((csvline[0].Length<1 && csvline[2].Length<1 && csvline[4].Length <1 )||( csvline[20].Length<1 && csvline[22].Length<1))
            {
                throw new Exception("not enough data to store a flight");
            }*/
            if (csvline[0].Length>0)
            {
                //Starttime = new DateTime(int.Parse(csvline[0].Substring(6, 4)), int.Parse(csvline[0].Substring(3, 2)), int.Parse(csvline[0].Substring(0, 2)), int.Parse(csvline[2].Substring(0, 2)), int.Parse(csvline[2].Substring(3, 2)), 0);
                if (!DateTime.TryParse(csvline[0], out Starttime))
                {
                    throw new Exception("unable to parse date");
                }
                else
                {
                    TimeSpan begin;
                    if (!TimeSpan.TryParse(csvline[2], out begin))
                    {
                        throw new Exception("unable to parse time");
                    }
                    else
                    {
                        Starttime.Add(begin);
                    }
                }
            }
            else
            {
                if (!DateTime.TryParse(csvline[20], out Starttime))
                {
                    date_of_sim = DateTime.MinValue;
                }
            }

            FROM = csvline[1];

            if (csvline[0].Length > 0 && csvline[4].Length > 0)
            {
                //Endtime = new DateTime(int.Parse(csvline[0].Substring(6, 4)), int.Parse(csvline[0].Substring(3, 2)), int.Parse(csvline[0].Substring(0, 2)), int.Parse(csvline[4].Substring(0, 2)), int.Parse(csvline[4].Substring(3, 2)), 0);
                Endtime = Starttime;
                    TimeSpan end;
                    if(!TimeSpan.TryParse(csvline[4],out end))
                    {
                        throw new Exception("unable to parse date");
                    }else
                    {
                        Endtime.Add(end);
                    }
                
            }
            else
            {
                if (!DateTime.TryParse(csvline[20], out Endtime))
                {
                    date_of_sim = DateTime.MinValue;
                }
            }

            TO = csvline[3];

            Type = csvline[5];

            Aircraft = csvline[6];

            if (!TimeSpan.TryParse(csvline[7], out SETime))
            {
                SETime = TimeSpan.Zero;
            }

            if (!TimeSpan.TryParse(csvline[8], out METime))
            {
                METime = TimeSpan.Zero;
            }

            if (!TimeSpan.TryParse(csvline[9], out MultiPilotTime))
            {
                MultiPilotTime = TimeSpan.Zero;
            }

            if (!TimeSpan.TryParse(csvline[10], out TotalTimeOfFlight))
            {
                TotalTimeOfFlight = TimeSpan.Zero;
            }

            PIC = csvline[11];

            if (!int.TryParse(csvline[12], out DayLanding))
            {
                DayLanding = 0;
            }

            if (!int.TryParse(csvline[13], out NightLanding))
            {
                NightLanding = 0;
            }

            if (!TimeSpan.TryParse(csvline[14], out NightTime))
            {
                NightTime = TimeSpan.Zero;
            }

            if (!TimeSpan.TryParse(csvline[15], out IFRTime))
            {
                IFRTime = TimeSpan.Zero;
            }

            if (!TimeSpan.TryParse(csvline[16], out PICTime))
            {
                PICTime = TimeSpan.Zero;
            }

            if (!TimeSpan.TryParse(csvline[17], out CopilotTime))
            {
                CopilotTime = TimeSpan.Zero;
            }

            if (!TimeSpan.TryParse(csvline[18], out DualTime))
            {
                DualTime = TimeSpan.Zero;
            }

            if (!TimeSpan.TryParse(csvline[19], out InstructorTime))
            {
                InstructorTime = TimeSpan.Zero;
            }

            if (!DateTime.TryParse(csvline[20], out date_of_sim))
            {
                date_of_sim = DateTime.MinValue;
            }
            

            Type_of_sim = csvline[21];
            if (!TimeSpan.TryParse(csvline[22], out sim_time))
            {
                sim_time = TimeSpan.Zero;
            }
            remarks = csvline[23];

            if (csvline[24].Length >1 )
            {
                //TODO: "pagebreak" as separator?
                    nextpageafter = true;                
            }
            else
            {
                nextpageafter = false;
            }

            // ---

            // SIM
            if (csvline[20].Length > 1)
            {
                return new Flight(Starttime, "", DateTime.MinValue, "", Type, Aircraft, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, date_of_sim, Type_of_sim, sim_time, remarks, nextpageafter);
            }
            // No SIM
            else
            {
                return new Flight(Starttime, FROM, Endtime, TO, Type, Aircraft, SETime, METime, MultiPilotTime, TotalTimeOfFlight, PIC, DayLanding, NightLanding, NightTime, IFRTime, PICTime, CopilotTime, DualTime, InstructorTime, DateTime.MinValue, "", TimeSpan.Zero, remarks, nextpageafter);
            }

        }
        public List<Flight> getFlightList()
        {
            return Flights;
        }
    }
}
