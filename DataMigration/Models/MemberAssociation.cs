using System;
using System.Collections.Generic;

namespace DataMigration.Models
{
    public partial class MemberAssociation
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string UnitId { get; set; }

        public virtual Members Member { get; set; }
    }
}
