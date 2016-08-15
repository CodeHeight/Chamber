namespace Chamber.Domain.DomainModel.Activity
{
    public sealed class MembershipLevelAddedActivity : ActivityBase
    {
        public const string KeyUserId = @"UserID";
        public const string KeyMembershipLevelId = @"MembershipLevelID";

        public MembershipUser User { get; set; }
        public MembershipLevel MembershipLevel { get; set; }

        public MembershipLevelAddedActivity(Activity activity, MembershipUser user, MembershipLevel membershipLevel)
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
                Timestamp = membershipLevel.CreateDate,
                Type = ActivityType.MembershipLevelAdded.ToString()
            };
        }
    }
}