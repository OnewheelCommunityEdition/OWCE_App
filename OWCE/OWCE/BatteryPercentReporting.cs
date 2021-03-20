namespace OWCE
{
    using OWCE.Converters;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using Xamarin.Essentials;

    public class BatteryPercentReporting : TTSReporting
    {
        private readonly TimeSpan GapTimeReport = TimeSpan.FromSeconds(3);
        private readonly TimeSpan GapTimeReportSameValue = TimeSpan.FromMinutes(5);

        private bool _running = false;
        private DateTime _nextReportTime = DateTime.Now;
        private OWBoard _board;
        private Dictionary<int, DateTime> _announcedValues = new Dictionary<int, DateTime>();

        public BatteryPercentReporting(OWBoard board, TextToSpeechProvider ttsProvider) : base(ttsProvider)
        {
            _board = board;
            TextToSpeechPriority = 3;

            Debug.WriteLine($"Battery Percent Reporting created for board {board.Name}");
        }

        public override void Start()
        {
            if (_running)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("ACTIVATED Battery Percent detector");

            _board.BatteryPercentChanged += Board_BatteryPercentChanged;
        }

        public override void Stop()
        {
            if (!_running)
            {
                return;
            }

            _board.BatteryPercentChanged -= Board_BatteryPercentChanged;

            _running = false;

            System.Diagnostics.Debug.WriteLine("DEACTIVATED Battery Percent detector");
        }

        private void Board_BatteryPercentChanged(object sender, BatteryPercentChangedEventArgs e)
        {
            var currentTime = DateTime.Now;

            if (currentTime > _nextReportTime
                && ShouldAnnounceValue(e.batteryPercentValue))
            {
                if (_announcedValues.ContainsKey(e.batteryPercentValue)
                    && _announcedValues[e.batteryPercentValue] > currentTime)
                {
                    System.Diagnostics.Debug.WriteLine($"Skipping announcement for Battery Percent detector: {e.batteryPercentValue} percent.");
                    return; // Too soon to announce same value
                }

                _announcedValues[e.batteryPercentValue] = currentTime + GapTimeReportSameValue;
                _nextReportTime = currentTime + GapTimeReport;

                SpeakMessage($"Battery update: {e.batteryPercentValue} percent.");
            }
        }

        private bool ShouldAnnounceValue(int batteryPercent)
        {
            return batteryPercent % 10 == 0;
        }
    }
}
