using System.Collections.Generic;
using System.Linq;
using Foundation;
using Newtonsoft.Json;
using System;

namespace WatchConnectivity
{
	// Handles WatchConnectivity sessions.
	// Based on sample Xamarin code in https://github.com/xamarin/ios-samples/tree/main/watchOS/WatchConnectivity
	public sealed class WCSessionManager : WCSessionDelegate
	{
		private static readonly WCSessionManager sharedManager = new WCSessionManager();
		private static WCSession session = WCSession.IsSupported ? WCSession.DefaultSession : null;

#if __IOS__
		public static string Device = "Phone";
#else
		public static string Device = "Watch";
#endif

		public event ApplicationContextUpdatedHandler ApplicationContextUpdated;
		public delegate void ApplicationContextUpdatedHandler(WCSession session, Dictionary<string, object> applicationContext);

		public event MessageReceivedHandler MessageReceived;
		public delegate void MessageReceivedHandler(WCSession session, Dictionary<string, object> applicationContext);


		private WCSession validSession
		{
			get
			{
#if __IOS__
				Console.WriteLine($"Paired status:{(session.Paired ? '✓' : '✗')}\n");
				Console.WriteLine($"Watch App Installed status:{(session.WatchAppInstalled ? '✓' : '✗')}\n");
				return (session.Paired && session.WatchAppInstalled) ? session : null;
#else
				return session;
#endif
			}
		}

		private WCSession validReachableSession
		{
			get
			{
				return session.Reachable ? validSession : null;
			}
		}

		private WCSessionManager() : base() { }

		public static WCSessionManager SharedManager
		{
			get
			{
				return sharedManager;
			}
		}

		public void StartSession()
		{
			if (session != null)
			{
				session.Delegate = this;
				session.ActivateSession();
				Console.WriteLine($"Started Watch Connectivity Session on {Device}");
			}
		}

		public override void SessionReachabilityDidChange(WCSession session)
		{
			Console.WriteLine($"Watch connectivity Reachable:{(session.Reachable ? '✓' : '✗')} from {Device}");
			// handle session reachability change
			if (session.Reachable)
			{
				// great! continue on with Interactive Messaging
			}
			else
			{
				// 😥 prompt the user to unlock their iOS device
			}
		}

		#region Application Context Methods

		public void UpdateApplicationContext(Dictionary<string, object> applicationContext)
		{
			// Application context doesnt need the watch to be reachable, it will be received when opened
			if (validSession != null)
			{
				try
				{
					var NSValues = applicationContext.Values.Select(x => new NSString(JsonConvert.SerializeObject(x))).ToArray();
					var NSKeys = applicationContext.Keys.Select(x => new NSString(x)).ToArray();
					var NSApplicationContext = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(NSValues, NSKeys);
					NSError error;
					var sendSuccessfully = validSession.UpdateApplicationContext(NSApplicationContext, out error);
					if (sendSuccessfully)
					{
						Console.WriteLine($"Sent App Context from {Device} \nPayLoad: {NSApplicationContext.ToString()} \n");
					}
					else
					{
						Console.WriteLine($"Error Updating Application Context: {error.LocalizedDescription}");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Exception Updating Application Context: {ex.Message}");
				}
			}
		}

		public void SendMessage(Dictionary<string, object> message)
		{
			// Application context doesnt need the watch to be reachable, it will be received when opened
			if (validSession != null)
			{
				try
				{
					var NSValues = message.Values.Select(x => new NSString(JsonConvert.SerializeObject(x))).ToArray();
					var NSKeys = message.Keys.Select(x => new NSString(x)).ToArray();
					var MessageToSend = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(NSValues, NSKeys);
					validSession.SendMessage(MessageToSend, null, null);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Exception Updating Application Context: {ex.Message}");
				}
			}
		}

		public override void DidReceiveApplicationContext(WCSession session, NSDictionary<NSString, NSObject> applicationContext)
		{
			Console.WriteLine($"Receiving App Context on {Device}");
			if (ApplicationContextUpdated != null)
			{
				var keys = applicationContext.Keys.Select(k => k.ToString()).ToArray();
				var values = applicationContext.Values.Select(v => JsonConvert.DeserializeObject(v.ToString())).ToArray();
				var dictionary = keys.Zip(values, (k, v) => new { Key = k, Value = v })
									 .ToDictionary(x => x.Key, x => x.Value);

				ApplicationContextUpdated(session, dictionary);
			}
		}

        public override void DidReceiveMessage(WCSession session, NSDictionary<NSString, NSObject> message)
        {
			Console.WriteLine($"Receiving Message on {Device}");
			if (MessageReceived != null)
			{
				var keys = message.Keys.Select(k => k.ToString()).ToArray();
				var values = message.Values.Select(v => JsonConvert.DeserializeObject(v.ToString())).ToArray();
				var dictionary = keys.Zip(values, (k, v) => new { Key = k, Value = v })
									 .ToDictionary(x => x.Key, x => x.Value);

				MessageReceived(session, dictionary);
			}
		}

		#endregion
	}
}
