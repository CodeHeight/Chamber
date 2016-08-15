using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class NewMembershipLevelEventArgs : ChamberEventArgs
    {
        public MembershipUser User { get; set; }
        public MembershipLevel MembershipLevel { get; set; }
        public MemberhipLevelCreateStatus CreateStatus { get; set; }
    }
}