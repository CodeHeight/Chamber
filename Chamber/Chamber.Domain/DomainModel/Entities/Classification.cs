using System;
using System.Collections.Generic;
using Chamber.Utilities;

namespace Chamber.Domain.DomainModel
{
    public enum ClassificationCreateStatus
    {
        Success,
        DuplicateName,
        InvalidName,
        NameRejected,
    }

    public partial class Classification : Entity
    {
        public Classification()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Active { get; set; }
        public IList<Business> Businesses { get; set; }
    }
}
