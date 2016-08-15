using System;

namespace Chamber.Domain.Events
{
    public abstract class ChamberEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}