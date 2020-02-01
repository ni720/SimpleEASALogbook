using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleEASALogbook
{
    public class Flight : IComparable
    {
        public DateTime OffBlockTime = DateTime.MinValue;
        public string DepartureAirport = "";
        public DateTime OnBlockTime = DateTime.MinValue;
        public string DestinationAirport = "";
        public string TypeOfAircraft = "";
        public string AircraftRegistration = "";
        public TimeSpan SEPTime = TimeSpan.Zero;
        public TimeSpan MEPTime = TimeSpan.Zero;
        public TimeSpan MultiPilotTime = TimeSpan.Zero;
        public TimeSpan TotalTimeOfFlight = TimeSpan.Zero;
        public string PilotInCommand = "";
        public int DayLandings = 0;
        public int NightLandings = 0;
        public TimeSpan NightTime = TimeSpan.Zero;
        public TimeSpan IFRTime = TimeSpan.Zero;
        public TimeSpan PICTime = TimeSpan.Zero;
        public TimeSpan CopilotTime = TimeSpan.Zero;
        public TimeSpan DualTime = TimeSpan.Zero;
        public TimeSpan InstructorTime = TimeSpan.Zero;
        public DateTime DateOfSim = DateTime.MinValue;
        public string TypeOfSim = "";
        public TimeSpan SimTime = TimeSpan.Zero;
        public string Remarks = "";
        public bool nextpageafter = false;


        public Flight(DateTime offblock, string dep, DateTime onblock, string dest, string type, string reg, TimeSpan septime, TimeSpan meptime, TimeSpan multitime, TimeSpan totaltime, string pic, int ldgday, int ldgnight, TimeSpan nighttime, TimeSpan ifrtime, TimeSpan pictime, TimeSpan copitime, TimeSpan dualtime, TimeSpan instructortime, DateTime dateofsim, string typeofsim, TimeSpan simtime, string remarks, bool nextpage)
        {
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

        public string getOffBlockTimeString()
        {
            if (OnBlockTime.Equals(DateTime.MinValue))
            {
                return "";
            }
            else
            {
                return OffBlockTime.ToShortTimeString();
            }
        }

        public string getOnBlockTimeString()
        {
            if (OnBlockTime.Equals(DateTime.MinValue))
            {
                return "";
            }
            else
            {
                return OnBlockTime.ToShortTimeString();
            }
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

        public int CompareTo(object obj)
        {
            Flight orderToCompare = obj as Flight;
            if (orderToCompare.OffBlockTime < OffBlockTime)
            {
                return 1;
            }
            if (orderToCompare.OffBlockTime > OffBlockTime)
            {
                return -1;
            }
            // The orders are equivalent.
            return 0;
        }
    }
}
