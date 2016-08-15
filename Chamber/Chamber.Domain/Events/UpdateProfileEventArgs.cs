using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class UpdateProfileEventArgs : ChamberEventArgs
    {
        public MembershipUser User { get; set; }
    }
}