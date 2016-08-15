using System;
using Chamber.Utilities;

namespace Chamber.Domain.DomainModel
{
    public partial class Email : Entity
    {
        public Email()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string EmailTo { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string NameTo { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
