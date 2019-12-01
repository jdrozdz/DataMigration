using System;
using System.Collections.Generic;

namespace DataMigration.Models
{
    public partial class UserAssociation
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public int? IdUser { get; set; }

        public virtual User IdUserNavigation { get; set; }
    }
}
