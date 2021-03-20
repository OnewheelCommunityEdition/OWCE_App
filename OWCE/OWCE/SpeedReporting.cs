namespace OWCE
{
    using OWCE.Converters;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Xamarin.Essentials;

    public class SpeedReporting : TTSReporting
    {
        private readonly TimeSpan GapTimeIncrementFast = TimeSpan.FromSeconds(2);

        private bool _running = false;
        private int _lastReportedSpeed = 0;
        private DateTime _nextHighSpeedTime = DateTime.Now;
        private DateTime _nextSlowSpeedTime = DateTime.Now;

        private OWBoard _board;

        private int MinSpeed
        {
            get
            {
                return App.Current.SpeedReportingMinimum > 0 ? App.Current.SpeedReportingMinimum : (App.Current.MetricDisplay ? 5 : 3);
            }
        }

        private int TimeInSecsIncrementSlow
        {
            get
            {
                return App.Current.SpeedReportingBaselineTimeout > 0 ? App.Current.SpeedReportingBaselineTimeout : 10;
            }
        }

        public SpeedReporting(OWBoard board, TextToSpeechProvider ttsProvider) : base(ttsProvider)
        {
            _board = board;
            TextToSpeechPriority = 1;

            Debug.WriteLine($"Speed Recording started for {board.BoardType} {board.ID} minspeed {MinSpeed} slowspeed {TimeInSecsIncrementSlow}");
        }

        public override void Start()
        {
            if (_running)
            {
                return;
            }

            _running = true;

            System.Diagnostics.Debug.WriteLine("ACTIVATED Speed Reporting");

            _board.SpeedChanged += Board_SpeedChanged;
        }

        public override void Stop()
        {
            if (!_running)
            {
                return;
            }

            _board.SpeedChanged -= Board_SpeedChanged;

            _running = false;

            System.Diagnostics.Debug.WriteLine("DEACTIVATED Speed Reporting");
        }

        private void Board_SpeedChanged(object sender, SpeedChangedEventArgs e)
        {
            var currentTime = DateTime.Now;
            int newSpeed = (int)e.speedValue;

            if (newSpeed >= MinSpeed)
            {
                if (currentTime > _nextSlowSpeedTime)
                {
                    _lastReportedSpeed = newSpeed;
                    _nextSlowSpeedTime = currentTime + TimeSpan.FromSeconds(TimeInSecsIncrementSlow);
                    _nextHighSpeedTime = currentTime + GapTimeIncrementFast;

                    string unit = App.Current.MetricDisplay ? "kph" : "mph";

                    System.Diagnostics.Debug.WriteLine($"New slow speed {newSpeed}");
                    SpeakMessage($"Speed {newSpeed} {unit}");
                }
                else if (currentTime > _nextHighSpeedTime)
                {
                    if (newSpeed > _lastReportedSpeed)
                    {
                        _lastReportedSpeed = newSpeed;
                        _nextSlowSpeedTime = currentTime + TimeSpan.FromSeconds(TimeInSecsIncrementSlow);
                        _nextHighSpeedTime = currentTime + GapTimeIncrementFast;

                        System.Diagnostics.Debug.WriteLine($"New fast speed {newSpeed}");
                        SpeakMessage($"{newSpeed}");
                    }
                }
            }
        }
    }
}
