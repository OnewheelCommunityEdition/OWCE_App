using System;
namespace OWCE
{
    public class Ride
    {
        private DateTime _dateTime;

        public DateTime DateTime
        {
            get
            {
                return _dateTime;
            }
        }

        private string _directory;

        public string Directory
        {
            get
            {
                return _directory;
            }
        }

        public string TextDisplay {
            get { return _dateTime.ToShortDateString() + " " + _dateTime.ToShortTimeString(); }
        }

        public Ride()
        {
        }

        public Ride(DateTime dateTime, string directory)
        {
            _dateTime = dateTime;
            _directory = directory;
        }
    }
}
