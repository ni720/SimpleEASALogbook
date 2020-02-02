using System;

namespace SimpleEASALogbook
{
    public class Flight : IComparable
    {
        private DateTime FlightDate = DateTime.MinValue;
        private TimeSpan OnBlockTime = TimeSpan.Zero;
        private string DepartureAirport = "";
        private TimeSpan OffBlockTime = TimeSpan.Zero;
        private string DestinationAirport = "";
        private string TypeOfAircraft = "";
        private string AircraftRegistration = "";
        private TimeSpan SEPTime = TimeSpan.Zero;
        private TimeSpan MEPTime = TimeSpan.Zero;
        private TimeSpan MultiPilotTime = TimeSpan.Zero;
        private TimeSpan TotalTimeOfFlight = TimeSpan.Zero;
        private string PilotInCommand = "";
        private int DayLandings = 0;
        private int NightLandings = 0;
        private TimeSpan NightTime = TimeSpan.Zero;
        private TimeSpan IFRTime = TimeSpan.Zero;
        private TimeSpan PICTime = TimeSpan.Zero;
        private TimeSpan CopilotTime = TimeSpan.Zero;
        private TimeSpan DualTime = TimeSpan.Zero;
        private TimeSpan InstructorTime = TimeSpan.Zero;
        private DateTime DateOfSim = DateTime.MinValue;
        private string TypeOfSim = "";
        private TimeSpan SimTime = TimeSpan.Zero;
        private string Remarks = "";
        private bool nextpageafter = false;

        public Flight(DateTime date, TimeSpan offblock, string dep, TimeSpan onblock, string dest, string type, string reg, TimeSpan septime, TimeSpan meptime, TimeSpan multitime, TimeSpan totaltime, string pic, int ldgday, int ldgnight, TimeSpan nighttime, TimeSpan ifrtime, TimeSpan pictime, TimeSpan copitime, TimeSpan dualtime, TimeSpan instructortime, DateTime dateofsim, string typeofsim, TimeSpan simtime, string remarks, bool nextpage)
        {
            FlightDate = date;
            OffBlockTime = offblock;
            DepartureAirport = dep;
            OnBlockTime = onblock;
            DestinationAirport = dest;
            TypeOfAircraft = type;
            AircraftRegistration = reg;
            SEPTime = septime;
            MEPTime = meptime;
            MultiPilotTime = multitime;
            TotalTimeOfFlight = totaltime;
            PilotInCommand = pic;
            DayLandings = ldgday;
            NightLandings = ldgnight;
            NightTime = nighttime;
            IFRTime = ifrtime;
            PICTime = pictime;
            CopilotTime = copitime;
            DualTime = dualtime;
            InstructorTime = instructortime;
            DateOfSim = dateofsim;
            TypeOfSim = typeofsim;
            SimTime = simtime;
            Remarks = remarks;
            nextpageafter = nextpage;
        }

        public string getDateString()
        {
            if (FlightDate.Equals(DateTime.MinValue))
            {
                return "";
            }
            else
            {
                return FlightDate.ToShortDateString();
            }
        }

        public string getDepartureString()
        {
            return DepartureAirport;
        }

        public string getOffBlockTimeString()
        {
            if (OnBlockTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return OffBlockTime.ToString().Substring(0, 5);
            }
        }

        public string getDestinationString()
        {
            return DestinationAirport;
        }

        public string getOnBlockTimeString()
        {
            if (OnBlockTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return OnBlockTime.ToString().Substring(0, 5);
            }
        }
        public string getTypeOfAircraftString()
        {
            return TypeOfAircraft;
        }

        public string getRegistrationString()
        {
            return AircraftRegistration;
        }

        public string getSEPTimeString()
        {
            if (SEPTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return SEPTime.ToString().Substring(0, 5);
            }
        }
        public string getMEPTimeString()
        {
            if (MEPTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return MEPTime.ToString().Substring(0, 5);
            }
        }

        public string getMultiPilotTimeString()
        {
            if (MultiPilotTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return MultiPilotTime.ToString().Substring(0, 5);
            }
        }

        public string getTotalTimeString()
        {
            if (TotalTimeOfFlight.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return TotalTimeOfFlight.ToString().Substring(0, 5);
            }
        }

        public string getPICNameString()
        {
            return PilotInCommand;
        }

        public string getDayLDGString()
        {
            if (DayLandings > 0)
            {
                return DayLandings.ToString();
            }
            else
            {
                return "";
            }
        }

        public string getNightLDGString()
        {
            if (NightLandings > 0)
            {
                return NightLandings.ToString();
            }
            else
            {
                return "";
            }
        }

        public string getNightTimeString()
        {
            if (NightTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return NightTime.ToString().Substring(0, 5);
            }
        }

        public string getIFRTimeString()
        {
            if (IFRTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return IFRTime.ToString().Substring(0, 5);
            }
        }

        public string getPICTimeString()
        {
            if (PICTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return PICTime.ToString().Substring(0, 5);
            }
        }

        public string getCopilotTimeString()
        {
            if (CopilotTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return CopilotTime.ToString().Substring(0, 5);
            }
        }
        public string getDualTimeString()
        {
            if (DualTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return DualTime.ToString().Substring(0, 5);
            }
        }
        public string getInstructorTimeString()
        {
            if (InstructorTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return InstructorTime.ToString().Substring(0, 5);
            }
        }
        public string getSimDateString()
        {
            if (DateOfSim.Equals(DateTime.MinValue))
            {
                return "";
            }
            else
            {
                return DateOfSim.ToShortDateString();
            }
        }

        public string getSimTypeString()
        {
            return TypeOfSim;
        }

        public string getSimTimeString()
        {
            if (SimTime.Equals(TimeSpan.Zero))
            {
                return "";
            }
            else
            {
                return SimTime.ToString().Substring(0, 5);
            }
        }
        public string getRemarksString()
        {
            return Remarks;
        }
        public string getPageBreakString()
        {
            if (nextpageafter)
            {
                return "pagebreak";
            }
            else
            {
                return "";
            }
        }
        public bool hasPageBreak()
        {
            return nextpageafter;
        }

        // this makes is possible to sort per date
        public int CompareTo(object obj)
        {
            Flight orderToCompare = obj as Flight;
            if (orderToCompare.FlightDate < FlightDate)
            {
                return 1;
            }
            if (orderToCompare.FlightDate > FlightDate)
            {
                return -1;
            }
            if(orderToCompare.FlightDate == FlightDate)
            {
                if (orderToCompare.DateOfSim < DateOfSim)
                {
                    return 1;
                }
                if (orderToCompare.DateOfSim > DateOfSim)
                {
                    return -1;
                }
            }
            // The orders are equivalent.
            return 0;
        }
    }
}
