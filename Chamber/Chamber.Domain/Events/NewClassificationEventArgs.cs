using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Events
{
    public class NewClassificationEventArgs : ChamberEventArgs
    {
        public MembershipUser User { get; set; }
        public Classification Classification { get; set; }
        public ClassificationCreateStatus CreateStatus { get; set; }
    }
}
