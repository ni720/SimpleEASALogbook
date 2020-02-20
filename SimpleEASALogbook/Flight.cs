using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// 2Do: sort all methods, definitions, need to change get-string-methods?, if(hasvalue) cases streamline

namespace SimpleEASALogbook
{
    public class Flight : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Nullable<DateTime> _FlightDate = null;
        private string _DepartureAirport = "";
        private Nullable<TimeSpan> _OffBlockTime = null;
        private string _DestinationAirport = "";
        private Nullable<TimeSpan> _OnBlockTime = null;
        private string _TypeOfAircraft = "";
        private string _AircraftRegistration = "";
        private Nullable<TimeSpan> _SEPTime = null;
        private Nullable<TimeSpan> _MEPTime = null;
        private Nullable<TimeSpan> _MultiPilotTime = null;
        private Nullable<TimeSpan> _TotalTimeOfFlight = null;
        private string _PilotInCommand = "";
        private Nullable<int> _DayLandings = null;
        private Nullable<int> _NightLandings = null;
        private Nullable<TimeSpan> _NightTime = null;
        private Nullable<TimeSpan> _IFRTime = null;
        private Nullable<TimeSpan> _PICTime = null;
        private Nullable<TimeSpan> _CopilotTime = null;
        private Nullable<TimeSpan> _DualTime = null;
        private Nullable<TimeSpan> _InstructorTime = null;
        private Nullable<DateTime> _DateOfSim = null;
        private string _TypeOfSim = "";
        private Nullable<TimeSpan> _SimTime = null;
        private string _Remarks = "";
        private bool _NextPageThereafter = false;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public Flight(DateTime date, TimeSpan offblock, string dep, TimeSpan onblock, string dest, string type, string reg, TimeSpan septime, TimeSpan meptime, TimeSpan multitime, TimeSpan totaltime, string pic, int ldgday, int ldgnight, TimeSpan nighttime, TimeSpan ifrtime, TimeSpan pictime, TimeSpan copitime, TimeSpan dualtime, TimeSpan instructortime, DateTime dateofsim, string typeofsim, TimeSpan simtime, string remarks, bool nextpage)
        {
            _FlightDate = date;
            _OffBlockTime = offblock;
            _DepartureAirport = dep;
            _OnBlockTime = onblock;
            _DestinationAirport = dest;
            _TypeOfAircraft = type;
            _AircraftRegistration = reg;
            _SEPTime = septime;
            _MEPTime = meptime;
            _MultiPilotTime = multitime;
            _TotalTimeOfFlight = totaltime;
            _PilotInCommand = pic;
            _DayLandings = ldgday;
            _NightLandings = ldgnight;
            _NightTime = nighttime;
            _IFRTime = ifrtime;
            _PICTime = pictime;
            _CopilotTime = copitime;
            _DualTime = dualtime;
            _InstructorTime = instructortime;
            _DateOfSim = dateofsim;
            _TypeOfSim = typeofsim;
            _SimTime = simtime;
            _Remarks = remarks;
            _NextPageThereafter = nextpage;
        }

