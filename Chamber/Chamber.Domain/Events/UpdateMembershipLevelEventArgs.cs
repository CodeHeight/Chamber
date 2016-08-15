using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class UpdateMembershipLevelEventArgs : ChamberEventArgs
    {
        public MembershipUser User { get; set; }
        public MembershipLevel MembershipLevel { get; set; }
    }
}
