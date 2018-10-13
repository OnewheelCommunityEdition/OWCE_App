using System;
namespace OWCE
{
    public class OWBoardEvent
    {
        public string ID { get; internal set; }
        public UInt16? UInt16Data { get; internal set; } = null;
        public double? DoubleData { get; internal set; } = null;
        public Int16? Int16Data { get; internal set; } = null;
        public long MillisecondsTimestamp { get; internal set; }

        private OWBoardEvent(string id)
        {
            ID = id;
            MillisecondsTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public OWBoardEvent(string id, UInt16 data) : this(id)
        {
            UInt16Data = data;
        }

        public OWBoardEvent(string id, double data) : this(id)
        {
            DoubleData = data;
        }

        public OWBoardEvent(string id, Int16 data) : this(id)
        {
            Int16Data = data;
        }
    }
}
