using System;
using Chamber.Utilities;
using System.Collections.Generic;

namespace Chamber.Domain.DomainModel
{
    public partial class BusinessBalance : Entity
    {
        public BusinessBalance()
        {
            Id = GuidComb.GenerateComb();
            Active = true;
        }

        public Guid Id { get; set; }
        public decimal? AmountDue { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? Credit { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public bool Active { get; set; }
        public virtual Business Business { get; set; }
    }
}
