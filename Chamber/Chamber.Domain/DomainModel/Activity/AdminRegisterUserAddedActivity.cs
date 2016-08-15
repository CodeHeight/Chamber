namespace Chamber.Domain.DomainModel.Activity
{
    public class AdminRegisterUserAddedActivity : ActivityBase
    {
        public const string KeyMembershipUserId = @"MembershipUserID";
        public const string KeyAdminId = @"AdminID";

        public MembershipUser MembershipUser { get; set; }
        public MembershipUser Admin { get; set; }

        public AdminRegisterUserAddedActivity(Activity activity, MembershipUser membershipUser, MembershipUser admin)
        {
            ActivityMapped = activity;
            MembershipUser = membershipUser;
            Admin = admin;
        }

        public static Activity GenerateMappedRecord(MembershipUser membershipUser, MembershipUser admin)
        {
            return new Activity
            {
                Data = KeyMembershipUserId + Equality + membershipUser.Id + Separator +
                    KeyAdminId + Equality + admin.Id,
                Timestamp = membershipUser.CreateDate,
                Type = ActivityType.AdminRegisteredUserAdded.ToString()
            };
        }
    }
}