        public string getDateString()
        {
            if (_FlightDate.HasValue)
            {
                if (_FlightDate.Value.Ticks > DateTime.MinValue.Ticks)
                {
                    return _FlightDate.Value.ToShortDateString()/*.Substring(0, 8)*/;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string getDepartureString()
        {
            return _DepartureAirport;
        }

        public string getOffBlockTimeString()
        {
            if (_OffBlockTime.HasValue)
            {
                if (_OffBlockTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _OffBlockTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string getDestinationString()
        {
            return _DestinationAirport;
        }

        public string getOnBlockTimeString()
        {
            if (_OnBlockTime.HasValue)
            {
                if (_OnBlockTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _OnBlockTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public string getTypeOfAircraftString()
        {
            return _TypeOfAircraft;
        }

        public string getRegistrationString()
        {
            return _AircraftRegistration;
        }

        public string getSEPTimeString()
        {
            if (_SEPTime.HasValue)
            {
                if (_SEPTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _SEPTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public string getMEPTimeString()
        {
            if (_MEPTime.HasValue)
            {
                if (_MEPTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _MEPTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string getMultiPilotTimeString()
        {
            if (_MultiPilotTime.HasValue)
            {
                if (_MultiPilotTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _MultiPilotTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string getTotalTimeString()
        {
            if (_TotalTimeOfFlight.HasValue)
            {
                if (_TotalTimeOfFlight.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _TotalTimeOfFlight.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string getPICNameString()
        {
            return _PilotInCommand;
        }

        public string getDayLDGString()
        {
            if (_DayLandings.HasValue)
            {
                if (_DayLandings.Value > 0)
                {
                    return _DayLandings.ToString();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string getNightLDGString()
        {
            if (_NightLandings.HasValue)
            {
                if (_NightLandings.Value > 0)
                {
                    return _NightLandings.ToString();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string getNightTimeString()
        {
            if (_NightTime.HasValue)
            {
                if (_NightTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _NightTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string getIFRTimeString()
        {
            if (_IFRTime.HasValue)
            {
                if (_IFRTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _IFRTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string getPICTimeString()
        {
            if (_PICTime.HasValue)
            {
                if (_PICTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _PICTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string getCopilotTimeString()
        {
            if (_CopilotTime.HasValue)
            {
                if (_CopilotTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _CopilotTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public string getDualTimeString()
        {
            if (_DualTime.HasValue)
            {
                if (_DualTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _DualTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public string getInstructorTimeString()
        {
            if (_InstructorTime.HasValue)
            {
                if (_InstructorTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _InstructorTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public string getSimDateString()
        {
            if (_DateOfSim.HasValue)
            {
                if (_DateOfSim.Value.Ticks > DateTime.MinValue.Ticks)
                {
                    return _DateOfSim.Value.ToShortDateString()/*.Substring(0, 8)*/;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return "";
            }
        }

        public string getSimTypeString()
        {
            return _TypeOfSim;
        }

        public string getSimTimeString()
        {
            if (_SimTime.HasValue)
            {
                if (_SimTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    return _SimTime.ToString().Substring(0, 5);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public string getRemarksString()
        {
            return _Remarks;
        }
        public string getPageBreakString()
        {
            if (_NextPageThereafter)
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
            return _NextPageThereafter;
        }

        public DateTime? FlightDate
        {
            get
            {
                if (_FlightDate.HasValue)
                {
                    if (_FlightDate.Value.Ticks > DateTime.MinValue.Ticks)
                    {
                        return _FlightDate.Value;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._FlightDate)
                {
                    this._FlightDate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public DateTime? DateOfSim
        {
            get
            {
                if (_DateOfSim.Value.Ticks > DateTime.MinValue.Ticks)
                {
                    return _DateOfSim.Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._DateOfSim)
                {
                    this._DateOfSim = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public TimeSpan? MultiPilotTime
        {
            get
            {
                if (_MultiPilotTime.HasValue)
                {
                    if (_MultiPilotTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _MultiPilotTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._MultiPilotTime)
                {
                    this._MultiPilotTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? SimTime
        {
            get
            {
                if (_SimTime.HasValue)
                {
                    if (_SimTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _SimTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._SimTime)
                {
                    this._SimTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? SEPTime
        {
            get
            {
                if (_SEPTime.HasValue)
                {
                    if (_SEPTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _SEPTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._SEPTime)
                {
                    this._SEPTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? MEPTime
        {
            get
            {
                if (_MEPTime.HasValue)
                {
                    if (_MEPTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _MEPTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._MEPTime)
                {
                    this._MEPTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? OffBlockTime
        {
            get
            {
                if (_OffBlockTime.HasValue)
                {
                    if (_OffBlockTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _OffBlockTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._OffBlockTime)
                {
                    this._OffBlockTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? OnBlockTime
        {
            get
            {
                if (_OnBlockTime.HasValue)
                {
                    if (_OnBlockTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _OnBlockTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._OnBlockTime)
                {
                    this._OnBlockTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? TotalTimeOfFlight
        {
            get
            {
                if (_TotalTimeOfFlight.HasValue)
                {
                    if (_TotalTimeOfFlight.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _TotalTimeOfFlight.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._TotalTimeOfFlight)
                {
                    this._TotalTimeOfFlight = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? NightTime
        {
            get
            {
                if (_NightTime.HasValue)
                {
                    if (_NightTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _NightTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._NightTime)
                {
                    this._NightTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? IFRTime
        {
            get
            {
                if (_IFRTime.HasValue)
                {
                    if (_IFRTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _IFRTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._IFRTime)
                {
                    this._IFRTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? PICTime
        {
            get
            {
                if (_PICTime.HasValue)
                {
                    if (_PICTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _PICTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._PICTime)
                {
                    this._PICTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? CopilotTime
        {
            get
            {
                if (_CopilotTime.HasValue)
                {
                    if (_CopilotTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _CopilotTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._CopilotTime)
                {
                    this._CopilotTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? DualTime
        {
            get
            {
                if (_DualTime.HasValue)
                {
                    if (_DualTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _DualTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._DualTime)
                {
                    this._DualTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public TimeSpan? InstructorTime
        {
            get
            {
                if (_InstructorTime.HasValue)
                {
                    if (_InstructorTime.Value.Ticks > TimeSpan.Zero.Ticks)
                    {
                        return _InstructorTime.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._InstructorTime)
                {
                    this._InstructorTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int? DayLandings
        {
            get
            {
                if (_DayLandings.HasValue)
                {
                    if (_DayLandings.Value > 0)
                    {
                        return _DayLandings.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._DayLandings)
                {
                    this._DayLandings = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int? NightLandings
        {
            get
            {
                if (_NightLandings.HasValue)
                {
                    if (_NightLandings.Value > 0)
                    {
                        return _NightLandings.Value;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != this._NightLandings)
                {
                    this._NightLandings = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string DepartureAirport
        {
            get
            {
                return _DepartureAirport;
            }
            set
            {
                if (value != this._DepartureAirport)
                {
                    this._DepartureAirport = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string DestinationAirport
        {
            get
            {
                return _DestinationAirport;
            }
            set
            {
                if (value != this._DestinationAirport)
                {
                    this._DestinationAirport = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string TypeOfAircraft
        {
            get
            {
                return _TypeOfAircraft;
            }
            set
            {
                if (value != this._TypeOfAircraft)
                {
                    this._TypeOfAircraft = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string AircraftRegistration
        {
            get
            {
                return _AircraftRegistration;
            }
            set
            {
                if (value != this._AircraftRegistration)
                {
                    this._AircraftRegistration = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string PilotInCommand
        {
            get
            {
                return _PilotInCommand;
            }
            set
            {
                if (value != this._PilotInCommand)
                {
                    this._PilotInCommand = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string TypeOfSim
        {
            get
            {
                return _TypeOfSim;
            }
            set
            {
                if (value != this._TypeOfSim)
                {
                    this._TypeOfSim = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Remarks
        {
            get
            {
                return _Remarks;
            }
            set
            {
                if (value != this._Remarks)
                {
                    this._Remarks = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool NextPageThereafter
        {
            get
            {
                return _NextPageThereafter;
            }
            set
            {
                if (value != this._NextPageThereafter)
                {
                    this._NextPageThereafter = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
