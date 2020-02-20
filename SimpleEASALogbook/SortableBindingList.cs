using System;
using System.Collections.Generic;
using System.ComponentModel;


// credits to: https://github.com/microsoftarchive/msdn-code-gallery-community-s-z/blob/master/SQL%20Server%20Table%20Trigger%20practical%20example%20with%20Entity%20Framework/%5BC%23%5D-SQL%20Server%20Table%20Trigger%20practical%20example%20with%20Entity%20Framework/C%23/Operations_cs/SortableBindingList.cs

namespace SimpleEASALogbook
{
    public class SortableBindingList<T> : BindingList<T> where T : class
    {
        private bool _isSorted;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor _sortProperty;

        public SortableBindingList()
        {
        }
        public SortableBindingList(IList<T> list) : base(list)
        {
        }
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }
        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }
        protected override ListSortDirection SortDirectionCore
        {
            get { return _sortDirection; }
        }
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _sortProperty; }
        }
        protected override void RemoveSortCore()
        {
            _sortDirection = ListSortDirection.Ascending;
            _sortProperty = null;
            _isSorted = false; //thanks Luca
        }
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortProperty = prop;
            _sortDirection = direction;

            List<T> list = Items as List<T>;
            if (list == null) return;

            list.Sort(Compare);

            _isSorted = true;
            //fire an event that the list has been changed.
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        private int Compare(T lhs, T rhs)
        {
            var result = OnComparison(lhs, rhs);
            //invert if descending
            if (_sortDirection == ListSortDirection.Descending)
                result = -result;
            return result;
        }
        private int OnComparison(T lhs, T rhs)
        {
            object lhsValue = lhs == null ? null : _sortProperty.GetValue(lhs);
            object rhsValue = rhs == null ? null : _sortProperty.GetValue(rhs);

            // apply different sorting logic when sorting by flight date
            if (_sortProperty.Name.Equals("FlightDate"))
            {
                return CompareFLTDate(lhs, rhs);
            }
            if (lhsValue == null)
            {
                return (rhsValue == null) ? 0 : -1; //nulls are equal
            }
            if (rhsValue == null)
            {
                return 1; //first has value, second doesn't
            }
            if (lhsValue.Equals(rhsValue))
            {

                return 0; //both are the same
            }
            if (lhsValue is IComparable)
            {
                return ((IComparable)lhsValue).CompareTo(rhsValue);
            }
            //not comparable, compare ToString
            return lhsValue.ToString().CompareTo(rhsValue.ToString());
        }
        // this makes is possible to sort per date.
        // flight date over sim date. and offblocktime within flight dates.
        private int CompareFLTDate(object x, object y)
        {
            Flight lhs = x as Flight;
            Flight rhs = y as Flight;
            // make nullable values comparable

            DateTime a = DateTime.MinValue;
            DateTime b = DateTime.MinValue;
            TimeSpan c = TimeSpan.Zero;
            TimeSpan d = TimeSpan.Zero;

            if (lhs.DateOfSim.HasValue)
            {
                if (lhs.DateOfSim.Value.Ticks > DateTime.MinValue.Ticks)
                {
                    a = lhs.DateOfSim.Value;
                }
            }
            if (lhs.FlightDate.HasValue)
            {
                if (lhs.FlightDate.Value.Ticks > DateTime.MinValue.Ticks)
                {
                    a = lhs.FlightDate.Value;
                }
            }
            if (rhs.DateOfSim.HasValue)
            {
                if (rhs.DateOfSim.Value.Ticks > DateTime.MinValue.Ticks)
                {
                    b = rhs.DateOfSim.Value;
                }
            }
            if (rhs.FlightDate.HasValue)
            {
                if (rhs.FlightDate.Value.Ticks > DateTime.MinValue.Ticks)
                {
                    b = rhs.FlightDate.Value;
                }

            }
            if (lhs.OffBlockTime.HasValue)
            {
                if (lhs.OffBlockTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    c = lhs.OffBlockTime.Value;
                }
            }
            if (rhs.OffBlockTime.HasValue)
            {
                if (rhs.OffBlockTime.Value.Ticks > TimeSpan.Zero.Ticks)
                {
                    d = rhs.OffBlockTime.Value;
                }
            }
            // compare values
            if (a.Ticks < b.Ticks)
            {
                return -1;
            }
            if (a.Ticks > b.Ticks)
            {
                return 1;
            }
            if (a.Ticks == b.Ticks)
            {
                if (c.Ticks < d.Ticks)
                {
                    return -1;
                }
                if (c.Ticks > d.Ticks)
                {
                    return 1;
                }
            }
            // The orders are equivalent.
            return 0;
        }
        public void Sort(string propertyName, ListSortDirection direction)
        {
            this.ApplySortCore(TypeDescriptor.GetProperties(typeof(T))[propertyName], direction);
        }
        public List<Flight> GetFlights()
        {
            return Items as List<Flight>;
        }
    }
}
