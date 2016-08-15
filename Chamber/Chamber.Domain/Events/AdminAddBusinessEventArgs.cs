using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class AdminAddBusinessEventArgs : ChamberEventArgs
    {
        public Business Business { get; set; }
        public MembershipUser Admin { get; set; }
        public BusinessCreateStatus CreateStatus { get; set; }
    }
}