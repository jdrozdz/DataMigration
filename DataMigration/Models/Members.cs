using System;
using System.Collections.Generic;

namespace DataMigration.Models
{
    public partial class Members
    {
        public Members()
        {
            MemberAssociation = new HashSet<MemberAssociation>();
        }

        public int MemberId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Phone { get; set; }
        public DateTime? Registered { get; set; }
        public int? SkillId { get; set; }
        public string Token { get; set; }

        public virtual Skills Skill { get; set; }
        public virtual ICollection<MemberAssociation> MemberAssociation { get; set; }
    }
}
