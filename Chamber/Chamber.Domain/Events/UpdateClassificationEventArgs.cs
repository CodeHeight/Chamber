using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class UpdateClassificationEventArgs : ChamberEventArgs
    {
        public MembershipUser User { get; set; }
        public Classification Classification { get; set; }
    }
}
