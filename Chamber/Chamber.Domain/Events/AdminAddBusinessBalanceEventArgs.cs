using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class AdminAddBusinessBalanceEventArgs : ChamberEventArgs
    {
        public BusinessBalance BusinessBalance { get; set; }
        public MembershipUser Admin { get; set; }
    }
}
