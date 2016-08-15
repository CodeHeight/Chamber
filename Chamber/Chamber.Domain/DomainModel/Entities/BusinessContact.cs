using System;
using Chamber.Utilities;


namespace Chamber.Domain.DomainModel
{
    public partial class BusinessContact : Entity
    {
        public BusinessContact()
        {
            Id = GuidComb.GenerateComb();
            Active = true;
        }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual Business Business { get; set; }
    }
}
