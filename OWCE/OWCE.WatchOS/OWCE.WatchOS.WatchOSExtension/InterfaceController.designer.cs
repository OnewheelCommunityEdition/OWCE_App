// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace OWCE.WatchOS.WatchOSExtension
{
	[Register ("InterfaceController")]
	partial class InterfaceController
	{
		[Outlet]
		WatchKit.WKInterfaceLabel batteryPercentageLabel { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel errorMessages { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel myLabel { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel speedLabel { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel speedUnitsLabel { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel tripDistanceLabel { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel voltageLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (batteryPercentageLabel != null) {
				batteryPercentageLabel.Dispose ();
				batteryPercentageLabel = null;
			}

			if (errorMessages != null) {
				errorMessages.Dispose ();
				errorMessages = null;
			}

			if (myLabel != null) {
				myLabel.Dispose ();
				myLabel = null;
			}

			if (speedLabel != null) {
				speedLabel.Dispose ();
				speedLabel = null;
			}

			if (tripDistanceLabel != null) {
				tripDistanceLabel.Dispose ();
				tripDistanceLabel = null;
			}

			if (voltageLabel != null) {
				voltageLabel.Dispose ();
				voltageLabel = null;
			}

			if (speedUnitsLabel != null) {
				speedUnitsLabel.Dispose ();
				speedUnitsLabel = null;
			}
		}
	}
}
