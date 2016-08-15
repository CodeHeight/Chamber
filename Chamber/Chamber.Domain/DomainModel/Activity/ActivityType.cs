using System;
using Chamber.Utilities;

namespace Chamber.Domain.DomainModel.Activity
{
    public enum ActivityType
    {
        MemberJoined,
        ProfileUpdated,
        AdminProfileUpdated,
        ClassificationAdded,
        ClassificationUpdated,
        ClassificationRemoved,
        MembershipLevelAdded,
        MembershipLevelUpdated,
        AdminBusinessAdded,
        AdminBusinessUpdated,
        AdminRegisteredUserAdded,
        AdminBusinessContactAdded,
        AdminBusinessBalanceAdded
    }

    public class Activity : Entity
    {
        public Activity()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}