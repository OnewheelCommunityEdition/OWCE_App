using System;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using OWCE.DependencyInterfaces;
using WebKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.iOS.DependencyImplementations.UserAgent))]

namespace OWCE.iOS.DependencyImplementations
{
    public class UserAgent : IUserAgent
    {
        public Task<string> GetSystemUserAgent()
        {
            var taskCompletionSource= new TaskCompletionSource<string>();
            var webView = new WKWebView(CGRect.Empty, new WKWebViewConfiguration());
            webView.EvaluateJavaScript("navigator.userAgent", (NSObject result, NSError err) =>
            {
                if (result != null && result is NSString resultString)
                {
                    taskCompletionSource.TrySetResult(resultString.ToString());
                    return;
                }

                // Return empty string if it failed.
                taskCompletionSource.TrySetResult(String.Empty);
            });
            return taskCompletionSource.Task;
        }
    }
}
