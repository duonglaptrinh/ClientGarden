using System;

namespace Mirabo.VoiceCall
{
    public abstract class DownPeerHandler : IDisposable
    {
        protected ISignaler _signaler;
        protected string _peerId;

        public DownPeerHandler(string id, ISignaler signaler)
        {
            _peerId = id;
            _signaler = signaler;
        }

        public virtual void Dispose()
        {
            _signaler?.Dispose();
        }
    }
}
