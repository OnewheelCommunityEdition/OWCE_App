namespace OWCE
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Essentials;

    /// <summary>
    /// TTS Messages go through this class to decide whether based on message priority the message cancels & overrides another message being spoken.
    /// </summary>
    public class TextToSpeechProvider
    {
        private CancellationTokenSource _speedReportingCts;
        private Task _speechTask = null;
        private int _currentSpeechTaskPriority = -1;
        private object _ttsLock = new object();

        public void SpeakMessage(string text, int priority = 0)
        {
            SpeakMessage(text, null, priority);
        }

        public void SpeakMessage(string text, SpeechOptions speechOptions, int priority = 0)
        {
            lock (_ttsLock)
            {
                if (!CanSpeak(priority))
                {
                    return;
                }

                CancelSpeech();
                _speedReportingCts = new CancellationTokenSource();
                _currentSpeechTaskPriority = priority;
                _speechTask = TextToSpeech.SpeakAsync(text, speechOptions, _speedReportingCts.Token);
            }
        }

        // Cancel speech if a cancellation token exists & hasn't been already requested.
        private void CancelSpeech()
        {
            if (_speedReportingCts?.IsCancellationRequested ?? true)
            {
                return;
            }

            _speedReportingCts.Cancel();
        }

        private bool CanSpeak(int priority)
        {
            return _speechTask == null
                || _speechTask.IsCompleted
                || _speechTask.IsFaulted
                || _speechTask.IsCanceled
                || priority >= _currentSpeechTaskPriority;
        }
    }
}
