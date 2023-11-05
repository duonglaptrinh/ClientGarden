using System;

namespace Mirabo.VoiceCall
{
    public abstract class UpPeerHandler : IDisposable
    {
        protected ISignaler _signaler;

        public UpPeerHandler(ISignaler signaler)
        {
            _signaler = signaler;
        }

        public virtual void Dispose()
        {
            _signaler?.Dispose();
        }
    }
}

