using System;
using System.Threading.Tasks;
using OWCE.DependencyInterfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.Droid.DependencyImplementations.UserAgent))]

namespace OWCE.Droid.DependencyImplementations
{
    public class UserAgent : IUserAgent
    {
        public Task<string> GetSystemUserAgent()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            var systemUserAgent = Java.Lang.JavaSystem.GetProperty("http.agent");
            if (String.IsNullOrWhiteSpace(systemUserAgent))
            {
                taskCompletionSource.TrySetResult(String.Empty);
            }
            else
            {
                taskCompletionSource.TrySetResult(systemUserAgent);
            }

            return taskCompletionSource.Task;
        }
    }
}
