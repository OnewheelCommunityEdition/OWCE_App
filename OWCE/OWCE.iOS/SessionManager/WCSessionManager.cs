using System.Collections.Generic;
using System.Linq;
using Foundation;
using Newtonsoft.Json;
using System;

// In the watch app this is just the SharedEnums
using OWCE;

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

		public Action<WCSession, Dictionary<WatchMessage, object>> ApplicationContextUpdated;
		public Action<WCSession, Dictionary<WatchMessage, object>> MessageReceived;


		private WCSession validSession
		{
			get
			{
#if __IOS__
				System.Diagnostics.Debug.WriteLine($"Paired status:{(session.Paired ? '✓' : '✗')}");
				System.Diagnostics.Debug.WriteLine($"Watch App Installed status:{(session.WatchAppInstalled ? '✓' : '✗')}\n");
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

		Dictionary<WatchMessage, NSString> _watchMessageToNativeString = new Dictionary<WatchMessage, NSString>();
		Dictionary<NSString, WatchMessage> _nativeStringToWatchMessage = new Dictionary<NSString, WatchMessage>();

		private WCSessionManager() : base()
		{
			// Prefil the _watchMessageToNativeString dictionary with enums and their NSString so we don't have to make them every time.
			var values = Enum.GetValues(typeof(WatchMessage));
			foreach (WatchMessage value in values)
            {
				var name = new NSString(Enum.GetName(typeof(WatchMessage), value));
				_watchMessageToNativeString[value] = name;
				_nativeStringToWatchMessage[name] = value;
			}
		}

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
				System.Diagnostics.Debug.WriteLine($"Started Watch Connectivity Session on {Device}");
			}
		}

		public override void SessionReachabilityDidChange(WCSession session)
		{
			System.Diagnostics.Debug.WriteLine($"Watch connectivity Reachable:{(session.Reachable ? '✓' : '✗')} from {Device}");
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

		/*
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
						System.Diagnostics.Debug.WriteLine($"Sent App Context from {Device} \nPayLoad: {NSApplicationContext.ToString()} \n");
					}
					else
					{
						System.Diagnostics.Debug.WriteLine($"Error Updating Application Context: {error.LocalizedDescription}");
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Exception Updating Application Context: {ex.Message}");
				}
			}
		}
		*/

		public void SendMessage(Dictionary<WatchMessage, object> message)
		{
			// Application context doesnt need the watch to be reachable, it will be received when opened
			if (validSession != null)
			{
				try
				{
					var MessageToSend = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(message.Values.ToArray(), message.Keys.Select(x => _watchMessageToNativeString[x]).ToArray());
					validSession.SendMessage(MessageToSend, null, null);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Exception Updating Application Context: {ex.Message}");
				}
			}
		}

		/*
		public override void DidReceiveApplicationContext(WCSession session, NSDictionary<NSString, NSObject> applicationContext)
		{
			System.Diagnostics.Debug.WriteLine($"Receiving App Context on {Device}");
			if (ApplicationContextUpdated != null)
			{
				var keys = applicationContext.Keys.Select(k => k.ToString()).ToArray();
				var values = applicationContext.Values.Select(v => JsonConvert.DeserializeObject(v.ToString())).ToArray();
				var dictionary = keys.Zip(values, (k, v) => new { Key = k, Value = v })
									 .ToDictionary(x => x.Key, x => x.Value);
				ApplicationContextUpdated?.Invoke(session, null);
			}
		}
		*/

        public override void DidReceiveMessage(WCSession session, NSDictionary<NSString, NSObject> message)
        {
			System.Diagnostics.Debug.WriteLine($"Receiving Message on {Device}");
			if (MessageReceived != null)
			{
				var dictionary = new Dictionary<WatchMessage, object>();
				foreach (var nativeKey in message.Keys)
				{
					var key = _nativeStringToWatchMessage[nativeKey];

					if (key == WatchMessage.Awake || key == WatchMessage.Speed || key == WatchMessage.BatteryPercent)
					{
						dictionary[key] = (message[nativeKey] as NSNumber)?.Int32Value ?? 0;
					}
					else if (key == WatchMessage.Voltage)
					{
						dictionary[key] = (message[nativeKey] as NSNumber)?.FloatValue ?? 0f;
					}
					else if (key == WatchMessage.Distance || key == WatchMessage.SpeedUnitsLabel)
					{
						dictionary[key] = (message[nativeKey] as NSString)?.ToString() ?? String.Empty;
					}
				}
				MessageReceived?.Invoke(session, dictionary);			
			}
		}

		public bool IsReachable()
        {
			return validReachableSession != null;
        }

		#endregion
	}
}
