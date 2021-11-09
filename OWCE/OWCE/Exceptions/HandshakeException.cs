using System;
namespace OWCE.Exceptions
{
    public class HandshakeException : Exception
    {
        public bool ShouldDisconnect { get; private set; } = true;

        public HandshakeException(string message, bool shouldDisconnect) : base(message)
        {

        }

    }
}
