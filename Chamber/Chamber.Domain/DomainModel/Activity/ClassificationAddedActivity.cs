namespace Chamber.Domain.DomainModel.Activity
{
    public sealed class ClassificationAddedActivity : ActivityBase
    {
        public const string KeyUserId = @"UserID";
        public const string KeyClassificationId = @"ClassificationID";

        public MembershipUser User { get; set; }
        public Classification Classification { get; set; }

        public ClassificationAddedActivity(Activity activity, MembershipUser user, Classification classification)
        {
            ActivityMapped = activity;
            User = user;
            Classification = classification;
        }

        public static Activity GenerateMappedRecord(MembershipUser user, Classification classification)
        {
            return new Activity
            {
                Data = KeyUserId + Equality + user.Id + Separator +
                    KeyClassificationId + Equality + classification.Id,
                Timestamp = classification.CreateDate,
                Type = ActivityType.ClassificationAdded.ToString()
            };
        }
    }
}