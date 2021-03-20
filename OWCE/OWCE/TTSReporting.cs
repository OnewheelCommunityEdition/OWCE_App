namespace OWCE
{
    using System;
    using Xamarin.Essentials;

    public abstract class TTSReporting
    {
        protected int TextToSpeechPriority = 0; // Higher or equal priority overrides currently active TTS

        private TextToSpeechProvider _textToSpeechProvider = null;

        public TTSReporting(TextToSpeechProvider ttsProvider)
        {
            _textToSpeechProvider = ttsProvider;
        }

        public abstract void Start();
        public abstract void Stop();

        public void SpeakMessage(string text)
        {
            SpeakMessage(text, null);
        }

        public void SpeakMessage(string text, SpeechOptions speechOptions)
        {
            if (_textToSpeechProvider == null)
            {
                throw new NullReferenceException("Class did not call base constructor");
            }

            _textToSpeechProvider.SpeakMessage(text, speechOptions, TextToSpeechPriority);
        }
    }
}
