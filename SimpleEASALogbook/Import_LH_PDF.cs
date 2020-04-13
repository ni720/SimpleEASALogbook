using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SimpleEASALogbook
{
    internal class Import_LH_PDF
    {
        private bool _ErrorOccured = false;
        private List<Flight> Flights = new List<Flight>();

        public Import_LH_PDF(string textToParse)
        {
            try
            {
                // define year
                string getyear = textToParse.Substring(textToParse.IndexOf("r Monat "), 20);
                int year = int.Parse(getyear.Substring(13, 4));
                int month = 0;
                int day = 0;
                int begin_hour = 0;
                int begin_min = 0;
                int end_hour = 0;
                int end_min = 0;
                string from = "", to = "";
                string aircraft = "", type = "";
                string PIC = "";
                DateTime date;
                TimeSpan offblock;
                TimeSpan onblock;
                TimeSpan duration;
                DateTime begin, end;

                Regex regexAllFlights = new Regex(@"\d\d\.\d\d\..*\/.*\/.*");
                Regex landings = new Regex(@"\ L\ ");
                Regex filter_dhFlights = new Regex(@"\w\w\w\ 01");
                Regex times = new Regex(@"\d\d\:\d\d\-\d\d:\d\d");
                Regex fromto = new Regex(@"\w\w\w\ \d*\:\d*\-\d*\:\d*\ \w\w\w");
                Regex acft = new Regex(@"\ \w*\d*\ \/\w*\d*\ \/");

                foreach (Match flightmatch in regexAllFlights.Matches(textToParse))
                {
                    if (filter_dhFlights.Matches(flightmatch.Value).Count > 0)
                    {
                        // disregard deadhead flights
                    }
                    else
                    {
                        month = int.Parse(flightmatch.Value.Substring(3, 2));
                        day = int.Parse(flightmatch.Value.Substring(0, 2));
                        Match Time = times.Match(flightmatch.Value);
                        begin_hour = int.Parse(Time.Value.Substring(0, 2));
                        begin_min = int.Parse(Time.Value.Substring(3, 2));
                        end_hour = int.Parse(Time.Value.Substring(6, 2));
                        end_min = int.Parse(Time.Value.Substring(9, 2));

                        date = new DateTime(year, month, day);
                        offblock = new TimeSpan(begin_hour, begin_min, 0);
                        onblock = new TimeSpan(end_hour, end_min, 0);
                        begin = new DateTime(year, month, day, begin_hour, begin_min, 0);
                        end = new DateTime(year, month, day, end_hour, end_min, 0);

                        if (end_hour < begin_hour)
                        {
                            end = end.AddHours(24);
                        }

                        duration = end.Subtract(begin);
                        Match FromTo = fromto.Match(flightmatch.Value);
                        from = FromTo.Value.Substring(0, 3);
                        to = FromTo.Value.Substring(FromTo.Value.Length - 3, 3);

                        Match ACFT = acft.Match(flightmatch.Value);
                        aircraft = ACFT.Value.Substring(0, ACFT.Value.IndexOf("/")).Trim();
                        type = ACFT.Value.Substring(ACFT.Value.IndexOf("/")).Replace("/", "").Trim();

                        PIC = flightmatch.Value.Substring(flightmatch.Value.LastIndexOf("/")).Replace("/", "").Trim();

                        // is SIM-flight
                        if (flightmatch.Value.Trim().EndsWith("/"))
                        {
                            Flights.Add(new Flight(null, "", null, "", null, "", "", false, false, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, begin, type, duration, aircraft, false));
                        }
                        else //no SIM
                        {
                            // with landing
                            if (landings.Matches(flightmatch.Value).Count > 0)
                            {
                                Flights.Add(new Flight(date, from, offblock, to, onblock, type, aircraft, false, false, duration, duration, PIC, 1, 0, TimeSpan.Zero, duration, TimeSpan.Zero, duration, TimeSpan.Zero, TimeSpan.Zero, null, "", TimeSpan.Zero, "", false));
                            }
                            else // without landing
                            {
                                Flights.Add(new Flight(date, from, offblock, to, onblock, type, aircraft, false, false, duration, duration, PIC, 0, 0, TimeSpan.Zero, duration, TimeSpan.Zero, duration, TimeSpan.Zero, TimeSpan.Zero, null, "", TimeSpan.Zero, "", false));
                            }
                        }
                    }
                }
                if (Flights.Count < 1)
                {
                    File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " Import_LH_PDF: found no Flights to parse.\n");
                    _ErrorOccured = true;
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " Import_LH_PDF:\n" + e.ToString() + "\n");
                _ErrorOccured = true;
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
    }
}