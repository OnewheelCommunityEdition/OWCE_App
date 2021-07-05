using System;

using Foundation;
using ClockKit;
using UIKit;

namespace OWCE.WatchOS.WatchOSExtension
{
	[Register("ComplicationController")]
	public class ComplicationController : CLKComplicationDataSource
	{
		public ComplicationController()
		{
		}

		public override void GetComplicationDescriptors(Action<CLKComplicationDescriptor[]> handler)
        {
			CLKComplicationFamily[] family = { CLKComplicationFamily.ModularSmall };
			CLKComplicationDescriptor desc = new CLKComplicationDescriptor("OWCE", "OWCE", family);
			CLKComplicationDescriptor[] descriptors = { desc };
			handler(descriptors);
		}

		public override void GetCurrentTimelineEntry(CLKComplication complication, Action<CLKComplicationTimelineEntry> handler)
		{
			// Call the handler with the current timeline entry
			Console.WriteLine("GetCurrentTimelineEntry");

			CLKComplicationTimelineEntry entry = null;
			try
			{
				var minutesPastHour = DateTime.Now.Minute.ToString();

				if (complication.Family == CLKComplicationFamily.ModularSmall)
				{
					var textTemplate1 = new CLKComplicationTemplateModularSmallSimpleText();
					textTemplate1.TextProvider = CLKSimpleTextProvider.FromText("OWCE");
					entry = CLKComplicationTimelineEntry.Create(NSDate.Now, textTemplate1);
					//var imageTemplate1 = new CLKComplicationTemplateModularSmallSimpleImage();
					//var imageProvider = new CLKImageProvider();
					//imageProvider.OnePieceImage = UIImage.FromBundle();
					//imageTemplate1.ImageProvider = imageProvider;

				}

			}
			catch (Exception x)
			{
				Console.WriteLine("Exception " + x);
			}
			handler(entry);
		}
		public override void GetPlaceholderTemplate(CLKComplication complication, Action<CLKComplicationTemplate> handler)
		{
			// This method will be called once per supported complication, and the results will be cached
			Console.WriteLine("GetPlaceholderTemplate for " + complication);

			CLKComplicationTemplate template = null;

			if (complication.Family == CLKComplicationFamily.ModularSmall)
			{
				var textTemplate = new CLKComplicationTemplateModularSmallSimpleText();
				textTemplate.TextProvider = CLKSimpleTextProvider.FromText("OWCE");
				template = textTemplate;
			}
			//else if (complication.Family == CLKComplicationFamily.UtilitarianSmall)
			//{
			//	var textTemplate = new CLKComplicationTemplateUtilitarianSmallFlat();
			//	textTemplate.TextProvider = CLKSimpleTextProvider.FromText("OWCE");
			//	template = textTemplate;

			//}
			//else if (complication.Family == CLKComplicationFamily.CircularSmall)
			//{
			//	var textTemplate = new CLKComplicationTemplateCircularSmallRingText();
			//	textTemplate.TextProvider = CLKSimpleTextProvider.FromText("OWCE");
			//	template = textTemplate;
			//}
			//else
			//{
			//	Console.WriteLine("GetPlaceholderTemplate: Complication not supported (" + complication + ")");
			//}

			handler(template);
		}

		public override void GetSupportedTimeTravelDirections(CLKComplication complication, Action<CLKComplicationTimeTravelDirections> handler)
		{
			// Retrieves the time travel directions supported by your complication
			Console.WriteLine("GetSupportedTimeTravelDirections");
			handler(CLKComplicationTimeTravelDirections.None);
		}
	}
}

