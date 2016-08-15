using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class AdminRegisterUserEventArgs : ChamberEventArgs
    {
        public MembershipUser User { get; set; }
        public MembershipUser Admin { get; set; }
        public MembershipCreateStatus CreateStatus { get; set; }
    }
}
