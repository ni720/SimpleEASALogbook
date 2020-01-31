using System;
using System.Collections.Generic;

namespace SimpleEASALogbook
{
    class Export_EASA_CSV
    {
        protected string stringbuilder = "";
        public Export_EASA_CSV(List<Flight> flights)
        {
            foreach (Flight flight in flights)
            {
                stringbuilder += flight.OffBlockTime.ToShortDateString() + ";";

                if (flight.DepartureAirport.Equals(""))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += flight.DepartureAirport + ";";
                }

                if (flight.DateOfSim.Equals(DateTime.MinValue))
                {
                    stringbuilder += flight.OffBlockTime.ToShortTimeString() + ";";
                }
                else
                {
                    stringbuilder += ";";
                }

                if (flight.DestinationAirport.Equals(""))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += flight.DestinationAirport + ";";
                }

                if (flight.DateOfSim.Equals(DateTime.MinValue))
                {
                    stringbuilder += flight.OnBlockTime.ToShortTimeString() + ";";
                }
                else
                {
                    stringbuilder += ";";
                }

                if (flight.TypeOfAircraft.Equals(""))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += flight.TypeOfAircraft + ";";
                }

                if (flight.AircraftRegistration.Equals(""))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += flight.AircraftRegistration + ";";
                }

                if (flight.SEPTime.Equals(TimeSpan.Zero))
                {
                    stringbuilder += ";";
                }
                else
                {             
                    stringbuilder += ((int)flight.SEPTime.TotalHours).ToString() +":"+ flight.SEPTime.Minutes.ToString()+ ";";
                }

                if (flight.MEPTime.Equals(TimeSpan.Zero))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += ((int)flight.MEPTime.TotalHours).ToString() + ":" + flight.MEPTime.Minutes.ToString() + ";";
                }

                if (flight.MultiPilotTime.Equals(TimeSpan.Zero))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += ((int)flight.MultiPilotTime.TotalHours).ToString() + ":" + flight.MultiPilotTime.Minutes.ToString() + ";";
                }

                if (flight.TotalTimeOfFlight.Equals(TimeSpan.Zero))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += ((int)flight.TotalTimeOfFlight.TotalHours).ToString() + ":" + flight.TotalTimeOfFlight.Minutes.ToString() + ";";
                }

                if (flight.PilotInCommand.Equals(""))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += flight.PilotInCommand + ";";
                }

                if (flight.DayLandings < 1)
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += flight.DayLandings.ToString() + ";";
                }

                if (flight.NightLandings < 1)
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += flight.NightLandings.ToString() + ";";
                }

                if (flight.NightTime.Equals(TimeSpan.Zero))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += ((int)flight.NightTime.TotalHours).ToString() + ":" + flight.NightTime.Minutes.ToString() + ";";
                }

                if (flight.IFRTime.Equals(TimeSpan.Zero))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += ((int)flight.IFRTime.TotalHours).ToString() + ":" + flight.IFRTime.Minutes.ToString() + ";";
                }

                if (flight.PICTime.Equals(TimeSpan.Zero))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += ((int)flight.PICTime.TotalHours).ToString() + ":" + flight.PICTime.Minutes.ToString() + ";";
                }

                if (flight.CopilotTime.Equals(TimeSpan.Zero))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += ((int)flight.CopilotTime.TotalHours).ToString() + ":" + flight.CopilotTime.Minutes.ToString() + ";";
                }

                if (flight.DualTime.Equals(TimeSpan.Zero))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += ((int)flight.DualTime.TotalHours).ToString() + ":" + flight.DualTime.Minutes.ToString() + ";";
                }

                if (flight.InstructorTime.Equals(TimeSpan.Zero))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += ((int)flight.InstructorTime.TotalHours).ToString() + ":" + flight.InstructorTime.Minutes.ToString() + ";";
                }

                if (flight.DateOfSim.Equals(DateTime.MinValue))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += flight.DateOfSim.ToShortDateString() + ";";
                }

                if (flight.TypeOfSim.Equals(""))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += flight.TypeOfSim + ";";
                }

                if (flight.SimTime.Equals(TimeSpan.Zero))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += ((int)flight.SimTime.TotalHours).ToString() + ":" + flight.SimTime.Minutes.ToString() + ";";
                }

                if (flight.Remarks.Equals(""))
                {
                    stringbuilder += ";";
                }
                else
                {
                    stringbuilder += flight.Remarks + ";";
                }
                if(flight.nextpageafter)
                {
                    stringbuilder += "pagebreak;";
                }
                stringbuilder += "\n";
            }
        }

        public string GetCSV()
        {
            return stringbuilder;
        }
    }
}
