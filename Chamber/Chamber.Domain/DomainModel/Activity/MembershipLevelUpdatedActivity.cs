using System;

namespace Chamber.Domain.DomainModel.Activity
{
    public sealed class MembershipLevelUpdatedActivity : ActivityBase
    {
        public const string KeyUserId = @"UserID";
        public const string KeyMembershipLevelId = @"MembershipLevelID";

        public MembershipUser User { get; set; }
        public MembershipLevel MembershipLevel { get; set; }

        public MembershipLevelUpdatedActivity(Activity activity, MembershipUser user, MembershipLevel membershipLevel)
        {
            ActivityMapped = activity;
            User = user;
            MembershipLevel = membershipLevel;
        }

        public static Activity GenerateMappedRecord(MembershipUser user, MembershipLevel membershipLevel)
        {
            return new Activity
            {
                Data = KeyUserId + Equality + user.Id + Separator +
                    KeyMembershipLevelId + Equality + membershipLevel.Id,
                Timestamp = DateTime.UtcNow,
                Type = ActivityType.MembershipLevelUpdated.ToString()
            };
        }
    }
}