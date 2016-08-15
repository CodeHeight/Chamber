using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class RegisterUserEventArgs : ChamberEventArgs
    {
        public MembershipUser User { get; set; }
        public MembershipCreateStatus CreateStatus { get; set; }
    }
}