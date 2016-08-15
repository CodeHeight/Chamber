namespace Chamber.Domain.DomainModel.Activity
{
    public class AdminBusinessContactAddedActivity : ActivityBase
    {
        public const string KeyBusinessContactId = @"BusinessContactID";
        public const string KeyAdminId = @"AdminID";

        public BusinessContact BusinessContact { get; set; }
        public MembershipUser Admin { get; set; }

        public AdminBusinessContactAddedActivity(Activity activity, BusinessContact businessContact, MembershipUser admin)
        {
            ActivityMapped = activity;
            BusinessContact = businessContact;
            Admin = admin;
        }

        public static Activity GenerateMappedRecord(BusinessContact businessContact, MembershipUser admin)
        {
            return new Activity
            {
                Data = KeyBusinessContactId + Equality + businessContact.Id + Separator +
                    KeyAdminId + Equality + admin.Id,
                Timestamp = businessContact.CreateDate,
                Type = ActivityType.AdminBusinessContactAdded.ToString()
            };
        }
    }
}