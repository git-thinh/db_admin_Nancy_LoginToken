using System;

namespace host.websocket
{
    public class HandshakeException : Exception
    {
        public HandshakeException() : base() { }
        
        public HandshakeException(string message) : base(message) {}
        
        public HandshakeException(string message, Exception innerException) : base(message, innerException) {}
    }
}

