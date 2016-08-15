using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class AdminUpdateBusinessEventArgs : ChamberEventArgs
    {
        public Business Business { get; set; }
        public MembershipUser Admin { get; set; }
    }
}
