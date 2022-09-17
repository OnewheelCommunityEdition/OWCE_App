using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using MvvmHelpers;
using OWCE.Protobuf;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public class LogSummary
    {
        public string Filename { get; set; }
        public string Name => Path.GetFileNameWithoutExtension(Filename);
        public DateTime Created { get; set; }
    }

    public partial class ListLogsPage : ContentPage
    {

        ObservableRangeCollection<LogSummary> _logsList = new ObservableRangeCollection<LogSummary>();
        public ObservableRangeCollection<LogSummary> LogsList { get { return _logsList; } }

        bool _isRefreshing = false;
        public bool IsRefreshing
        {
            set
            {
                if (_isRefreshing == value)
                {
                    return;
                }

                _isRefreshing = value;
                OnPropertyChanged();
            }
            get
            {
                return _isRefreshing;
            }
        }

        public ICommand RefreshCommand => new Command(() =>
        {
            IsRefreshing = true;
            RefreshLogs();
            IsRefreshing = false;
        });

        public ListLogsPage()
        {
            InitializeComponent();

            BindingContext = this;

            var ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            RefreshLogs();
        }

        void RefreshLogs()
        {
            var tempLogSummary = new List<LogSummary>();
            var files = Directory.GetFiles(App.Current.LogsDirectory, "*.bin");
            foreach (var file in files)
            {
                var logSummary = new LogSummary()
                {
                    Filename = file,
                    Created = File.GetCreationTime(file),
                };
                tempLogSummary.Add(logSummary);
            }

            _logsList.ReplaceRange(tempLogSummary);
        }

        void CollectionView_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            if (sender is CollectionView collectionView)
            {
                var previous = (e.PreviousSelection.FirstOrDefault() as LogSummary);
                var current = (e.CurrentSelection.FirstOrDefault() as LogSummary);

                collectionView.SelectedItem = null;

                if (previous == null && current != null)
                {
                    try
                    {
                        var events = new List<OWBoardEvent>();
                        var logPath = Path.Combine(App.Current.LogsDirectory, current.Filename);
                        using (FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read))
                        {
                            OWBoardEvent owBoardEvent;
                            do
                            {
                                owBoardEvent = OWBoardEvent.Parser.ParseDelimitedFrom(fs);
                                if (owBoardEvent != null)
                                {
                                    events.Add(owBoardEvent);
                                }
                            }
                            while (fs.Position < fs.Length);
                        }
                    }
                    catch (Exception err)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: " + err.Message);
                    }
                }

            }
        }
    }
}
