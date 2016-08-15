using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class AdminAddBusinessContactEventArgs : ChamberEventArgs
    {
        public BusinessContact BusinessContact { get; set; }
        public MembershipUser Admin { get; set; }
    }
}