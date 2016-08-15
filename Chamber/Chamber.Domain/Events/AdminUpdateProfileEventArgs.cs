using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class AdminUpdateProfileEventArgs : ChamberEventArgs
    {
        public MembershipUser User { get; set; }
        public MembershipUser Admin { get; set; }
    }
}
