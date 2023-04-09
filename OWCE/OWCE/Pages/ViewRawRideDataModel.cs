using System;
using OWCE.Protobuf;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using OWCE.Network;

namespace OWCE.Pages
{
	public class ViewRawRideDataModel : Xamarin.CommunityToolkit.ObjectModel.ObservableObject
    {
        string dataFile = String.Empty;
        public string DataFile
        {
            get => dataFile;
            set => SetProperty(ref dataFile, value);
        }

        SubmitRideRequest submitRideRequest;
        public SubmitRideRequest SubmitRideRequest
        {
            get => submitRideRequest;
            set => SetProperty(ref submitRideRequest, value);
        }

        List<OWBoardEvent> boardEvents = new List<OWBoardEvent>();
        public List<OWBoardEvent> BoardEvents
        {
            get => boardEvents;
            set => SetProperty(ref boardEvents, value);
        }


        WeakReference<ViewRawRideDataPage> _page = null;

        public ViewRawRideDataModel(ViewRawRideDataPage page)
        {
            _page = new WeakReference<ViewRawRideDataPage>(page);
        }

        internal void LoadData()
        {
            ThreadPool.QueueUserWorkItem(delegate {
                var boardEvents = new List<OWBoardEvent>();
                using (var inputFile = new FileStream(DataFile, FileMode.Open, FileAccess.Read))
                {
                    do
                    {
                        var currentEvent = OWBoardEvent.Parser.ParseDelimitedFrom(inputFile);
                        boardEvents.Add(currentEvent);
                    }
                    while (inputFile.Position < inputFile.Length);
                }

                BoardEvents = null;
                BoardEvents = boardEvents;            
            });
        }
    }
}

