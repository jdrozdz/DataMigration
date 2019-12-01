using System;
using System.Collections.Generic;

namespace DataMigration.Models
{
    public partial class Skills
    {
        public Skills()
        {
            Members = new HashSet<Members>();
        }

        public int Id { get; set; }
        public string Skill { get; set; }

        public virtual ICollection<Members> Members { get; set; }
    }
}
