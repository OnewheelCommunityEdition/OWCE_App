using System;
using System.IO;
using Xamarin.Essentials;

namespace OWCE
{
    public class Ride
    {
        private DateTime _startTime;
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
            }
        }

        private string _file;
        public string File
        {
            get
            {
                return _file;
            }
        }

        public string TextDisplay
        {
            get { return _startTime.ToShortDateString() + " " + _startTime.ToShortTimeString(); }
        }

        public Ride()
        {
        }

        public Ride(string file)
        {
            _startTime = DateTime.Now;
            _file = file;
        }

        public string GetLogFilePath()
        {
            return Path.Combine(FileSystem.CacheDirectory, _file);
        }
    }
}
